CREATE TABLE [dbo].[RoundStatus]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL,
	CONSTRAINT [UQ_RoundStatus_Name] UNIQUE([Name])   
)
