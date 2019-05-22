using System.ComponentModel.DataAnnotations;

namespace GymBooking.v1.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
