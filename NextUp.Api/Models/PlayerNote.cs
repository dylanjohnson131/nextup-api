using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("PlayerNote")]
    public class PlayerNote
    {
        [Key]
        public int PlayerNoteId { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public int CoachId { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsPrivate { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        [ForeignKey("CoachId")]
        public User Coach { get; set; }
    }
}
