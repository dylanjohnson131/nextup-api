using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Data;
using NextUp.Models;
namespace NextUp.Api.Endpoints
{
    public static class StatsEndpoints
    {
        // Hardcoded mapping of position to relevant stat fields
        private static readonly Dictionary<string, string[]> PositionStatsMap = new()
        {
            ["QB"] = new[] { "Completions", "PassingAttempts", "CompletionPercentage", "YardsPerPassAttempt", "Touchdowns", "Interceptions", "LongestPass", "Sacked", "RushingYards", "Penalties" },
            ["RB"] = new[] { "RushingAttempts", "RushingYards", "YardsPerRushAttempt", "RushingTDs", "Receptions", "LongestRushing", "ReceivingYards", "Fumbles", "Penalties" },
            ["WR"] = new[] { "Receptions", "Targets", "YardsPerReception", "ReceivingYards", "ReceivingTDs", "LongestReception", "Fumbles", "Penalties" },
            ["TE"] = new[] { "Receptions", "ReceivingYards", "ReceivingTDs", "Targets", "YardsPerReception", "LongestReception", "Fumbles", "Penalties" },
            ["LT"] = new[] { "SacksAllowed", "PancakeBlocks", "SnapsPlayed", "Penalties" },
            ["RT"] = new[] { "SacksAllowed", "PancakeBlocks", "SnapsPlayed", "Penalties" },
            ["C"] = new[] { "CleanSnaps", "TotalSnaps", "SnapAccuracy", "PancakeBlocks", "SacksAllowed", "SnapsPlayed", "Penalties" },
            ["LG"] = new[] { "PancakeBlocks", "SacksAllowed", "SnapsPlayed", "Penalties" },
            ["RG"] = new[] { "PancakeBlocks", "SacksAllowed", "SnapsPlayed", "Penalties" },
            ["DE"] = new[] { "Sacks", "TacklesForLoss", "Pressures", "TotalTackles", "ForcedFumbles", "Penalties" },
            ["LDT"] = new[] { "Sacks", "TacklesForLoss", "Pressures", "TotalTackles", "ForcedFumbles", "Penalties" },
            ["RDT"] = new[] { "Sacks", "TacklesForLoss", "Pressures", "TotalTackles", "ForcedFumbles", "Penalties" },
            ["WLB"] = new[] { "Tackles", "TacklesForLoss", "Sacks", "Interceptions", "Penalties" },
            ["MLB"] = new[] { "Tackles", "TacklesForLoss", "Sacks", "Interceptions", "Penalties" },
            ["SLB"] = new[] { "Tackles", "TacklesForLoss", "Sacks", "Interceptions", "Penalties" },
            ["CB"] = new[] { "Tackles", "Interceptions", "PassBreakups", "ForcedFumbles", "InterceptionReturnYards", "InterceptionReturnTouchDown", "Penalties" },
            ["S"] = new[] { "Tackles", "Interceptions", "PassBreakups", "ForcedFumbles", "InterceptionReturnYards", "InterceptionReturnTouchDown", "Penalties" },
            ["P"] = new[] { "YardsPerPunt", "Touchbacks" },
            ["K"] = new[] { "FieldGoalMade", "FieldGoalAttempts", "LongestFieldGoal", "BlockedKicks" }
        };
        public static void MapStatsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/stats");
            group.MapGet("/player/{playerId:int}", async (int playerId, NextUpDbContext db) =>
            {
                var stats = await db.PlayerGameStats
                    .Where(s => s.PlayerId == playerId)
                    .Include(s => s.Game)
                    .ToListAsync();
                return Results.Ok(stats.Select(s => {
                    string NormalizePosition(string pos)
                    {
                        return pos switch
                        {
                            "Quarterback" => "QB",
                            "Running Back" => "RB",
                            "Wide Receiver" => "WR",
                            "Tight End" => "TE",
                            "Left Tackle" => "LT",
                            "Right Tackle" => "RT",
                            "Center" => "C",
                            "Left Guard" => "LG",
                            "Right Guard" => "RG",
                            "Defensive End" => "DE",
                            "Left Defensive Tackle" => "LDT",
                            "Right Defensive Tackle" => "RDT",
                            "Weakside Linebacker" => "WLB",
                            "Middle Linebacker" => "MLB",
                            "Strongside Linebacker" => "SLB",
                            "Cornerback" => "CB",
                            "Safety" => "S",
                            "Punter" => "P",
                            "Kicker" => "K",
                            _ => pos
                        };
                    }
                    var pos = NormalizePosition(s.Position ?? "");
                    var statFields = PositionStatsMap.ContainsKey(pos) ? PositionStatsMap[pos] : Array.Empty<string>();
                    var statDict = new Dictionary<string, object?>
                    {
                        ["PlayerGameStatsId"] = s.PlayerGameStatsId,
                        ["GameId"] = s.GameId,
                        ["Game"] = new { s.Game.GameDate, s.Game.Location },
                        ["CreatedAt"] = s.CreatedAt,
                        ["UpdatedAt"] = s.UpdatedAt,
                        ["Position"] = s.Position
                    };
                    foreach (var field in statFields)
                    {
                        var prop = typeof(PlayerGameStats).GetProperty(field);
                        if (prop != null)
                        {
                            statDict[field] = prop.GetValue(s);
                        }
                    }
                    return statDict;
                }));
            });

