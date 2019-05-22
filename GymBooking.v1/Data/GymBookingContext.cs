using GymBooking.v1.Models;
using GymBooking.v1.ModelsIdentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.v1.Data
{
    public class GymBookingContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClass { get; set; }

        public GymBookingContext(DbContextOptions<GymBookingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(memberGymClass => new {memberGymClass.ApplicationUserId, memberGymClass.GymClassId});
        }
    }
}
