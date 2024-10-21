using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is Required!!!")]
        [MaxLength(100, ErrorMessage = "Max lenght 100")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is Required!!!")]
        [DataType(DataType.Password)]
        [MaxLength(20, ErrorMessage = "Max lenght 20")]
        public string Password { get; set; }

        public bool? Role { get; set; }
    }
}
