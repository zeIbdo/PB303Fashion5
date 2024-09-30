namespace PB303Fashion.Models
{
    public class ForgetPasswordViewModel
    {
        public string Email { get; set; } = null!;
        public string? ResetLink { get; set; }
    }
}
