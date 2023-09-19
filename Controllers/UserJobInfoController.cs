// EXAMPLE USING ENTITY FRAMEWORK (but not user repository)
using APIOne.Data;
using APIOne.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APIOne.Controllers;

[ApiController]
[Route("[controller]")]
public class UserJobInfoController : ControllerBase
{
    DataContextEF _EF;

    IMapper _mapper;

    public UserJobInfoController(IConfiguration config)
    {
        _EF = new DataContextEF(config);

        _mapper = new Mapper(new MapperConfiguration((cfg) =>
        {
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
            // Or this?
            // cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
        }));
    }

    [HttpGet("jobInfo")]
    public IEnumerable<UserJobInfo>? GetUserJobInfo()
    {
        return _EF.UserJobInfo?.ToList<UserJobInfo>();
    }

    [HttpGet("jobInfo/{userId}")]
    public UserJobInfo? GetUserJobInfoByUserId(int userId)
    {
        var info = _EF.UserJobInfo?.FirstOrDefault<UserJobInfo>(u => u.UserId == userId);
        if (info != null)
        {
            return info;
        }
        throw new Exception("Failed to find user");

        // Another way to do the same thing:
        // return _EF.UserJobInfo?.Where(u => u.UserId == userId).FirstOrDefault<UserJobInfo>();
    }

    [HttpPut("updateJobInfo")]
    public IActionResult UpdateJobInfo(UserJobInfo info)
    {
        var infoDb = _EF.UserJobInfo?.FirstOrDefault(u => u.UserId == info.UserId);
        if (infoDb != null)
        {

            _mapper.Map(info, infoDb);

            // Could also just do this instead of mapper
            // infoDb.Department = info.Department;
            // infoDb.JobTitle = info.JobTitle;

            if (_EF.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to update user job info");
        }
        throw new Exception("Failed to find user job info to update.");
    }

    [HttpPost("addJobInfo")]
    public IActionResult AddJobInfo(UserJobInfo info)
    {
        var infoDb = new UserJobInfo
        {
            UserId = info.UserId,
            Department = info.Department,
            JobTitle = info.JobTitle
        };

        _EF.Add(infoDb);
        if (_EF.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Failed to add new user job info");
    }

    [HttpDelete("deleteJobInfo")]
    public IActionResult DeleteJobInfo(int UserId)
    {
        var infoDb = _EF.UserJobInfo?.FirstOrDefault<UserJobInfo>(u => u.UserId == UserId);

        if (infoDb != null)
        {
            _EF.Remove(infoDb);
            if (_EF.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }
        throw new Exception("Failed to find user to delete");
    }
}