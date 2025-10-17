-- Fix missing Coach records for Lisa Martinez and Mark Stevens
-- First, let's see the User IDs for these coaches
SELECT u.UserId, u.FirstName, u.LastName, u.Email, t.TeamId, t.Name as TeamName
FROM "User" u
JOIN "Team" t ON t.CoachId = u.UserId
WHERE u.FirstName IN ('Lisa', 'Mark');

-- Check if Coach records exist
SELECT c.CoachId, c.UserId, u.FirstName, u.LastName
FROM "Coach" c
JOIN "User" u ON c.UserId = u.UserId
WHERE u.FirstName IN ('Lisa', 'Mark');

-- Insert missing Coach records if they don't exist
INSERT INTO "Coach" (UserId, TeamId, ExperienceYears, Specialty, Certification, Bio, CreatedAt, UpdatedAt)
SELECT 
    u.UserId,
    t.TeamId,
    CASE 
        WHEN u.FirstName = 'Lisa' THEN 10
        WHEN u.FirstName = 'Mark' THEN 6
    END as ExperienceYears,
    CASE 
        WHEN u.FirstName = 'Lisa' THEN 'Team Coordination'
        WHEN u.FirstName = 'Mark' THEN 'Speed & Agility'
    END as Specialty,
    CASE 
        WHEN u.FirstName = 'Lisa' THEN 'Advanced Football Coach'
        WHEN u.FirstName = 'Mark' THEN 'Level 2 Football Coach'
    END as Certification,
    CASE 
        WHEN u.FirstName = 'Lisa' THEN 'Strategic coach with expertise in team building and coordination.'
        WHEN u.FirstName = 'Mark' THEN 'Dynamic coach focusing on fast-paced offensive strategies.'
    END as Bio,
    NOW(),
    NOW()
FROM "User" u
JOIN "Team" t ON t.CoachId = u.UserId
WHERE u.FirstName IN ('Lisa', 'Mark')
AND NOT EXISTS (
    SELECT 1 FROM "Coach" c WHERE c.UserId = u.UserId
);