            // New endpoint: Get stats for a specific player and game
            group.MapGet("/game/{gameId:int}/player/{playerId:int}", async (int gameId, int playerId, NextUpDbContext db) =>
            {
                var s = await db.PlayerGameStats
                    .Where(x => x.PlayerId == playerId && x.GameId == gameId)
                    .Include(x => x.Game)
                    .FirstOrDefaultAsync();
                if (s == null)
                    return Results.NotFound();
                string NormalizePosition(string pos)
                {
                    return pos switch
                    {
                        "Quarterback" => "QB",
                        "Running Back" => "RB",
                        "Wide Receiver" => "WR",
                        "Tight End" => "TE",
                        "Left Tackle" => "LT",
                        "Right Tackle" => "RT",
                        "Center" => "C",
                        "Left Guard" => "LG",
                        "Right Guard" => "RG",
                        "Defensive End" => "DE",
                        "Left Defensive Tackle" => "LDT",
                        "Right Defensive Tackle" => "RDT",
                        "Weakside Linebacker" => "WLB",
                        "Middle Linebacker" => "MLB",
                        "Strongside Linebacker" => "SLB",
                        "Cornerback" => "CB",
                        "Safety" => "S",
                        "Punter" => "P",
                        "Kicker" => "K",
                        _ => pos
                    };
                }
                var pos = NormalizePosition(s.Position ?? "");
                var statFields = PositionStatsMap.ContainsKey(pos) ? PositionStatsMap[pos] : Array.Empty<string>();
                var statDict = new Dictionary<string, object?>
                {
                    ["PlayerGameStatsId"] = s.PlayerGameStatsId,
                    ["GameId"] = s.GameId,
                    ["Game"] = new { s.Game.GameDate, s.Game.Location },
                    ["CreatedAt"] = s.CreatedAt,
                    ["UpdatedAt"] = s.UpdatedAt,
                    ["Position"] = s.Position
                };
                foreach (var field in statFields)
                {
                    var prop = typeof(PlayerGameStats).GetProperty(field);
                    if (prop != null)
                    {
                        statDict[field] = prop.GetValue(s);
                    }
                }
                return Results.Ok(statDict);
            });
                group.MapPost("/game/{gameId:int}/player/{playerId:int}", async (int gameId, int playerId, UpdatePlayerStatsRequest request, NextUpDbContext db) =>
                {
                    var player = await db.Players.FindAsync(playerId);
                    var game = await db.Games.FindAsync(gameId);
                    if (player == null || game == null)
                    {
                        Console.WriteLine($"[StatsEndpoints] Player or Game not found. PlayerId: {playerId}, GameId: {gameId}");
                        return Results.BadRequest(new { error = "Player or Game not found." });
                    }

                    // Check if stats already exist for this player/game
                    var stats = await db.PlayerGameStats.FirstOrDefaultAsync(s => s.PlayerId == playerId && s.GameId == gameId);
                    if (stats == null)
                    {
                        Console.WriteLine($"[StatsEndpoints] Creating new stats for PlayerId: {playerId}, GameId: {gameId}");
                        Console.WriteLine($"[StatsEndpoints] Incoming request: {System.Text.Json.JsonSerializer.Serialize(request)}");
                        // Create new stats
                        stats = new PlayerGameStats
                        {
                            PlayerId = playerId,
                            GameId = gameId,
                            Position = player.Position ?? "",
                            PassingYards = request.PassingYards ?? 0,
                            PassingTouchdowns = request.PassingTouchdowns ?? 0,
                            Interceptions = request.Interceptions ?? 0,
                            RushingYards = request.RushingYards ?? 0,
                            RushingTouchdowns = request.RushingTouchdowns ?? 0,
                            ReceivingYards = request.ReceivingYards ?? 0,
                            ReceivingTouchdowns = request.ReceivingTouchdowns ?? 0,
                            Receptions = request.Receptions ?? 0,
                            Tackles = request.Tackles ?? 0,
                            Assists = request.Assists ?? 0,
                            Sacks = request.Sacks ?? 0,
                            ForcedFumbles = request.ForcedFumbles ?? 0,
                            InterceptionsDef = request.InterceptionsDef ?? 0,
                            FieldGoalsMade = request.FieldGoalsMade ?? 0,
                            FieldGoalsAttempted = request.FieldGoalsAttempted ?? 0,
                            ExtraPointsMade = request.ExtraPointsMade ?? 0,
                            ExtraPointsAttempted = request.ExtraPointsAttempted ?? 0,
                            MinutesPlayed = request.MinutesPlayed ?? 0,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            PassingAttempts = request.PassingAttempts ?? 0,
                            YardsPerPassAttempt = request.YardsPerPassAttempt ?? 0,
                            LongestPass = request.LongestPass ?? 0,
                            Sacked = request.Sacked ?? 0,
                            Penalties = request.Penalties ?? 0,
                            RushingAttempts = request.RushingAttempts ?? 0,
                            YardsPerRushAttempt = request.YardsPerRushAttempt ?? 0,
                            RushingTDs = request.RushingTDs ?? 0,
                            LongestRushing = request.LongestRushing ?? 0,
                            Fumbles = request.Fumbles ?? 0,
                            Targets = request.Targets ?? 0,
                            YardsPerReception = request.YardsPerReception ?? 0,
                            ReceivingTDs = request.ReceivingTDs ?? 0,
                            LongestReception = request.LongestReception ?? 0,
                            SacksAllowed = request.SacksAllowed ?? 0,
                            PancakeBlocks = request.PancakeBlocks ?? 0,
                            SnapsPlayed = request.SnapsPlayed ?? 0,
                            CleanSnaps = request.CleanSnaps ?? 0,
                            TotalSnaps = request.TotalSnaps ?? 0,
                            SnapAccuracy = request.SnapAccuracy ?? 0,
                            TacklesForLoss = request.TacklesForLoss ?? 0,
                            Pressures = request.Pressures ?? 0,
                            TotalTackles = request.TotalTackles ?? 0,
                            PassBreakups = request.PassBreakups ?? 0,
                            InterceptionReturnYards = request.InterceptionReturnYards ?? 0,
                            InterceptionReturnTouchDown = request.InterceptionReturnTouchDown ?? 0,
                            YardsPerPunt = request.YardsPerPunt ?? 0,
                            Touchbacks = request.Touchbacks ?? 0,
                            FieldGoalMade = request.FieldGoalMade ?? 0,
                            FieldGoalAttempts = request.FieldGoalAttempts ?? 0,
                            LongestFieldGoal = request.LongestFieldGoal ?? 0,
                            BlockedKicks = request.BlockedKicks ?? 0
                        };
                        // Calculate completion percentage
                        if (stats.PassingAttempts > 0)
                            stats.CompletionPercentage = (double)stats.Completions / stats.PassingAttempts * 100;
                        else
                            stats.CompletionPercentage = 0;
                        db.PlayerGameStats.Add(stats);
                    }
                    else
                    {
                        Console.WriteLine($"[StatsEndpoints] Updating stats for PlayerId: {playerId}, GameId: {gameId}");
                        Console.WriteLine($"[StatsEndpoints] Incoming request: {System.Text.Json.JsonSerializer.Serialize(request)}");
                        // Update existing stats
                        stats.Position = player.Position ?? stats.Position;
                        stats.PassingYards = request.PassingYards ?? stats.PassingYards;
                        stats.PassingTouchdowns = request.PassingTouchdowns ?? stats.PassingTouchdowns;
                        stats.Touchdowns = request.Touchdowns ?? stats.Touchdowns;
                        stats.Completions = request.Completions ?? stats.Completions;
                        stats.Interceptions = request.Interceptions ?? stats.Interceptions;
                        stats.RushingYards = request.RushingYards ?? stats.RushingYards;
                        stats.RushingTouchdowns = request.RushingTouchdowns ?? stats.RushingTouchdowns;
                        stats.ReceivingYards = request.ReceivingYards ?? stats.ReceivingYards;
                        stats.ReceivingTouchdowns = request.ReceivingTouchdowns ?? stats.ReceivingTouchdowns;
                        stats.Receptions = request.Receptions ?? stats.Receptions;
                        stats.Tackles = request.Tackles ?? stats.Tackles;
                        stats.Assists = request.Assists ?? stats.Assists;
                        stats.Sacks = request.Sacks ?? stats.Sacks;
                        stats.ForcedFumbles = request.ForcedFumbles ?? stats.ForcedFumbles;
                        stats.InterceptionsDef = request.InterceptionsDef ?? stats.InterceptionsDef;
                        stats.FieldGoalsMade = request.FieldGoalsMade ?? stats.FieldGoalsMade;
                        stats.FieldGoalsAttempted = request.FieldGoalsAttempted ?? stats.FieldGoalsAttempted;
                        stats.ExtraPointsMade = request.ExtraPointsMade ?? stats.ExtraPointsMade;
                        stats.ExtraPointsAttempted = request.ExtraPointsAttempted ?? stats.ExtraPointsAttempted;
                        stats.MinutesPlayed = request.MinutesPlayed ?? stats.MinutesPlayed;
                        stats.UpdatedAt = DateTime.UtcNow;
                        stats.PassingAttempts = request.PassingAttempts ?? stats.PassingAttempts;
                        stats.YardsPerPassAttempt = request.YardsPerPassAttempt ?? stats.YardsPerPassAttempt;
                        stats.LongestPass = request.LongestPass ?? stats.LongestPass;
                        stats.Sacked = request.Sacked ?? stats.Sacked;
                        stats.Penalties = request.Penalties ?? stats.Penalties;
                        stats.RushingAttempts = request.RushingAttempts ?? stats.RushingAttempts;
                        stats.YardsPerRushAttempt = request.YardsPerRushAttempt ?? stats.YardsPerRushAttempt;
                        stats.RushingTDs = request.RushingTDs ?? stats.RushingTDs;
                        stats.LongestRushing = request.LongestRushing ?? stats.LongestRushing;
                        stats.Fumbles = request.Fumbles ?? stats.Fumbles;
                        stats.Targets = request.Targets ?? stats.Targets;
                        stats.YardsPerReception = request.YardsPerReception ?? stats.YardsPerReception;
                        stats.ReceivingTDs = request.ReceivingTDs ?? stats.ReceivingTDs;
                        stats.LongestReception = request.LongestReception ?? stats.LongestReception;
                        stats.SacksAllowed = request.SacksAllowed ?? stats.SacksAllowed;
                        stats.PancakeBlocks = request.PancakeBlocks ?? stats.PancakeBlocks;
                        stats.SnapsPlayed = request.SnapsPlayed ?? stats.SnapsPlayed;
                        stats.CleanSnaps = request.CleanSnaps ?? stats.CleanSnaps;
                        stats.TotalSnaps = request.TotalSnaps ?? stats.TotalSnaps;
                        stats.SnapAccuracy = request.SnapAccuracy ?? stats.SnapAccuracy;
                        stats.TacklesForLoss = request.TacklesForLoss ?? stats.TacklesForLoss;
                        stats.Pressures = request.Pressures ?? stats.Pressures;
                        stats.TotalTackles = request.TotalTackles ?? stats.TotalTackles;
                        stats.PassBreakups = request.PassBreakups ?? stats.PassBreakups;
                        stats.InterceptionReturnYards = request.InterceptionReturnYards ?? stats.InterceptionReturnYards;
                        stats.InterceptionReturnTouchDown = request.InterceptionReturnTouchDown ?? stats.InterceptionReturnTouchDown;
                        stats.YardsPerPunt = request.YardsPerPunt ?? stats.YardsPerPunt;
                        stats.Touchbacks = request.Touchbacks ?? stats.Touchbacks;
                        stats.FieldGoalMade = request.FieldGoalMade ?? stats.FieldGoalMade;
                        stats.FieldGoalAttempts = request.FieldGoalAttempts ?? stats.FieldGoalAttempts;
                        stats.LongestFieldGoal = request.LongestFieldGoal ?? stats.LongestFieldGoal;
                        stats.BlockedKicks = request.BlockedKicks ?? stats.BlockedKicks;
                        // Calculate completion percentage
                        if (stats.PassingAttempts > 0)
                            stats.CompletionPercentage = (double)stats.Completions / stats.PassingAttempts * 100;
                        else
                            stats.CompletionPercentage = 0;
                    }
                    await db.SaveChangesAsync();
                    return Results.Ok(new { message = "Stats saved", stats.PlayerGameStatsId });
                }).RequireAuthorization("CoachOnly");

