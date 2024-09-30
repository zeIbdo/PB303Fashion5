using System.ComponentModel.DataAnnotations;

namespace PB303Fashion.Models;

public class RegisterViewModel
{
    public string Username { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }=null!;

    [DataType(DataType.Password)]
    public string Password { get; set; }=null!;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }=null !;
}

