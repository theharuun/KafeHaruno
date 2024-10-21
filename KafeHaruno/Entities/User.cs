using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{

    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }


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
        public bool? Role { get; set; }  // Whether ,Admin or Waiter/Waitress

        // A user can have multiple orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