                group.MapGet("/", async (NextUpDbContext db) =>
                {
                    var stats = await db.PlayerGameStats
                        .Include(s => s.Player).ThenInclude(p => p.User)
                        .Include(s => s.Game)
                        .ToListAsync();
                    return Results.Ok(stats.Select(s => new
                    {
                        s.PlayerGameStatsId,
                        Player = new { s.PlayerId, Name = s.Player != null ? $"{s.Player.User?.FirstName} {s.Player.User?.LastName}" : null },
                        Game = new { s.GameId, s.Game.GameDate, s.Game.Location },
                        s.PassingYards,
                        s.PassingTouchdowns,
                        s.Interceptions,
                        s.RushingYards,
                        s.RushingTouchdowns,
                        s.ReceivingYards,
                        s.ReceivingTouchdowns,
                        s.Receptions,
                        s.Tackles,
                        s.Assists,
                        s.Sacks,
                        s.ForcedFumbles,
                        s.InterceptionsDef,
                        s.FieldGoalsMade,
                        s.FieldGoalsAttempted,
                        s.ExtraPointsMade,
                        s.ExtraPointsAttempted,
                        s.MinutesPlayed
                            , s.PassingAttempts
                            , s.CompletionPercentage
                            , s.YardsPerPassAttempt
                            , s.LongestPass
                            , s.Sacked
                            , s.Penalties
                            , s.RushingAttempts
                            , s.YardsPerRushAttempt
                            , s.RushingTDs
                            , s.LongestRushing
                            , s.Fumbles
                            , s.Targets
                            , s.YardsPerReception
                            , s.ReceivingTDs
                            , s.LongestReception
                            , s.SacksAllowed
                            , s.PancakeBlocks
                            , s.SnapsPlayed
                            , s.CleanSnaps
                            , s.TotalSnaps
                            , s.SnapAccuracy
                            , s.TacklesForLoss
                            , s.Pressures
                            , s.TotalTackles
                            , s.PassBreakups
                            , s.InterceptionReturnYards
                            , s.InterceptionReturnTouchDown
                            , s.YardsPerPunt
                            , s.Touchbacks
                            , s.FieldGoalMade
                            , s.FieldGoalAttempts
                            , s.LongestFieldGoal
                            , s.BlockedKicks
                    }));
                });

