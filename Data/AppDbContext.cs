using Microsoft.EntityFrameworkCore;
using HotelBooking.Models;

namespace HotelBooking.Data;

public class AppDbContext : DbContext
{
    // Constructor (used for dependency injection)
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Tables
    public DbSet<User> Users { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}