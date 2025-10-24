using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("Player")]
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TeamId { get; set; }
        [MaxLength(50)]
        public string Position { get; set; }
        public int? Age { get; set; }
        [MaxLength(10)]
        public string Height { get; set; }
        public int? Weight { get; set; }
        public int? JerseyNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        public ICollection<PlayerGoal> Goals { get; set; }
        public ICollection<PlayerGameStats> GameStats { get; set; }
        public ICollection<PlayerNote> Notes { get; set; }
    }
}
