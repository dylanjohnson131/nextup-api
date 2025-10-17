using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("AthleticDirector")]
    public class AthleticDirector
    {
        [Key]
        public int AthleticDirectorId { get; set; }
        [Required]
        public int UserId { get; set; }
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;
        public int ExperienceYears { get; set; } = 0;
        [MaxLength(100)]
        public string Certification { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Institution { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
