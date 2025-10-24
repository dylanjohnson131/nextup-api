using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace NextUp.Api.Migrations
{
    public partial class FixMissingCoachRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""Coach"" (""UserId"", ""TeamId"", ""ExperienceYears"", ""Specialty"", ""Certification"", ""Bio"", ""CreatedAt"", ""UpdatedAt"")
                SELECT 
                    u.""UserId"",
                    t.""TeamId"",
                    CASE 
                        WHEN u.""FirstName"" = 'Lisa' AND u.""LastName"" = 'Martinez' THEN 10
                        WHEN u.""FirstName"" = 'Mark' AND u.""LastName"" = 'Stevens' THEN 6
                    END as ""ExperienceYears"",
                    CASE 
                        WHEN u.""FirstName"" = 'Lisa' AND u.""LastName"" = 'Martinez' THEN 'Team Coordination'
                        WHEN u.""FirstName"" = 'Mark' AND u.""LastName"" = 'Stevens' THEN 'Speed & Agility'
                    END as ""Specialty"",
                    CASE 
                        WHEN u.""FirstName"" = 'Lisa' AND u.""LastName"" = 'Martinez' THEN 'Advanced Football Coach'
                        WHEN u.""FirstName"" = 'Mark' AND u.""LastName"" = 'Stevens' THEN 'Level 2 Football Coach'
                    END as ""Certification"",
                    CASE 
                        WHEN u.""FirstName"" = 'Lisa' AND u.""LastName"" = 'Martinez' THEN 'Strategic coach with expertise in team building and coordination.'
                        WHEN u.""FirstName"" = 'Mark' AND u.""LastName"" = 'Stevens' THEN 'Dynamic coach focusing on fast-paced offensive strategies.'
                    END as ""Bio"",
                    NOW(),
                    NOW()
                FROM ""Users"" u
                JOIN ""Team"" t ON t.""CoachId"" = u.""UserId""
                WHERE (u.""FirstName"" = 'Lisa' AND u.""LastName"" = 'Martinez')
                   OR (u.""FirstName"" = 'Mark' AND u.""LastName"" = 'Stevens')
                AND NOT EXISTS (
                    SELECT 1 FROM ""Coach"" c WHERE c.""UserId"" = u.""UserId""
                );
            ");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""Coach"" 
                WHERE ""UserId"" IN (
                    SELECT u.""UserId"" 
                    FROM ""Users"" u 
                    WHERE (u.""FirstName"" = 'Lisa' AND u.""LastName"" = 'Martinez')
                       OR (u.""FirstName"" = 'Mark' AND u.""LastName"" = 'Stevens')
                );
            ");
        }
    }
}
