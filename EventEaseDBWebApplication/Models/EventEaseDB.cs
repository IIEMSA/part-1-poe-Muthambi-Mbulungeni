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
        public virtual DbSet<EventType> EventTypes { get; set; } // New DbSet

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<EventType>().ToTable("EventType");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<Venue>().ToTable("Venue");
            modelBuilder.Entity<Booking>().ToTable("Booking");

            // Existing relationships
            modelBuilder.Entity<Event>()
                .HasRequired(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Venue)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VenueId)
                .WillCascadeOnDelete(false);

            // New EventType relationship
            modelBuilder.Entity<Event>()
                .HasRequired(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeId)
                .WillCascadeOnDelete(false);

            // Unique constraints
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.EventId, b.VenueId, b.BookingDate })
                .IsUnique();

            modelBuilder.Entity<Event>()
                .HasIndex(e => new { e.EventName, e.EventDate })
                .IsUnique();

            modelBuilder.Entity<Venue>()
                .HasIndex(v => new { v.VenueName, v.Location })
                .IsUnique();
        }
    }
}