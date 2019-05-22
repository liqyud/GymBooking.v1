using System;

namespace GymBooking.v1.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Trainger { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime => StartTime + Duration;
        public string Description { get; set; }

        public bool IsBooked { get; set; }
        public string Message { get; set; }
    }
}
