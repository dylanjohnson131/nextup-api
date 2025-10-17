using Microsoft.EntityFrameworkCore;
using NextUp.Models;
namespace NextUp.Data
{
    public class NextUpDbContext : DbContext
    {
        public NextUpDbContext(DbContextOptions<NextUpDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<AthleticDirector> AthleticDirectors { get; set; }
        public DbSet<PlayerGoal> PlayerGoals { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerGameStats> PlayerGameStats { get; set; }
        public DbSet<GameNote> GameNotes { get; set; }
        public DbSet<PlayerNote> PlayerNotes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Coach)
                .WithMany(u => u.Teams)
                .HasForeignKey(t => t.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.CoachId)
                .HasDatabaseName("idx_team_coach");
            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithMany(u => u.Players)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.UserId)
                .HasDatabaseName("idx_player_user");
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.TeamId)
                .HasDatabaseName("idx_player_team");
            modelBuilder.Entity<Coach>()
                .HasOne(c => c.User)
                .WithOne(u => u.Coach)
                .HasForeignKey<Coach>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Coach>()
                .HasIndex(c => c.UserId)
                .HasDatabaseName("idx_coach_user")
                .IsUnique();
            modelBuilder.Entity<Coach>()
                .HasIndex(c => c.TeamId)
                .HasDatabaseName("idx_coach_team");
            modelBuilder.Entity<AthleticDirector>()
                .HasOne(ad => ad.User)
                .WithOne(u => u.AthleticDirector)
                .HasForeignKey<AthleticDirector>(ad => ad.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AthleticDirector>()
                .HasIndex(ad => ad.UserId)
                .HasDatabaseName("idx_athleticdirector_user")
                .IsUnique();
            modelBuilder.Entity<PlayerGoal>()
                .HasOne(pg => pg.Player)
                .WithMany(p => p.Goals)
                .HasForeignKey(pg => pg.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PlayerGoal>()
                .HasIndex(pg => pg.PlayerId)
                .HasDatabaseName("idx_goal_player");
            modelBuilder.Entity<Game>()
                .HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Game>()
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.GameDate)
                .HasDatabaseName("idx_game_date");
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.HomeTeamId)
                .HasDatabaseName("idx_home_team");
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.AwayTeamId)
                .HasDatabaseName("idx_away_team");
            modelBuilder.Entity<PlayerGameStats>()
                .HasOne(pgs => pgs.Player)
                .WithMany(p => p.GameStats)
                .HasForeignKey(pgs => pgs.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PlayerGameStats>()
                .HasOne(pgs => pgs.Game)
                .WithMany(g => g.PlayerStats)
                .HasForeignKey(pgs => pgs.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PlayerGameStats>()
                .HasOne(pgs => pgs.RecordedByUser)
                .WithMany(u => u.RecordedStats)
                .HasForeignKey(pgs => pgs.RecordedBy)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<PlayerGameStats>()
                .HasIndex(pgs => new { pgs.PlayerId, pgs.GameId })
                .HasDatabaseName("idx_player_game_unique")
                .IsUnique();
            modelBuilder.Entity<PlayerGameStats>()
                .HasIndex(pgs => pgs.GameId)
                .HasDatabaseName("idx_stats_game");
            modelBuilder.Entity<PlayerGameStats>()
                .HasIndex(pgs => pgs.PlayerId)
                .HasDatabaseName("idx_stats_player");
            modelBuilder.Entity<GameNote>()
                .HasOne(gn => gn.Game)
                .WithMany(g => g.GameNotes)
                .HasForeignKey(gn => gn.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GameNote>()
                .HasOne(gn => gn.Coach)
                .WithMany(u => u.GameNotes)
                .HasForeignKey(gn => gn.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GameNote>()
                .HasIndex(gn => gn.GameId)
                .HasDatabaseName("idx_gamenote_game");
            modelBuilder.Entity<GameNote>()
                .HasIndex(gn => gn.CoachId)
                .HasDatabaseName("idx_gamenote_coach");
            modelBuilder.Entity<PlayerNote>()
                .HasOne(pn => pn.Player)
                .WithMany(p => p.Notes)
                .HasForeignKey(pn => pn.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PlayerNote>()
                .HasOne(pn => pn.Coach)
                .WithMany(u => u.PlayerNotes)
                .HasForeignKey(pn => pn.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PlayerNote>()
                .HasIndex(pn => pn.PlayerId)
                .HasDatabaseName("idx_playernote_player");
            modelBuilder.Entity<PlayerNote>()
                .HasIndex(pn => pn.CoachId)
                .HasDatabaseName("idx_playernote_coach");
        }
    }
}
