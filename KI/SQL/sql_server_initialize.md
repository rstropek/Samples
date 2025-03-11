### SQL Initialisierungs Script
```sql
-- create database WizardAcademy

IF NOT EXISTS (SELECT name
               FROM sys.databases
               WHERE name = 'WizardAcademy')
    BEGIN
        CREATE DATABASE WizardAcademy;
    END
GO

-- set scheme

USE WizardAcademy;
GO

-- Tables

-- Houses
CREATE TABLE Houses
(
    HouseID         INT IDENTITY (1,1) PRIMARY KEY,
    Name            NVARCHAR(50) UNIQUE NOT NULL,
    FoundingYear    INT CHECK (FoundingYear >= 0),
    SpecialFeatures NVARCHAR(MAX)
);

-- Wizards
CREATE TABLE Wizards
(
    WizardID   INT IDENTITY (1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL,
    Birthdate  DATE,
    HouseID    INT,
    MentorID   INT,
    Magic_Rank NVARCHAR(20) CHECK (Magic_Rank IN ('Apprentice', 'Mage', 'Archmage')),
    FOREIGN KEY (HouseID) REFERENCES Houses (HouseID),
    FOREIGN KEY (MentorID) REFERENCES Wizards (WizardID)
);

-- Spells
CREATE TABLE Spells
(
    SpellID         INT IDENTITY (1,1) PRIMARY KEY,
    Name            NVARCHAR(100) UNIQUE NOT NULL,
    DifficultyLevel NVARCHAR(10) CHECK (DifficultyLevel IN ('Easy', 'Medium', 'Hard')),
    Element         NVARCHAR(10) CHECK (Element IN ('Fire', 'Water', 'Earth', 'Air', 'Dark', 'Light')),
    Description     NVARCHAR(MAX)
);

-- Wizards_Spells (M:N relationship between Wizards and Spells)
CREATE TABLE Wizards_Spells
(
    WizardID     INT,
    SpellID      INT,
    MasteryLevel INT CHECK (MasteryLevel BETWEEN 1 AND 100),
    PRIMARY KEY (WizardID, SpellID),
    FOREIGN KEY (WizardID) REFERENCES Wizards (WizardID),
    FOREIGN KEY (SpellID) REFERENCES Spells (SpellID)
);

-- Creature Types
CREATE TABLE CreatureTypes
(
    TypeID   INT IDENTITY (1,1) PRIMARY KEY,
    TypeName NVARCHAR(50) UNIQUE NOT NULL
);

-- Magical Creatures
CREATE TABLE MagicalCreatures
(
    CreatureID         INT IDENTITY (1,1) PRIMARY KEY,
    Name               NVARCHAR(100) NOT NULL,
    TypeID             INT           NOT NULL,
    Magic_Level        INT CHECK (Magic_Level BETWEEN 1 AND 100),
    RelationshipStatus NVARCHAR(10) CHECK (RelationshipStatus IN ('Wild', 'Tamed', 'Allied')),
    FOREIGN KEY (TypeID) REFERENCES CreatureTypes (TypeID)
);

-- Wizards_Creatures (M:N relationship)
CREATE TABLE Wizards_Creatures
(
    WizardID     INT,
    CreatureID   INT,
    BondStrength INT CHECK (BondStrength BETWEEN 1 AND 100),
    PRIMARY KEY (WizardID, CreatureID),
    FOREIGN KEY (WizardID) REFERENCES Wizards (WizardID),
    FOREIGN KEY (CreatureID) REFERENCES MagicalCreatures (CreatureID)
);

-- Artifacts
CREATE TABLE Artifacts
(
    ArtifactID INT IDENTITY (1,1) PRIMARY KEY,
    Name       NVARCHAR(100) UNIQUE NOT NULL,
    Magic_Type NVARCHAR(10) CHECK (Magic_Type IN ('Protection', 'Attack', 'Healing', 'Curse')),
    OwnerID    INT,
    FOREIGN KEY (OwnerID) REFERENCES Wizards (WizardID)
);

-- Classes (with recursive structure)
CREATE TABLE Classes
(
    ClassID          INT IDENTITY (1,1) PRIMARY KEY,
    Subject          NVARCHAR(100) UNIQUE NOT NULL,
    InstructorID     INT,
    Max_Participants INT,
    PrerequisiteID   INT,
    FOREIGN KEY (InstructorID) REFERENCES Wizards (WizardID),
    FOREIGN KEY (PrerequisiteID) REFERENCES Classes (ClassID)
);

-- Enrollments (M:N relationship between Wizards and Classes)
CREATE TABLE Enrollments
(
    WizardID INT,
    ClassID  INT,
    Result   NVARCHAR(12) CHECK (Result IN ('Passed', 'Failed', 'In Progress')),
    PRIMARY KEY (WizardID, ClassID),
    FOREIGN KEY (WizardID) REFERENCES Wizards (WizardID),
    FOREIGN KEY (ClassID) REFERENCES Classes (ClassID)
);

-- trigger so no wild creature can be assigned to a wizard
CREATE TRIGGER PreventWildCreatures
    ON Wizards_Creatures
    INSTEAD OF INSERT
    AS
BEGIN
    IF EXISTS (SELECT 1
               FROM inserted i
                        JOIN MagicalCreatures mc ON i.CreatureID = mc.CreatureID
               WHERE mc.RelationshipStatus = 'Wild')
        BEGIN
            THROW 50000, 'Cannot assign wild creatures to wizards.', 1;
            RETURN;
        END;

    -- If valid, proceed with insertion
    INSERT INTO Wizards_Creatures (WizardID, CreatureID, BondStrength)
    SELECT WizardID, CreatureID, BondStrength
    FROM inserted;
END;

    -- Inserting data

-- Insert Creature Types
    INSERT INTO CreatureTypes (TypeName)
    VALUES ('Dragon'),
           ('Griffin'),
           ('Phoenix'),
           ('Basilisk'),
           ('Unicorn'),
           ('Chimera'),
           ('Golem'),
           ('Fairy'),
           ('Werewolf'),
           ('Shadow Beast');

-- Insert Houses
    INSERT INTO Houses (Name, FoundingYear, SpecialFeatures)
    VALUES ('House of Flames', 1200, 'Mastery of fire magic'),
           ('Frozen Keep', 1300, 'Specializes in ice and water spells'),
           ('Stormcallers', 1250, 'Focused on air and lightning magic'),
           ('Earthwardens', 1100, 'Protectors of the land and nature magic'),
           ('Voidborn', 1400, 'Masters of dark and void magic'),
           ('Celestial Order', 1500, 'Sacred magic and healing arts');

-- Insert Wizards
    INSERT INTO Wizards (Name, Birthdate, HouseID, MentorID, Magic_Rank)
    VALUES ('Merlin Fireborn', '1010-05-14', 1, NULL, 'Archmage'),
           ('Lysandra Frostveil', '1120-07-14', 2, 1, 'Mage'),
           ('Garrik Stormcaller', '1205-05-30', 3, 1, 'Apprentice'),
           ('Eldon Earthshaker', '1185-03-21', 4, 2, 'Mage'),
           ('Seraphina Lightweaver', '1230-09-10', 5, NULL, 'Archmage'),
           ('Thalion Shadowbane', '1100-11-20', 6, 1, 'Mage'),
           ('Valeria Sunstrike', '1290-02-17', 5, 2, 'Apprentice'),
           ('Darius Nightfall', '1315-06-30', 6, 3, 'Mage'),
           ('Orion Windwalker', '1199-08-25', 3, 1, 'Apprentice'),
           ('Sylva Moonshade', '1266-04-12', 2, 5, 'Mage'),
           ('Borin Ironfist', '1220-12-01', 4, 6, 'Apprentice'),
           ('Azariah Starfire', '1305-03-29', 1, NULL, 'Archmage'),
           ('Cassius Thunderstrike', '1282-09-23', 3, 2, 'Mage'),
           ('Elena Brightwind', '1247-07-19', 5, 3, 'Apprentice'),
           ('Ragnar Duskrune', '1278-10-05', 6, 4, 'Mage'),
           ('Vesper Shadowthorn', '1321-01-16', 2, 1, 'Apprentice'),
           ('Lucian Emberflare', '1215-04-22', 1, NULL, 'Archmage'),
           ('Zara Frostwhisper', '1298-06-13', 2, 6, 'Mage'),
           ('Felix Stormweaver', '1256-02-09', 3, 1, 'Apprentice'),
           ('Helios Dawnbringer', '1300-09-27', 5, NULL, 'Archmage'),
           ('Cyrus Nightshade', '1262-05-31', 6, 8, 'Mage'),
           ('Nova Windrider', '1329-07-02', 3, 1, 'Apprentice'),
           ('Gideon Earthwarden', '1287-10-11', 4, 5, 'Mage'),
           ('Selene Starlight', '1308-03-15', 5, NULL, 'Archmage'),
           ('Orpheus Duskbane', '1314-12-22', 6, 11, 'Mage'),
           ('Rowan Firebrand', '1272-06-17', 1, 12, 'Apprentice'),
           ('Athena Moonblade', '1251-01-08', 2, 1, 'Mage'),
           ('Magnus Stormchaser', '1302-11-19', 3, 5, 'Apprentice'),
           ('Zephyr Sunfire', '1319-04-06', 5, NULL, 'Archmage'),
           ('Lyra Frostsong', '1245-09-03', 2, 15, 'Mage'),
           ('Tiberius Shadowmark', '1267-07-25', 6, 16, 'Apprentice'),
           ('Artemis Starborn', '1284-08-16', 5, NULL, 'Archmage'),
           ('Draven Darkfang', '1291-02-11', 6, 17, 'Mage'),
           ('Caius Fireheart', '1275-06-01', 1, 1, 'Apprentice'),
           ('Evangeline Lightrune', '1295-05-09', 5, NULL, 'Archmage'),
           ('Aldric Ironhelm', '1307-10-14', 4, 5, 'Mage'),
           ('Lucinda Stormwhisper', '1264-03-23', 3, 5, 'Apprentice'),
           ('Oberon Nightwalker', '1325-01-07', 6, 21, 'Mage'),
           ('Isolde Moonray', '1243-12-09', 2, NULL, 'Archmage'),
           ('Victor Daggerfall', '1259-06-26', 6, 22, 'Mage'),
           ('Freya Sunfury', '1289-08-30', 5, NULL, 'Archmage'),
           ('Soren Duskmire', '1311-09-14', 6, 23, 'Mage'),
           ('Callista Iceborn', '1279-07-21', 2, 1, 'Apprentice'),
           ('Dante Fireforge', '1293-04-11', 1, NULL, 'Archmage'),
           ('Iskander Stonehand', '1257-10-31', 4, 5, 'Mage'),
           ('Valeria Moonfire', '1276-06-08', 5, NULL, 'Archmage'),
           ('Renard Blackthorn', '1304-05-15', 6, 26, 'Mage'),
           ('Anya Starfrost', '1327-09-18', 2, 1, 'Apprentice');

-- Insert Spells
    INSERT INTO Spells (Name, DifficultyLevel, Element, Description)
    VALUES ('Inferno Surge', 'Hard', 'Fire', 'Unleashes a massive eruption of flames that engulfs enemies'),
           ('Frostbite Prison', 'Medium', 'Water', 'Encases the target in an icy prison, freezing them in place'),
           ('Thunderclap', 'Easy', 'Air', 'Emits a loud thunderous boom, disorienting nearby enemies'),
           (N'Stone Titan’s Embrace', 'Hard', 'Earth', 'Summons colossal stone arms to immobilize the target'),
           ('Celestial Radiance', 'Medium', 'Light', 'Channels divine energy to heal allies and blind foes'),
           ('Abyssal Grasp', 'Hard', 'Dark', 'Summons shadowy tendrils to weaken and drain energy from enemies'),
           ('Tempest Cyclone', 'Hard', 'Air', 'Creates a raging storm, hurling enemies and objects into the air'),
           ('Molten Spear', 'Medium', 'Fire', 'Launches a concentrated spear of molten lava'),
           ('Glacial Barrier', 'Easy', 'Water', 'Forms a protective shield of ice around the caster'),
           ('Tremor Wave', 'Medium', 'Earth', 'Sends a seismic shockwave that disrupts balance and causes damage'),
           ('Lunar Blessing', 'Easy', 'Light', 'Increases the magic power of allies under moonlight'),
           ('Void Step', 'Hard', 'Dark', 'Allows the caster to momentarily phase into the void to avoid attacks'),
           ('Gale Dance', 'Medium', 'Air', 'Enhances speed and agility with the power of wind currents'),
           ('Cinder Cloak', 'Easy', 'Fire', 'Surrounds the caster in embers, burning anyone who comes too close'),
           ('Tidal Crash', 'Hard', 'Water', 'Summons a towering wave to crush foes in its path'),
           ('Earthen Fortress', 'Medium', 'Earth', 'Raises an impenetrable barrier of stone from the ground'),
           ('Solar Flare', 'Hard', 'Light', 'Unleashes a concentrated burst of sunlight, searing enemies'),
           ('Nightmare Veil', 'Medium', 'Dark', 'Shrouds the battlefield in shadows, confusing enemies'),
           ('Ethereal Drift', 'Easy', 'Air', 'Temporarily grants the caster the ability to levitate'),
           ('Lava Flow', 'Hard', 'Fire', 'Melts the ground beneath enemies, creating pools of molten rock');

-- Insert spells wizard relation
    INSERT INTO Wizards_Spells (WizardID, SpellID, MasteryLevel)
    VALUES (1, 1, 14),
           (1, 7, 49),
           (1, 17, 83),
           (1, 19, 62),
           (1, 8, 59),
           (2, 3, 20),
           (2, 14, 20),
           (2, 5, 65),
           (2, 10, 63),
           (3, 4, 100),
           (3, 9, 20),
           (3, 20, 41),
           (4, 2, 90),
           (4, 9, 70),
           (4, 16, 17),
           (4, 6, 99),
           (5, 5, 63),
           (5, 11, 87),
           (5, 15, 33),
           (5, 18, 76),
           (5, 14, 58),
           (5, 3, 76),
           (6, 1, 47),
           (6, 5, 83),
           (6, 10, 78),
           (6, 17, 11),
           (6, 8, 33),
           (7, 6, 48),
           (7, 19, 30),
           (7, 3, 52),
           (7, 12, 81),
           (7, 14, 94),
           (7, 7, 76),
           (8, 11, 30),
           (8, 18, 70),
           (8, 8, 26),
           (8, 2, 57),
           (9, 5, 10),
           (9, 9, 93),
           (9, 16, 84),
           (9, 7, 73),
           (10, 10, 25),
           (10, 14, 85),
           (10, 19, 93),
           (10, 12, 15),
           (10, 8, 66),
           (10, 6, 20),
           (11, 4, 53),
           (11, 7, 40),
           (11, 17, 47),
           (12, 2, 95),
           (12, 8, 70),
           (12, 20, 26),
           (12, 3, 33),
           (12, 12, 34),
           (13, 1, 97),
           (13, 6, 60),
           (13, 14, 20),
           (13, 8, 87),
           (13, 11, 58),
           (14, 3, 84),
           (14, 5, 64),
           (14, 10, 89),
           (14, 18, 74),
           (14, 14, 65),
           (14, 9, 84),
           (15, 8, 33),
           (15, 16, 57),
           (15, 19, 63),
           (16, 10, 34),
           (16, 13, 80),
           (16, 17, 17),
           (16, 6, 22),
           (17, 4, 31),
           (17, 5, 20),
           (17, 14, 33),
           (17, 11, 67),
           (17, 2, 16),
           (18, 3, 20),
           (18, 6, 39),
           (18, 10, 76),
           (18, 18, 33),
           (18, 14, 13),
           (19, 8, 38),
           (19, 16, 63),
           (19, 19, 90),
           (19, 12, 29),
           (19, 2, 88),
           (20, 6, 47),
           (20, 14, 64),
           (20, 17, 21),
           (20, 4, 55),
           (20, 1, 54),
           (21, 10, 14),
           (21, 13, 91),
           (21, 17, 16),
           (21, 6, 92),
           (21, 7, 95),
           (22, 4, 40),
           (22, 5, 65),
           (22, 14, 87),
           (22, 11, 74),
           (22, 2, 46),
           (22, 9, 50),
           (23, 2, 80),
           (23, 5, 64),
           (23, 9, 86),
           (24, 4, 77),
           (24, 10, 81),
           (24, 18, 12),
           (25, 6, 27),
           (25, 19, 83),
           (25, 3, 32),
           (25, 12, 72),
           (25, 14, 91),
           (26, 1, 58),
           (26, 5, 11),
           (26, 10, 90),
           (26, 17, 45),
           (27, 6, 77),
           (27, 19, 74),
           (27, 3, 73),
           (28, 2, 90),
           (28, 8, 85),
           (28, 20, 83),
           (28, 3, 88),
           (29, 3, 76),
           (29, 5, 74),
           (29, 10, 73),
           (29, 18, 71),
           (29, 14, 70),
           (30, 8, 77),
           (30, 16, 75),
           (30, 19, 74),
           (30, 12, 73),
           (31, 10, 78),
           (31, 13, 76),
           (31, 17, 74),
           (31, 6, 73),
           (32, 4, 80),
           (32, 5, 78),
           (32, 14, 76),
           (32, 11, 75),
           (33, 6, 86),
           (33, 14, 82),
           (33, 17, 81),
           (34, 10, 78),
           (34, 13, 76),
           (34, 17, 74),
           (35, 4, 80),
           (35, 5, 78),
           (35, 14, 76),
           (35, 11, 75),
           (36, 2, 71),
           (36, 5, 70),
           (36, 9, 69),
           (36, 12, 68),
           (37, 4, 76),
           (37, 10, 74),
           (37, 18, 72),
           (37, 6, 71),
           (38, 6, 77),
           (38, 19, 74),
           (38, 3, 73),
           (38, 12, 72),
           (39, 8, 77),
           (39, 16, 75),
           (39, 19, 74),
           (39, 12, 73),
           (40, 10, 78),
           (40, 13, 76),
           (40, 17, 74),
           (40, 6, 73),
           (41, 4, 80),
           (41, 5, 78),
           (41, 14, 76),
           (41, 11, 75),
           (42, 6, 86),
           (42, 14, 82),
           (42, 17, 81),
           (43, 10, 78),
           (43, 13, 76),
           (43, 17, 74),
           (44, 4, 80),
           (44, 5, 78),
           (44, 14, 76),
           (44, 11, 75),
           (45, 2, 71),
           (45, 5, 70),
           (45, 9, 69),
           (45, 12, 68),
           (46, 20, 41),
           (46, 10, 61),
           (47, 12, 100),
           (47, 16, 88),
           (47, 8, 77),
           (47, 3, 44),
           (47, 2, 98),
           (48, 17, 82),
           (48, 13, 72),
           (48, 11, 87),
           (48, 7, 92);

-- Insert magical Creatures
    INSERT INTO MagicalCreatures (Name, TypeID, Magic_Level, RelationshipStatus)
    VALUES ('Emberdrake', 1, 84, 'Tamed'),
           ('Celestara', 3, 50, 'Tamed'),
           ('Thornspike', 7, 60, 'Tamed'),
           ('Blazetail', 6, 85, 'Tamed'),
           ('Luminara', 5, 66, 'Tamed'),
           ('Moonveil', 8, 59, 'Tamed'),
           ('Galeclaw', 2, 65, 'Allied'),
           ('Stormtalon', 2, 80, 'Allied'),
           ('Thunderhoof', 5, 62, 'Allied'),
           ('Shadowfang', 9, 70, 'Allied'),
           ('Aquaflare', 4, 78, 'Allied'),
           ('Earthshaker', 7, 92, 'Allied'),
           ('Nightwhisper', 8, 54, 'Allied'),
           ('Frostpaw', 9, 72, 'Allied'),
           ('Duskprowler', 9, 72, 'Allied'),
           ('Tidecaller', 4, 69, 'Wild'),
           ('Glacierfang', 9, 60, 'Wild'),
           ('Bramblefiend', 7, 74, 'Wild'),
           ('Infernal Maw', 1, 97, 'Wild'),
           ('Voidfang', 10, 50, 'Wild');

-- Insert wizard creature relation
    INSERT INTO Wizards_Creatures (WizardID, CreatureID, BondStrength)
    VALUES
        (3, 2, 29), (3, 13, 9), (3, 4, 100),
        (6, 3, 98), (6, 1, 96), (6, 13, 55),
        (8, 11, 59),
        (10, 2, 91),
        (12, 5, 93), (12, 12, 49),
        (15, 12, 20),
        (16, 13, 77), (16, 11, 82), (16, 2, 26),
        (18, 2, 40), (18, 11, 45), (18, 5, 99),
        (22, 14, 90), (22, 2, 86),
        (24, 14, 9),
        (26, 13, 12), (26, 10, 83),
        (27, 15, 59),
        (28, 1, 7),
        (29, 13, 90), (29, 11, 77),
        (30, 5, 8), (30, 11, 58),
        (31, 10, 67),
        (34, 1, 74), (34, 5, 85), (34, 9, 79),
        (36, 13, 55), (36, 8, 35),
        (38, 11, 88),
        (39, 1, 89), (39, 12, 29),
        (44, 1, 66), (44, 2, 5), (44, 9, 29),
        (45, 10, 84), (45, 9, 3), (45, 11, 54),
        (46, 7, 85), (46, 6, 54),
        (48, 14, 70), (48, 13, 32), (48, 12, 41), (48, 7, 58), (48, 1, 96);


-- Insert Artifacts
    INSERT INTO Artifacts (Name, Magic_Type, OwnerID)
    VALUES ('Orb of Eternity', 'Protection', 5),
           ('Blade of the Ancients', 'Protection', 7),
           ('Staff of Storms', 'Protection', 32),
           ('Ring of Shadows', 'Curse', 44),
           ('Crown of the Phoenix', 'Protection', 14),
           ('Amulet of the Moon', 'Healing', 32),
           ('Gauntlets of Power', 'Protection', 33),
           ('Tome of Lost Knowledge', 'Protection', 33),
           ('Crystal of the Arcane', 'Protection', 13),
           ('Cloak of Invisibility', 'Healing', 6),
           ('Pendant of the Depths', 'Protection', 48),
           ('Helm of Wisdom', 'Healing', 48),
           ('Boots of Swiftness', 'Healing', 5),
           ('Bracers of the Titan', 'Curse', 46),
           ('Necklace of the Void', 'Protection', 33),
           ('Lance of the Sun', 'Curse', 13),
           ('Shield of Aegis', 'Healing', 33),
           ('Horn of the Wild', 'Protection', 48),
           ('Mask of Illusions', 'Curse', 13),
           ('Chalice of Destiny', 'Healing', 41);

-- Insert Classes
    INSERT INTO Classes (Subject, InstructorID, Max_Participants, PrerequisiteID)
    VALUES ('Introduction to Elemental Magic', 5, 20, NULL),
           ('Advanced Fire Manipulation', 7, 15, 1),
           ('Defensive Spellcasting', 32, 25, NULL),
           ('Dark Arts and Forbidden Spells', 44, 10, NULL),
           ('Potion Brewing and Alchemy', 14, 18, NULL),
           ('Summoning and Familiar Bonding', 33, 12, 3),
           ('Healing and Restoration Magic', 6, 22, 5),
           ('Teleportation and Spatial Manipulation', 48, 14, NULL),
           ('Enchanting and Rune Crafting', 13, 16, 2),
           ('Necromancy: Theory and Ethics', 41, 8, 4);

-- Insert Enrollments
    INSERT INTO Enrollments (WizardID, ClassID, Result)
    VALUES (1, 1, 'In Progress'),
           (2, 1, 'Failed'),
           (2, 3, 'In Progress'),
           (3, 3, 'Passed'),
           (3, 6, 'In Progress'),
           (4, 4, 'Passed'),
           (4, 10, 'Passed'),
           (5, 5, 'Passed'),
           (5, 7, 'In Progress'),
           (6, 1, 'Failed'),
           (6, 4, 'In Progress'),
           (7, 3, 'Passed'),
           (7, 6, 'In Progress'),
           (8, 4, 'In Progress'),
           (9, 5, 'Passed'),
           (9, 7, 'Failed'),
           (9, 1, 'In Progress'),
           (10, 1, 'Failed'),
           (10, 5, 'In Progress'),
           (11, 3, 'In Progress'),
           (12, 4, 'Failed'),
           (12, 8, 'In Progress'),
           (13, 5, 'Passed'),
           (13, 7, 'In Progress'),
           (14, 1, 'Failed'),
           (14, 3, 'In Progress'),
           (15, 3, 'In Progress'),
           (16, 4, 'Failed'),
           (16, 1, 'In Progress'),
           (17, 5, 'In Progress'),
           (18, 1, 'Failed'),
           (18, 8, 'In Progress'),
           (19, 1, 'Passed'),
           (19, 2, 'Failed'),
           (20, 8, 'In Progress'),
           (21, 5, 'Passed'),
           (21, 7, 'In Progress'),
           (22, 1, 'Failed'),
           (22, 5, 'In Progress'),
           (23, 3, 'Passed'),
           (23, 6, 'In Progress'),
           (24, 4, 'In Progress'),
           (25, 5, 'Passed'),
           (25, 7, 'Failed'),
           (26, 1, 'Failed'),
           (26, 3, 'In Progress'),
           (27, 3, 'In Progress'),
           (28, 4, 'Passed'),
           (28, 10, 'In Progress'),
           (29, 1, 'Passed'),
           (29, 2, 'Passed'),
           (29, 9, 'In Progress'),
           (30, 8, 'Failed'),
           (30, 1, 'In Progress'),
           (31, 3, 'Passed'),
           (31, 6, 'In Progress'),
           (32, 4, 'In Progress'),
           (33, 8, 'Passed'),
           (34, 1, 'In Progress'),
           (35, 3, 'Passed'),
           (35, 6, 'In Progress'),
           (36, 4, 'Failed'),
           (36, 5, 'In Progress'),
           (37, 5, 'In Progress'),
           (38, 1, 'Failed'),
           (38, 3, 'In Progress'),
           (39, 3, 'Passed'),
           (39, 6, 'In Progress'),
           (40, 4, 'Failed'),
           (40, 8, 'In Progress'),
           (41, 5, 'Passed'),
           (41, 7, 'Failed'),
           (42, 1, 'In Progress'),
           (43, 3, 'Passed'),
           (43, 6, 'Failed'),
           (44, 4, 'Failed'),
           (44, 1, 'In Progress'),
           (45, 5, 'Passed'),
           (45, 7, 'In Progress'),
           (46, 1, 'Failed'),
           (46, 3, 'In Progress'),
           (47, 3, 'In Progress'),
           (48, 1, 'Passed'),
           (48, 2, 'Passed'),
           (48, 9, 'Passed');

```