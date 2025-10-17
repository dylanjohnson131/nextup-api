using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("PlayerGoal")]
    public class PlayerGoal
    {
        [Key]
        public int PlayerGoalId { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        [MaxLength(50)]
        public string GoalType { get; set; }
        [Required]
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; } = 0;
        [MaxLength(20)]
        public string Season { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
    }
}
