// EXAMPLE USING ENTITY FRAMEWORK AND USER REPOSITORY
using APIOne.Data;
using APIOne.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIOne.Controllers;

[ApiController]
[Route("[controller]")]
public class UserSalaryController : ControllerBase
{
    // DataContextEF _EF;
    IUserRepository _userRepository;

    public UserSalaryController(IConfiguration config, IUserRepository userRepository)
    {
        Console.WriteLine("user repository is");
        Console.WriteLine(typeof(UserRepository));
        _userRepository = userRepository;
    }

    [HttpGet("salary/{userId}")]
    public UserSalary? GetUserSalaryByUserId(int userId)
    {
        return _userRepository.GetUserSalary(userId);

        // Another way to do the same thing:
        // return _EF.UserSalary?.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();
    }

    [HttpPut("updateSalary")]
    public IActionResult UpdateSalary(UserSalary salary)
    {
        var salaryDb = _userRepository.GetUserSalary(salary.UserId);
        if (salaryDb != null)
        {
            salaryDb.Salary = salary.Salary;
            if(_userRepository.SaveChanges()){
                return Ok();
            }
            throw new Exception("Failed to update user salary");
        }
        throw new Exception("Failed to find user salary to update.");
    }

    [HttpPost("addSalary")]
    public IActionResult AddSalary(UserSalary salary)
    {
        var salaryDb = new UserSalary
        {
            UserId = salary.UserId,
            Salary = salary.Salary,
        };

        _userRepository.AddEntity<UserSalary>(salaryDb);
        if (_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to add new user salary");
    }

    [HttpDelete("deleteSalary")]
    public IActionResult DeleteSalary(int UserId)
    {
        var salaryDb = _userRepository.GetUserSalary(UserId);

        if (salaryDb != null)
        {
            _userRepository.RemoveEntity<UserSalary>(salaryDb);
            if(_userRepository.SaveChanges()){
                return Ok();
            }
            throw new Exception("Failed to delete salary");
        }
        throw new Exception("Failed to find user to delete");
    }
}