using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NextUp.Models.Validation;

namespace NextUp.Models
{
    public class User
    {
        // Valid role constants - no generic "User" role allowed
        public const string PLAYER_ROLE = "Player";
        public const string COACH_ROLE = "Coach";
        public const string ATHLETIC_DIRECTOR_ROLE = "AthleticDirector";
        
        public static readonly string[] ValidRoles = { PLAYER_ROLE, COACH_ROLE, ATHLETIC_DIRECTOR_ROLE };
        
        public static bool IsValidRole(string role)
        {
            return Array.Exists(ValidRoles, r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
        [Key]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        [ValidRole]
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

                // Navigation properties
        public Coach Coach { get; set; }
        public AthleticDirector AthleticDirector { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<PlayerGameStats> RecordedStats { get; set; }
        public ICollection<GameNote> GameNotes { get; set; }
        public ICollection<PlayerNote> PlayerNotes { get; set; }

    }
}