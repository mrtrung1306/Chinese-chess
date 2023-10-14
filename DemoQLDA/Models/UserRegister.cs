using System.ComponentModel.DataAnnotations;

namespace DemoQLDA.Models
{
    public class UserRegister
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
