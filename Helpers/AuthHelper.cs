using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace APIOne.Helpers;

public class AuthHelper
{
    IConfiguration _config;
    public AuthHelper(IConfiguration config)
    {
        Console.WriteLine("initializing helper");
        _config = config;
    }

    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string? passwordSaltPlusSecret = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        var saltPlusSecretByteArray = Encoding.ASCII.GetBytes(passwordSaltPlusSecret);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: saltPlusSecretByteArray,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 500,
            numBytesRequested: 256 / 8
        );
    }

    public string CreateToken(int userId)
    {
        // define claims
        var claims = new Claim[] {
            new Claim("userId", userId.ToString()),
            new Claim("otherInfo", "some other stuff")
        };

        var tokenKey = _config.GetSection("AppSettings:JwtKey").Value;

        // create symmetric key from config key
        var symmetricKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(tokenKey != null ? tokenKey : "")
        );

        // create credentials from symmetric key
        var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha512Signature);

        // create descriptor from claims and credentials
        // also, here we are using an object initilizer instead of a constructor - this lets 
        // us set the options we want vs a constructor which will require certain inputs
        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(14)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(descriptor);

        // turn token into string for client
        return tokenHandler.WriteToken(token);
    }

}