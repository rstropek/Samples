# SQL Queries and Procedures for Wizarding Database

## Questions

### 1. Which house has the most powerful wizards based on spell mastery levels?
<details>
<summary>View SQL Query</summary>

```sql
SELECT TOP 1 h.Name AS HouseName, AVG(ws.MasteryLevel) AS AvgSpellMastery
FROM Wizard w
JOIN House h ON w.HouseID = h.HouseID
JOIN Wizard_Spell ws ON w.WizardID = ws.WizardID
GROUP BY h.Name
ORDER BY AvgSpellMastery DESC;
```
</details>

### 2. Which wizard, with more than 2 magical creatures, has the highest bond strength with their creatures?
<details>
<summary>View SQL Query</summary>

```sql
SELECT TOP 1 w.Name AS WizardName, COUNT(wc.CreatureID) AS CreatureCount, MAX(wc.BondStrength) AS TotalBondStrength
FROM Wizard w
JOIN Wizard_Creature wc ON w.WizardID = wc.WizardID
GROUP BY w.Name
HAVING COUNT(wc.CreatureID) > 2
ORDER BY TotalBondStrength DESC;
```
</details>

## CTE Queries

### 3. Print out the Lineage of Mentorship
<details>
<summary>View SQL Query</summary>

```sql
WITH WizardMentorship AS (
    SELECT w.WizardID, w.Name AS WizardName, w.MentorID, 1 AS Generation
    FROM Wizard w
    WHERE w.MentorID IS NULL -- Root mentors
    
    UNION ALL
    
    SELECT w.WizardID, w.Name AS WizardName, w.MentorID, wm.Generation + 1
    FROM Wizard w
    INNER JOIN WizardMentorship wm ON w.MentorID = wm.WizardID
)
SELECT *
FROM WizardMentorship
ORDER BY Generation, WizardName;
```
</details>

### 4. Which wizards have mastered at least 2 hard spells with a mastery level of 80 or higher?
<details>
<summary>View SQL Query</summary>

```sql
WITH HardSpellWizards AS (
    SELECT ws.WizardID, w.Name AS WizardName, COUNT(ws.SpellID) AS HardSpellCount
    FROM Wizard_Spell ws
    JOIN Wizard w ON ws.WizardID = w.WizardID
    JOIN Spell s ON ws.SpellID = s.SpellID
    WHERE s.DifficultyLevel = 'Hard'
    AND ws.MasteryLevel >= 80
    GROUP BY ws.WizardID, w.Name
)
SELECT *
FROM HardSpellWizards
WHERE HardSpellCount >= 2;
```
</details>

## Stored Procedures

### 5. Retrieve a Wizard's Full Profile
<details>
<summary>View SQL Query</summary>

```sql
CREATE PROCEDURE GetWizardProfile @WizardID INT
AS
BEGIN
    SELECT w.WizardID, w.Name AS WizardName, h.Name AS HouseName, w.Magic_Rank, w.Birthdate
    FROM Wizard w
    LEFT JOIN House h ON w.HouseID = h.HouseID
    WHERE w.WizardID = @WizardID;

    SELECT mc.Name AS CreatureName, mc.Magic_Level, wc.BondStrength
    FROM Wizard_Creature wc
    JOIN MagicalCreature mc ON wc.CreatureID = mc.CreatureID
    WHERE wc.WizardID = @WizardID;

    SELECT a.Name AS ArtifactName, a.Magic_Type
    FROM Artifact a
    WHERE a.OwnerID = @WizardID;
END;
```
</details>

#### Execute the Procedure:
```sql
EXEC GetWizardProfile 48;
```

### 6. Enroll a Wizard in a Class Only If They Meet Requirements
<details>
<summary>View SQL Query</summary>

```sql
CREATE PROCEDURE EnrollWizardInClass @WizardID INT, @ClassID INT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Class c
        LEFT JOIN Enrollment e ON c.PrerequisiteID = e.ClassID AND e.WizardID = @WizardID
        WHERE c.ClassID = @ClassID AND e.Result != 'Passed'
    )
    BEGIN
        THROW 50001, 'Wizard has not passed the prerequisite class.', 1;
    END;
    
    INSERT INTO Enrollment (WizardID, ClassID, Result)
    VALUES (@WizardID, @ClassID, 'In Progress');
END;
```
</details>

## Merge Statement

### 7. Update or Insert a Wizard's Spell Mastery
<details>
<summary>View SQL Query</summary>

```sql
MERGE INTO Wizard_Spell AS target
USING (VALUES (12, 8, 85)) AS source (WizardID, SpellID, MasteryLevel)
ON target.WizardID = source.WizardID AND target.SpellID = source.SpellID
WHEN MATCHED THEN
    UPDATE SET target.MasteryLevel = source.MasteryLevel
WHEN NOT MATCHED THEN
    INSERT (WizardID, SpellID, MasteryLevel)
    VALUES (source.WizardID, source.SpellID, source.MasteryLevel);
```
</details>

