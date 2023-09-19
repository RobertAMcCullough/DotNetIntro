namespace APIOne.DTOs;

public partial class UserConfirmLogin
{
    public byte[] Hash { get; set; } = new byte[0]; // byte[] maps to VARBINARY in DB
    public byte[] Salt { get; set; } = new byte[0];
}