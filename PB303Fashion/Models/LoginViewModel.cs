using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace PB303Fashion.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public string? ReturnUrl { get; set; }
        public IEnumerable<AuthenticationScheme>? Schemes {  get; set; } 
    }
}
