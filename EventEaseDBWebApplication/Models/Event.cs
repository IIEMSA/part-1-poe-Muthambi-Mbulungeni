using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseDBWebApplication.Models
{
    [Table("Event")]
    public partial class Event
    {
        public Event()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime EventDate { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public int VenueId { get; set; }

        [ForeignKey("VenueId")]
        public virtual Venue Venue { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}