using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymBooking.v1.Models
{
    public class GymClass
    {
        public GymClass()
        {
            AttendingMembers = new HashSet<ApplicationUserGymClass>();
        }

        public int GymClassId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Trainer is required")]
        public string Trainer { get; set; }

        public string Description { get; set; }

        [DisplayName("Starts: Date and time")]
        public DateTime StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        [DisplayName("Ends: Date and time")]
        public DateTime EndTime => StartTime + Duration;

        public virtual ICollection<ApplicationUserGymClass> AttendingMembers { get; set; } 
    }
}
