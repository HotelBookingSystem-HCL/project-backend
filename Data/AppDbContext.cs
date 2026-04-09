using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor - receives options (like connection string) via dependency injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Each DbSet represents a table in the database
        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // OnModelCreating lets us configure table relationships and seed initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Relationships ---

            // A Hotel has many Rooms
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a hotel deletes its rooms

            // A Booking belongs to one User
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // A Booking belongs to one Room
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete room if it has bookings

            // --- Column configurations ---
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Room>()
                .Property(r => r.PricePerNight)
                .HasColumnType("decimal(18,2)");

            // --- Seed Data (Admin user created at startup) ---
            // Password: "Admin@123" (hashed using BCrypt)
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@hotelbooking.com",
                // This is BCrypt hash of "Admin@123"
                PasswordHash = "$2a$11$tGM8C8h5M1wLxBrXqz5JTuKfzDV5JdU8Q8cNk3R2xPQbvUiJlOLQi",
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1),
                IsActive = true
            });
        }
    }
}