                group.MapPost("/", async (CreatePlayerStatsRequest request, NextUpDbContext db) =>
                {
                    var player = await db.Players.FindAsync(request.PlayerId);
                    var game = await db.Games.FindAsync(request.GameId);
                    if (player == null || game == null)
                        return Results.BadRequest(new { error = "Player or Game not found." });
                    var stats = new PlayerGameStats
                    {
                        PlayerId = request.PlayerId,
                        GameId = request.GameId,
                        PassingYards = request.PassingYards,
                        PassingTouchdowns = request.PassingTouchdowns,
                        Interceptions = request.Interceptions,
                        RushingYards = request.RushingYards,
                        RushingTouchdowns = request.RushingTouchdowns,
                        ReceivingYards = request.ReceivingYards,
                        ReceivingTouchdowns = request.ReceivingTouchdowns,
                        Receptions = request.Receptions,
                        Tackles = request.Tackles,
                        Assists = request.Assists,
                        Sacks = request.Sacks,
                        ForcedFumbles = request.ForcedFumbles,
                        InterceptionsDef = request.InterceptionsDef,
                        FieldGoalsMade = request.FieldGoalsMade,
                        FieldGoalsAttempted = request.FieldGoalsAttempted,
                        ExtraPointsMade = request.ExtraPointsMade,
                        ExtraPointsAttempted = request.ExtraPointsAttempted,
                        MinutesPlayed = request.MinutesPlayed,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                            , PassingAttempts = request.PassingAttempts
                            , CompletionPercentage = request.CompletionPercentage
                            , YardsPerPassAttempt = request.YardsPerPassAttempt
                            , LongestPass = request.LongestPass
                            , Sacked = request.Sacked
                            , Penalties = request.Penalties
                            , RushingAttempts = request.RushingAttempts
                            , YardsPerRushAttempt = request.YardsPerRushAttempt
                            , RushingTDs = request.RushingTDs
                            , LongestRushing = request.LongestRushing
                            , Fumbles = request.Fumbles
                            , Targets = request.Targets
                            , YardsPerReception = request.YardsPerReception
                            , ReceivingTDs = request.ReceivingTDs
                            , LongestReception = request.LongestReception
                            , SacksAllowed = request.SacksAllowed
                            , PancakeBlocks = request.PancakeBlocks
                            , SnapsPlayed = request.SnapsPlayed
                            , CleanSnaps = request.CleanSnaps
                            , TotalSnaps = request.TotalSnaps
                            , SnapAccuracy = request.SnapAccuracy
                            , TacklesForLoss = request.TacklesForLoss
                            , Pressures = request.Pressures
                            , TotalTackles = request.TotalTackles
                            , PassBreakups = request.PassBreakups
                            , InterceptionReturnYards = request.InterceptionReturnYards
                            , InterceptionReturnTouchDown = request.InterceptionReturnTouchDown
                            , YardsPerPunt = request.YardsPerPunt
                            , Touchbacks = request.Touchbacks
                            , FieldGoalMade = request.FieldGoalMade
                            , FieldGoalAttempts = request.FieldGoalAttempts
                            , LongestFieldGoal = request.LongestFieldGoal
                            , BlockedKicks = request.BlockedKicks
                    };
                    db.PlayerGameStats.Add(stats);
                    await db.SaveChangesAsync();
                    return Results.Created($"/api/stats/{stats.PlayerGameStatsId}", new { stats.PlayerGameStatsId, stats.PlayerId, stats.GameId });
                }).RequireAuthorization("CoachOnly");

