using EventHubApp.Configurations;
using EventHubApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EventHubApp.Data
{
    public class EventHubDbContext : DbContext
    {
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<OrganizerProfile> OrganizerProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Badge> Badges { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=EventHubDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventConfiguration());

            modelBuilder.Entity<OrganizerProfile>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Organizer)
                      .WithOne(o => o.Profile)
                      .HasForeignKey<OrganizerProfile>(p => p.OrganizerId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Badge>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.BadgeNumber)
                      .IsRequired();

                entity.HasIndex(b => b.BadgeNumber)
                      .IsUnique();

                entity.Property(b => b.IssuedDate)
                      .IsRequired();

                entity.Property(b => b.Tier)
                      .HasConversion<string>()
                      .IsRequired();

                entity.HasOne(b => b.Attendee)
                      .WithOne(a => a.Badge)
                      .HasForeignKey<Badge>(b => b.AttendeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}