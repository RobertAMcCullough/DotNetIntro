using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIOne.Models;

public partial class UserJobInfo
{
    public int UserId { get; set; }
    public string JobTitle { get; set; } = "";
    public string Department { get; set; } = "";
}