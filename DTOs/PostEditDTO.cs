namespace APIOne.DTOs;

public class PostEditDTO
{
    public int PostId { get; set; }
    public string PostTitle { get; set; } = "";
    public string PostContent { get; set; } = "";
}