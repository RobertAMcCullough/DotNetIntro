// EXAMPLE USING DAPPER
using APIOne.Data;
using APIOne.DTOs;
using APIOne.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIOne.Controllers;

[ApiController]
// tells it to remove "controller" from class name and use that as route, can also just type route name instead
// also adds "User/" before all the route names - so User/users/{id}
[Route("[controller]")] 
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    // config automatically passed in
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("users")]
    public IEnumerable<User> GetUsers()
    {
        return _dapper.LoadData<User>("SELECT * FROM TutorialAppSchema.Users");
    }

    [HttpGet("users/{id}")]
    public User? GetUserById(string id)
    {
        return _dapper.LoadDataSingle<User>($"SELECT * FROM TutorialAppSchema.Users WHERE UserId = {id}");
    }

    [HttpPut("editUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @$"
        UPDATE TutorialAppSchema.Users SET FirstName = '{user.FirstName}', LastName = '{user.LastName}', 
        Email ='{user.Email}', Gender ='{user.Gender}', Active = '{user.Active}'
        WHERE UserId = '{user.UserId}';
        ";
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to edit user");

    }

    [HttpPost("addUser")]
    public IActionResult AddUser(UserCreateDTO user) // can have a [FromBody] tag
    {
        string sql = @$"
        INSERT INTO TutorialAppSchema.Users ([FirstName], [LastName], [Email], [Gender], [Active])
        VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{user.Gender}', '{user.Active}');
        ";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to add user");
    }

    [HttpDelete("deleteUser/{id}")]
    public IActionResult DeleteUser(int id) {
        string sql = $"DELETE FROM TutorialAppSchema.Users WHERE userId = {id}";
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Could not delete user");
    }
}