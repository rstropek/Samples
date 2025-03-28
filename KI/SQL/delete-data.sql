-- Delete all data from tables in correct order based on dependencies

-- First delete from junction/relationship tables
DELETE FROM Enrollments;
DELETE FROM Wizards_Creatures;
DELETE FROM Wizards_Spells;

-- Delete from tables with foreign keys to other tables
DELETE FROM Artifacts;
DELETE FROM Classes;
DELETE FROM MagicalCreatures;

-- Delete from independent tables or tables only referenced by already cleaned tables
DELETE FROM CreatureTypes;
DELETE FROM Spells;
DELETE FROM Wizards;
DELETE FROM Houses;
