using System.Collections.Generic;
using GymBooking.v1.Models;
using Microsoft.AspNetCore.Identity;

namespace GymBooking.v1.ModelsIdentity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            GymClasses = new HashSet<ApplicationUserGymClass>();
        }

        public string Name { get; set; }
        public virtual ICollection<ApplicationUserGymClass> GymClasses { get; set; }
    }
}
