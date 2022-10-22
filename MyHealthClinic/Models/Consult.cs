using System.ComponentModel.DataAnnotations;

namespace MyHealthClinic.Models
{
    public class Consult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ApplicationUser Patient { get; set; }

        [Required]
        public AvailableTime AvailableTime { get; set; }

        public string Comment { get; set; }
    }
}