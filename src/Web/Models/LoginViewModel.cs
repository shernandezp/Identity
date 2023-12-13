namespace Security.Web.Models;

public sealed class LoginViewModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ReturnUrl { get; set; }
}
