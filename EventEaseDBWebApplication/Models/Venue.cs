using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace EventEaseDBWebApplication.Models
{
    [Table("Venue")]
    public partial class Venue
    {
        public Venue()
        {
            Bookings = new HashSet<Booking>();
            Events = new HashSet<Event>();
            Capacity = 1;
        }

        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; } = 1;

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true; // New property

        [StringLength(255)]
        public string ImageUrl { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Event> Events { get; set; }

        [NotMapped]
        [DisplayName("Upload Image")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}