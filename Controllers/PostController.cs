using APIOne.Data;
using APIOne.DTOs;
using APIOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIOne.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [AllowAnonymous]
    [HttpGet("GetMyPosts/{a}/{b}/{c}")]
    public int GetMyPosts(int a, int b, int c)
    {

        var getPostsSql = $"SELECT * FROM TutorialAppSchema.posts WHERE UserId = '{User.FindFirst("userId")?.Value}'";

        Console.WriteLine(getPostsSql);

        // return _dapper.LoadData<Post>(getPostsSql);

        return a + b + c;
    }

    [HttpPost("Post")]
    public IActionResult CreatePost(PostCreateDTO postCreateDto)
    {
        var addPostSql = @$"INSERT INTO TutorialAppSchema.Posts (
            UserId, PostTitle, PostContent, CreatedAt, UpdatedAt
         ) VALUES (
            '{User.FindFirst("userId")?.Value}', '{postCreateDto.PostTitle}', '{postCreateDto.PostContent}', '{DateTime.Now}', '{DateTime.Now}'
         )";
        if (_dapper.ExecuteSql(addPostSql))
        {
            return Ok("Post created");
        }
        throw new Exception("Could not add new post");
    }

    [HttpPut("Post")]
    public IActionResult EditPost(PostEditDTO postEditDto)
    {
        // Should also confirm that current userId matches post - could add a second where clause (AND WHERE userId = {User.FindFirst("userId")?.Value})
        var editPostSql = @$"UPDATE TutorialAppSchema.posts SET
         PostTitle = '{postEditDto.PostTitle}',
         PostContent = '{postEditDto.PostContent}',
         UpdatedAt = GETDATE()
         WHERE PostId = '{postEditDto.PostId}'
        ";
        Console.WriteLine(editPostSql);
        if (_dapper.ExecuteSql(editPostSql))
        {
            return Ok();
        }
        throw new Exception("Failed to update post");
    }

    [AllowAnonymous]
    [HttpGet("Search/{searchTerm}")]
    public IEnumerable<Post> SearchPosts(string searchTerm)
    {
        var sqlSearch = @$"
        SELECT * FROM TutorialAppSchema.posts WHERE
        PostTitle LIKE '%{searchTerm}%'
        OR PostContent LIKE '%{searchTerm}%'
        ";

        return _dapper.LoadData<Post>(sqlSearch);
    }
}