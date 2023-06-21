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

-- Seed round status
INSERT INTO Dbo.[RoundStatus] ([Id], [Name])
SELECT 1, 'Open' WHERE NOT EXISTS (SELECT 1 FROM Dbo.[RoundStatus] WHERE [Name] = 'Open')
GO

INSERT INTO Dbo.[RoundStatus] ([Id], [Name])
SELECT 2, 'Closed' WHERE NOT EXISTS (SELECT 1 FROM Dbo.[RoundStatus] WHERE [Name] = 'Closed')
GO

INSERT INTO Dbo.[RoundStatus] ([Id], [Name])
SELECT 3, 'Voided' WHERE NOT EXISTS (SELECT 1 FROM Dbo.[RoundStatus] WHERE [Name] = 'Voided')
GO
-- End seed round status