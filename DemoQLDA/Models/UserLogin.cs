using System.ComponentModel.DataAnnotations;

namespace DemoQLDA.Models
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
