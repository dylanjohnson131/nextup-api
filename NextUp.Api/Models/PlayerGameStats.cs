using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("PlayerGameStats")]
    public class PlayerGameStats
    {
        [Key]
        public int PlayerGameStatsId { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public int GameId { get; set; }
        public int PassingYards { get; set; } = 0;
        public int PassingTouchdowns { get; set; } = 0;
        public int Interceptions { get; set; } = 0;
        public int RushingYards { get; set; } = 0;
        public int RushingTouchdowns { get; set; } = 0;
        public int ReceivingYards { get; set; } = 0;
        public int ReceivingTouchdowns { get; set; } = 0;
        public int Receptions { get; set; } = 0;
        public int Tackles { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int Sacks { get; set; } = 0;
        public int ForcedFumbles { get; set; } = 0;
        public int InterceptionsDef { get; set; } = 0;
        public int FieldGoalsMade { get; set; } = 0;
        public int FieldGoalsAttempted { get; set; } = 0;
        public int ExtraPointsMade { get; set; } = 0;
        public int ExtraPointsAttempted { get; set; } = 0;
        public int MinutesPlayed { get; set; } = 0;
        public int? RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        [ForeignKey("RecordedBy")]
        public User RecordedByUser { get; set; }
    }
}
