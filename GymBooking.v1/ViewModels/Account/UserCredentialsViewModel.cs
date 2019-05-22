using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymBooking.v1.ViewModels.Account
{
    public class UserCredentialsViewModel
    {
        public string Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Firstname is required")]
        public string Name { get; set; }


        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
