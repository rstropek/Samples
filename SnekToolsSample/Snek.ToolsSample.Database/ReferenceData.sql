/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF (SELECT COUNT(*) FROM TravelType) = 0 BEGIN
	INSERT INTO TravelType ( Id, Description ) VALUES ( 0, 'Arrival' );
	INSERT INTO TravelType ( Id, Description ) VALUES ( 1, 'Continuation' );
	INSERT INTO TravelType ( Id, Description ) VALUES ( 2, 'Homeward' );
END
