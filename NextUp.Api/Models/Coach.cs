using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("Coach")]
    public class Coach
    {
        [Key]
        public int CoachId { get; set; }
        [Required]
        public int UserId { get; set; }
        public int? TeamId { get; set; }
        public int ExperienceYears { get; set; } = 0;
        [MaxLength(100)]
        public string Specialty { get; set; }
        [MaxLength(100)]
        public string Certification { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
    }
}
