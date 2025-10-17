using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NextUp.Models
{
    [Table("GameNote")]
    public class GameNote
    {
        [Key]
        public int GameNoteId { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public int CoachId { get; set; }
        [MaxLength(20)]
        public string NoteType { get; set; } = "Text";
        public string Content { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        [ForeignKey("CoachId")]
        public User Coach { get; set; }
    }
}
