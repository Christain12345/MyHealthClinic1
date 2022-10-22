using System;
using System.ComponentModel.DataAnnotations;

namespace MyHealthClinic.Models
{
    public class AvailableTime
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ApplicationUser GeneralPractioner { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsBooked { get; set; } = false;
    }
}
