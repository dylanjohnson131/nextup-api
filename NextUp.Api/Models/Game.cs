using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextUp.Models
{
    [Table("Game")]
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        [Required]
        public int HomeTeamId { get; set; }

        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        public DateTime GameDate { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        public int HomeScore { get; set; } = 0;
        public int AwayScore { get; set; } = 0;

        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled

        [MaxLength(20)]
        public string Season { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("HomeTeamId")]
        public Team HomeTeam { get; set; }

        [ForeignKey("AwayTeamId")]
        public Team AwayTeam { get; set; }

        public ICollection<PlayerGameStats> PlayerStats { get; set; }
        public ICollection<GameNote> GameNotes { get; set; }
    }
}
