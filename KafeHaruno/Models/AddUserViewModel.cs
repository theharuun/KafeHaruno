using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Models
{
   
        [Index(nameof(Username), IsUnique = true)]
        public class AddUserViewModel
        {

            [Required(ErrorMessage = "Name is Required!!!")]
            [MaxLength(100, ErrorMessage = "Max lenght 100")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Surname is Required!!!")]
            [MaxLength(100, ErrorMessage = "Max lenght 100")]
            public string Surname { get; set; }


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