                group.MapPut("/{id:int}", async (int id, UpdatePlayerStatsRequest request, NextUpDbContext db) =>
                {
                    var s = await db.PlayerGameStats.FindAsync(id);
                    if (s == null)
                        return Results.NotFound(new { error = $"Stats entry with ID {id} not found." });
                    s.PassingYards = request.PassingYards ?? s.PassingYards;
                    s.PassingTouchdowns = request.PassingTouchdowns ?? s.PassingTouchdowns;
                    s.Interceptions = request.Interceptions ?? s.Interceptions;
                    s.RushingYards = request.RushingYards ?? s.RushingYards;
                    s.RushingTouchdowns = request.RushingTouchdowns ?? s.RushingTouchdowns;
                    s.ReceivingYards = request.ReceivingYards ?? s.ReceivingYards;
                    s.ReceivingTouchdowns = request.ReceivingTouchdowns ?? s.ReceivingTouchdowns;
                    s.Receptions = request.Receptions ?? s.Receptions;
                    s.Tackles = request.Tackles ?? s.Tackles;
                    s.Assists = request.Assists ?? s.Assists;
                    s.Sacks = request.Sacks ?? s.Sacks;
                    s.ForcedFumbles = request.ForcedFumbles ?? s.ForcedFumbles;
                    s.InterceptionsDef = request.InterceptionsDef ?? s.InterceptionsDef;
                    s.FieldGoalsMade = request.FieldGoalsMade ?? s.FieldGoalsMade;
                    s.FieldGoalsAttempted = request.FieldGoalsAttempted ?? s.FieldGoalsAttempted;
                    s.ExtraPointsMade = request.ExtraPointsMade ?? s.ExtraPointsMade;
                    s.ExtraPointsAttempted = request.ExtraPointsAttempted ?? s.ExtraPointsAttempted;
                    s.MinutesPlayed = request.MinutesPlayed ?? s.MinutesPlayed;
                    s.UpdatedAt = DateTime.UtcNow;
                        s.PassingAttempts = request.PassingAttempts ?? s.PassingAttempts;
                        s.CompletionPercentage = request.CompletionPercentage ?? s.CompletionPercentage;
                        s.YardsPerPassAttempt = request.YardsPerPassAttempt ?? s.YardsPerPassAttempt;
                        s.LongestPass = request.LongestPass ?? s.LongestPass;
                        s.Sacked = request.Sacked ?? s.Sacked;
                        s.Penalties = request.Penalties ?? s.Penalties;
                        s.RushingAttempts = request.RushingAttempts ?? s.RushingAttempts;
                        s.YardsPerRushAttempt = request.YardsPerRushAttempt ?? s.YardsPerRushAttempt;
                        s.RushingTDs = request.RushingTDs ?? s.RushingTDs;
                        s.LongestRushing = request.LongestRushing ?? s.LongestRushing;
                        s.Fumbles = request.Fumbles ?? s.Fumbles;
                        s.Targets = request.Targets ?? s.Targets;
                        s.YardsPerReception = request.YardsPerReception ?? s.YardsPerReception;
                        s.ReceivingTDs = request.ReceivingTDs ?? s.ReceivingTDs;
                        s.LongestReception = request.LongestReception ?? s.LongestReception;
                        s.SacksAllowed = request.SacksAllowed ?? s.SacksAllowed;
                        s.PancakeBlocks = request.PancakeBlocks ?? s.PancakeBlocks;
                        s.SnapsPlayed = request.SnapsPlayed ?? s.SnapsPlayed;
                        s.CleanSnaps = request.CleanSnaps ?? s.CleanSnaps;
                        s.TotalSnaps = request.TotalSnaps ?? s.TotalSnaps;
                        s.SnapAccuracy = request.SnapAccuracy ?? s.SnapAccuracy;
                        s.TacklesForLoss = request.TacklesForLoss ?? s.TacklesForLoss;
                        s.Pressures = request.Pressures ?? s.Pressures;
                        s.TotalTackles = request.TotalTackles ?? s.TotalTackles;
                        s.PassBreakups = request.PassBreakups ?? s.PassBreakups;
                        s.InterceptionReturnYards = request.InterceptionReturnYards ?? s.InterceptionReturnYards;
                        s.InterceptionReturnTouchDown = request.InterceptionReturnTouchDown ?? s.InterceptionReturnTouchDown;
                        s.YardsPerPunt = request.YardsPerPunt ?? s.YardsPerPunt;
                        s.Touchbacks = request.Touchbacks ?? s.Touchbacks;
                        s.FieldGoalMade = request.FieldGoalMade ?? s.FieldGoalMade;
                        s.FieldGoalAttempts = request.FieldGoalAttempts ?? s.FieldGoalAttempts;
                        s.LongestFieldGoal = request.LongestFieldGoal ?? s.LongestFieldGoal;
                        s.BlockedKicks = request.BlockedKicks ?? s.BlockedKicks;
                    await db.SaveChangesAsync();
                    return Results.Ok(new { message = "Stats updated", s.PlayerGameStatsId });
                }).RequireAuthorization("CoachOnly");

                group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
                {
                    var s = await db.PlayerGameStats.FindAsync(id);
                    if (s == null)
                        return Results.NotFound(new { error = $"Stats entry with ID {id} not found." });
                    db.PlayerGameStats.Remove(s);
                    await db.SaveChangesAsync();
                    return Results.Ok(new { message = $"Stats {id} deleted." });
                }).RequireAuthorization("CoachOnly");
        }
    }
}
