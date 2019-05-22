using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymBooking.v1.ViewModels.Account
{
    public class RegisterViewModel
    {
        [DisplayName("Firstname")]
        [Required(ErrorMessage = "Firstname is required")]
        public string Firstname { get; set; }

        [DisplayName("Lastname")]
        [Required(ErrorMessage = "Lastname is required")]
        public string Lastname { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
