using HDA.Domain.Entities;
using HDA.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HDA.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(HdaDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync()) return;

        // ── Users ─────────────────────────────────────────────────────────────
        var admin = new User
        {
            Username = "admin",
            Email = "admin@hda.gg",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(admin);

        var regularUser = new User
        {
            Username = "fanboy",
            Email = "fan@hda.gg",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Fan123!"),
            Role = UserRole.Regular,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(regularUser);

        // ── Heroes ────────────────────────────────────────────────────────────
        var heroes = new[]
        {
            new Hero { Name = "anti_mage", LocalizedName = "Anti-Mage", PrimaryAttribute = "agi", AttackType = "Melee", Roles = ["Carry", "Escape"], OpenDotaId = 1, ImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/anti_mage.png" },
            new Hero { Name = "axe", LocalizedName = "Axe", PrimaryAttribute = "str", AttackType = "Melee", Roles = ["Initiator", "Durable", "Disabler"], OpenDotaId = 2, ImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/axe.png" },
            new Hero { Name = "crystal_maiden", LocalizedName = "Crystal Maiden", PrimaryAttribute = "int", AttackType = "Ranged", Roles = ["Support", "Disabler", "Nuker", "Jungler"], OpenDotaId = 5, ImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/crystal_maiden.png" },
            new Hero { Name = "invoker", LocalizedName = "Invoker", PrimaryAttribute = "int", AttackType = "Ranged", Roles = ["Carry", "Nuker", "Disabler", "Escape", "Pusher"], OpenDotaId = 74, ImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/invoker.png" },
            new Hero { Name = "pudge", LocalizedName = "Pudge", PrimaryAttribute = "str", AttackType = "Melee", Roles = ["Support", "Carry", "Durable", "Disabler", "Initiator", "Nuker"], OpenDotaId = 14, ImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/pudge.png" },
            new Hero { Name = "earthshaker", LocalizedName = "Earthshaker", PrimaryAttribute = "str", AttackType = "Melee", Roles = ["Support", "Initiator", "Disabler", "Nuker"], OpenDotaId = 7, ImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/earthshaker.png" },
        };
        context.Heroes.AddRange(heroes);

        // ── Teams ─────────────────────────────────────────────────────────────
        var teamOG = new Team { Name = "OG", Tag = "OG", Region = "Europe", Country = "International", Founded = new DateTime(2015, 8, 12), RatingPoints = 4200, WorldRank = 1, LogoUrl = "/images/teams/og.png" };
        var teamNavi = new Team { Name = "Natus Vincere", Tag = "NAVI", Region = "CIS", Country = "Ukraine", Founded = new DateTime(2009, 12, 14), RatingPoints = 3850, WorldRank = 2, LogoUrl = "/images/teams/navi.png" };
        var teamSecret = new Team { Name = "Team Secret", Tag = "Secret", Region = "Europe", Country = "International", Founded = new DateTime(2014, 9, 1), RatingPoints = 3600, WorldRank = 3, LogoUrl = "/images/teams/secret.png" };
        context.Teams.AddRange(teamOG, teamNavi, teamSecret);

        // ── Tournament ────────────────────────────────────────────────────────
        var ti = new Tournament
        {
            Name = "The International 2024",
            Organizer = "Valve",
            Region = "Global",
            PrizePool = 40_000_000,
            StartDate = new DateTime(2024, 9, 5),
            EndDate = new DateTime(2024, 9, 15),
            Tier = TournamentTier.S,
            Status = TournamentStatus.Completed,
            Description = "The biggest Dota 2 tournament of the year.",
            LogoUrl = "/images/tournaments/ti2024.png"
        };
        context.Tournaments.Add(ti);

        await context.SaveChangesAsync();

        // ── Tournament Participants ───────────────────────────────────────────
        context.TournamentParticipants.AddRange(
            new TournamentParticipant { TournamentId = ti.Id, TeamId = teamOG.Id, FinalPlacement = 1, PrizeWon = 15_000_000 },
            new TournamentParticipant { TournamentId = ti.Id, TeamId = teamNavi.Id, FinalPlacement = 2, PrizeWon = 6_000_000 },
            new TournamentParticipant { TournamentId = ti.Id, TeamId = teamSecret.Id, FinalPlacement = 3, PrizeWon = 3_000_000 }
        );

        // ── Stage ─────────────────────────────────────────────────────────────
        var stage = new TournamentStage
        {
            TournamentId = ti.Id,
            Name = "Grand Finals",
            Type = StageType.GrandFinals,
            Order = 3,
            StartDate = new DateTime(2024, 9, 15),
            EndDate = new DateTime(2024, 9, 15)
        };
        context.TournamentStages.Add(stage);
        await context.SaveChangesAsync();

        // ── Match ─────────────────────────────────────────────────────────────
        var grandFinal = new Match
        {
            TournamentId = ti.Id,
            StageId = stage.Id,
            TeamAId = teamOG.Id,
            TeamBId = teamNavi.Id,
            Format = MatchFormat.Bo5,
            Status = MatchStatus.Completed,
            ScheduledAt = new DateTime(2024, 9, 15, 14, 0, 0, DateTimeKind.Utc),
            StartedAt = new DateTime(2024, 9, 15, 14, 10, 0, DateTimeKind.Utc),
            FinishedAt = new DateTime(2024, 9, 15, 20, 30, 0, DateTimeKind.Utc),
            TeamAScore = 3,
            TeamBScore = 2,
            WinnerId = teamOG.Id,
            TwitchUrl = "https://twitch.tv/dota2ti"
        };
        context.Matches.Add(grandFinal);
        await context.SaveChangesAsync();

        // ── News ──────────────────────────────────────────────────────────────
        context.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "OG Wins The International 2024",
                Slug = "og-wins-ti-2024",
                Summary = "OG clinches the Aegis for the third time in a historic bo5 series against NAVI.",
                Content = "In an epic grand finals match, OG defeated Natus Vincere 3-2 in a thrilling best-of-five series...",
                Category = "Result",
                IsPublished = true,
                AuthorId = admin.Id,
                PublishedAt = new DateTime(2024, 9, 15, 22, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 9, 15, 22, 0, 0, DateTimeKind.Utc)
            },
            new NewsArticle
            {
                Title = "Patch 7.37 — Major Hero Rework Announced",
                Slug = "patch-737-hero-rework",
                Summary = "Valve drops a massive balance patch ahead of the new DPC season.",
                Content = "Valve has released patch 7.37 bringing significant changes to over 40 heroes...",
                Category = "Patch",
                IsPublished = true,
                AuthorId = admin.Id,
                PublishedAt = new DateTime(2024, 10, 1, 10, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 10, 1, 10, 0, 0, DateTimeKind.Utc)
            }
        );

        // ── Activity Log ──────────────────────────────────────────────────────
        context.ActivityLogs.Add(new ActivityLog
        {
            UserId = admin.Id,
            Action = "System initialized",
            Details = "Database seeded with initial data",
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }
}
