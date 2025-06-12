using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.DTOs
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
