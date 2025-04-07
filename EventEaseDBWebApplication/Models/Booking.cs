namespace EventEaseDBWebApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Booking")]
    public partial class Booking
    {
        public int BookingId { get; set; }

        public int? EventId { get; set; }

        public int? VenueId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BookingDate { get; set; }

        public virtual Event Event { get; set; }

        public virtual Venue Venue { get; set; }
    }
}
