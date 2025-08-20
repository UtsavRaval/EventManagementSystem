using EventManagement.api.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.EnrollmentNumber).IsUnique();
                entity.Property(u => u.Email).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Name).HasMaxLength(200).IsRequired();
                entity.Property(u => u.Branch).HasMaxLength(100);
                entity.Property(u => u.EnrollmentNumber).HasMaxLength(50);
                entity.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("User");
            });

            // Event entity configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SrNo).IsUnique();
                entity.Property(e => e.EventName).HasMaxLength(200).IsRequired();
                entity.HasOne(e => e.Creator)
                      .WithMany(u => u.CreatedEvents)
                      .HasForeignKey(e => e.CreatedBy);
            });

            // EventRegistration entity configuration
            modelBuilder.Entity<EventRegistration>(entity =>
            {
                entity.HasKey(er => er.Id);
                entity.HasIndex(er => new { er.EventId, er.UserId }).IsUnique();
                entity.HasOne(er => er.Event)
                      .WithMany(e => e.EventRegistrations)
                      .HasForeignKey(er => er.EventId);
                entity.HasOne(er => er.User)
                      .WithMany(u => u.EventRegistrations)
                      .HasForeignKey(er => er.UserId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

