using System.Security.Cryptography;
using System.Text;
using System.Data;
using APIOne.Data;
using APIOne.DTOs;
using APIOne.Helpers;
using APIOne.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace APIOne.Controllers;

// Requires auth/token for routes, unless they are marked with [AllowAnonymous]
[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    // [AllowAnonymous] allows access to route without auth/token
    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult RegisterUser(UserRegistrationDTO userToRegister)
    {
        if (userToRegister.Password == userToRegister.PasswordConfirm)
        {
            var sql = $"SELECT a.email FROM tutorialAppSchema.Auth as a WHERE email = '{userToRegister.Email}'";
            var userAlreadyExists = _dapper.LoadData<string>(sql);
            if (userAlreadyExists.Count() == 0)
            {
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = _authHelper.GetPasswordHash(userToRegister.Password, passwordSalt);

                // Need to use params for byte array since they're not a string:
                var sqlAddAuth = @$"
                    INSERT INTO TutorialAppSchema.Auth(
                        email, salt, hash
                    ) VALUES (
                        '{userToRegister.Email}',
                        @sqlSalt,
                        @sqlHash
                    )
                ";

                var sqlSalt = new SqlParameter("@sqlSalt", SqlDbType.VarBinary);
                sqlSalt.Value = passwordSalt;
                var sqlHash = new SqlParameter("@sqlHash", SqlDbType.VarBinary);
                sqlHash.Value = passwordHash;

                var sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(sqlSalt);
                sqlParameters.Add(sqlHash);

                if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    var sqlUserAdd = $@"INSERT INTO TutorialAppSchema.Users(
                        [FirstName],
                        [LastName],
                        [Email],
                        [Gender],
                        [Active]
                        ) VALUES (
                            '{userToRegister.FirstName}',
                            '{userToRegister.LastName}',
                            '{userToRegister.Email}',
                            '{userToRegister.Gender}',
                            1
                        )";

                    if (_dapper.ExecuteSql(sqlUserAdd))
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to add User entry");
                }

                throw new Exception("Failed to add Auth entry");
            }
            throw new Exception("User already exists");
        }
        throw new Exception("Passwords don't match");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult LoginUser(UserLoginDTO userLoginDto)
    {
        var sql = $"SELECT salt, hash FROM TutorialAppSchema.Auth WHERE email = '{userLoginDto.Email}'";
        var userConfirmLoginDto = _dapper.LoadDataSingle<UserConfirmLogin>(sql);
        if (userConfirmLoginDto != null)
        {
            var hash = _authHelper.GetPasswordHash(userLoginDto.Password, userConfirmLoginDto.Salt);

            // can't just check if hashes are equal because they are arrays and pointers to different locations
            for (int index = 0; index < hash.Length; index++)
            {
                if (hash[index] != userConfirmLoginDto.Hash[index])
                {
                    return StatusCode(401, "Incorrect password");
                }
            }

            var sqlUserId = $"SELECT userId from TutorialAppSchema.Users WHERE email = '{userLoginDto.Email}'";

            var userId = _dapper.LoadDataSingle<int>(sqlUserId);

            var token = _authHelper.CreateToken(userId);

            return Ok(new Dictionary<string, string>(){
                {"a", "b"},
                {"token", token}
            });
        }

        throw new Exception("Could not find user to login");

    }

    [HttpGet("RefreshToken")]
    public string RefreshToken()
    {
        // this is how you get the userId from the Bearer token in header (need to use postman, not swagger)
        // User is available on BaseController class and represents current user. Can get other claims besides userId too
        var idFromToken = User.FindFirst("userId")?.Value;

        // confirm this id is in DB
        var userId = _dapper.LoadDataSingle<int>($"SELECT userId FROM TutorialAppSchema.users where userId = '{idFromToken}'");

        return _authHelper.CreateToken(userId);
    }
}