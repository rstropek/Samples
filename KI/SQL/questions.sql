/* Which house has the most powerful wizards based on spell mastery levels? */
SELECT TOP 1 h.Name AS HouseName, AVG(ws.MasteryLevel) AS AvgSpellMastery
FROM Wizards w
JOIN Houses h ON w.HouseID = h.HouseID
JOIN Wizards_Spells ws ON w.WizardID = ws.WizardID
GROUP BY h.Name
ORDER BY AvgSpellMastery DESC;

/* Which wizard, with more than 2 magical creatures, has the highest bond strength with their creatures? */
SELECT TOP 1 w.Name AS WizardName, COUNT(wc.CreatureID) AS CreatureCount, MAX(wc.BondStrength) AS TotalBondStrength
FROM Wizards w
JOIN Wizards_Creatures wc ON w.WizardID = wc.WizardID
GROUP BY w.Name
HAVING COUNT(wc.CreatureID) > 2
ORDER BY TotalBondStrength DESC;

/* Print out the Lineage of Mentorship */
WITH WizardMentorship AS (
    SELECT w.WizardID, w.Name AS WizardName, w.MentorID, 1 AS Generation
    FROM Wizards w
    WHERE w.MentorID IS NULL -- Root mentors
    
    UNION ALL
    
    SELECT w.WizardID, w.Name AS WizardName, w.MentorID, wm.Generation + 1
    FROM Wizards w
    INNER JOIN WizardMentorship wm ON w.MentorID = wm.WizardID
)
SELECT *
FROM WizardMentorship
ORDER BY Generation, WizardName;

/* Which wizards have mastered at least 2 hard spells with a mastery level of 80 or higher? */
WITH HardSpellWizards AS (
    SELECT ws.WizardID, w.Name AS WizardName, COUNT(ws.SpellID) AS HardSpellCount
    FROM Wizards_Spells ws
    JOIN Wizards w ON ws.WizardID = w.WizardID
    JOIN Spells s ON ws.SpellID = s.SpellID
    WHERE s.DifficultyLevel = 'Hard'
    AND ws.MasteryLevel >= 80
    GROUP BY ws.WizardID, w.Name
)
SELECT *
FROM HardSpellWizards
WHERE HardSpellCount >= 2;

/* Retrieve a Wizard's Full Profile */
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'GetWizardProfile' AND type = 'P')
BEGIN
    DROP PROCEDURE GetWizardProfile;
END;
 
GO

CREATE PROCEDURE GetWizardProfile @WizardID INT
AS
BEGIN
    SELECT w.WizardID, w.Name AS WizardName, h.Name AS HouseName, w.Magic_Rank, w.Birthdate
    FROM Wizards w
    LEFT JOIN Houses h ON w.HouseID = h.HouseID
    WHERE w.WizardID = @WizardID;

    SELECT mc.Name AS CreatureName, mc.Magic_Level, wc.BondStrength
    FROM Wizards_Creatures wc
    JOIN MagicalCreatures mc ON wc.CreatureID = mc.CreatureID
    WHERE wc.WizardID = @WizardID;

    SELECT a.Name AS ArtifactName, a.Magic_Type
    FROM Artifacts a
    WHERE a.OwnerID = @WizardID;
END;

GO 

/* Execute the Procedure */
EXEC GetWizardProfile 48;

GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'EnrollWizardInClass' AND type = 'P')
BEGIN
    DROP PROCEDURE EnrollWizardInClass;
END;

GO

/* Enroll a Wizard in a Class Only If They Meet Requirements */
CREATE PROCEDURE EnrollWizardInClass @WizardID INT, @ClassID INT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Classes c
        LEFT JOIN Enrollments e ON c.PrerequisiteID = e.ClassID AND e.WizardID = @WizardID
        WHERE c.ClassID = @ClassID AND e.Result != 'Passed'
    )
    BEGIN
        RAISERROR('Wizard has not passed the prerequisite class.', 16, 1);
        RETURN;
    END;
    
    INSERT INTO Enrollments (WizardID, ClassID, Result)
    VALUES (@WizardID, @ClassID, 'In Progress');
END;

GO

/* Update or Insert a Wizard's Spell Mastery */
MERGE INTO Wizards_Spells AS target
USING (VALUES (12, 8, 85)) AS source (WizardID, SpellID, MasteryLevel)
ON target.WizardID = source.WizardID AND target.SpellID = source.SpellID
WHEN MATCHED THEN
    UPDATE SET target.MasteryLevel = source.MasteryLevel
WHEN NOT MATCHED THEN
    INSERT (WizardID, SpellID, MasteryLevel)
    VALUES (source.WizardID, source.SpellID, source.MasteryLevel);
