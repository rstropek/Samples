CREATE TABLE [dbo].[Timesheet]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	BeginTime DateTime not null,
	EndTime DateTime not null,
	[Description] nvarchar(150) null,
	TravelTypeId smallint NULL, 
	CustomerID int null,
    CONSTRAINT [FK_Timesheet_TravelType] FOREIGN KEY (TravelTypeId) REFERENCES TravelType (Id)
)
