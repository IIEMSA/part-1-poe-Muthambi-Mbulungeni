using System.Data.Entity;

namespace EventEaseDBWebApplication.Models
{
    public partial class EventEaseDB : DbContext
    {
        public EventEaseDB()
            : base("name=EventEaseDB")
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Venue> Venues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure relationships with cascade delete
            modelBuilder.Entity<Event>()
                .HasRequired(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .WillCascadeOnDelete(false); // Changed to NO ACTION

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .WillCascadeOnDelete(true); // CASCADE when Event is deleted

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Venue)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VenueId)
                .WillCascadeOnDelete(false); // NO ACTION when Venue is deleted

            // Unique constraint
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.EventId, b.VenueId, b.BookingDate })
                .IsUnique();
        }
    }
}