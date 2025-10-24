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

        public int Completions { get; set; } = 0;
        public int Touchdowns { get; set; } = 0;
        public int? PassingAttempts { get; set; }
        public double? CompletionPercentage { get; set; }
        public double? YardsPerPassAttempt { get; set; }
        public int? LongestPass { get; set; }
        public int? Sacked { get; set; }
        public int? Penalties { get; set; }
        public int? RushingAttempts { get; set; }
        public double? YardsPerRushAttempt { get; set; }
        public int? RushingTDs { get; set; }
        public int? LongestRushing { get; set; }
        public int? Fumbles { get; set; }
        public int? Targets { get; set; }
        public double? YardsPerReception { get; set; }
        public int? ReceivingTDs { get; set; }
        public int? LongestReception { get; set; }
        public int? SacksAllowed { get; set; }
        public int? PancakeBlocks { get; set; }
        public int? SnapsPlayed { get; set; }
        public int? CleanSnaps { get; set; }
        public int? TotalSnaps { get; set; }
        public double? SnapAccuracy { get; set; }
        public int? TacklesForLoss { get; set; }
        public int? Pressures { get; set; }
        public int? TotalTackles { get; set; }
        public int? PassBreakups { get; set; }
        public int? InterceptionReturnYards { get; set; }
        public int? InterceptionReturnTouchDown { get; set; }
        public double? YardsPerPunt { get; set; }
        public int? Touchbacks { get; set; }
        public int? FieldGoalMade { get; set; }
        public int? FieldGoalAttempts { get; set; }
        public int? LongestFieldGoal { get; set; }
        public int? BlockedKicks { get; set; }
        [Required]
        public string Position { get; set; }
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
