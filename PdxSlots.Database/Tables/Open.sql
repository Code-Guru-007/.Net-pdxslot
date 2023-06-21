CREATE TABLE [dbo].[Open]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[OperatorId] varchar(55) NOT NULL,
	[UserId] varchar(500) NOT NULL,
	[ExternalGameId] varchar(55) NOT NULL,
	[IpAddress] varchar(500) NOT NULL,
	[OperatingSystem] varchar(500) NOT NULL,
	[Browser] varchar(500) NOT NULL,
	[Mobile] bit NOT NULL,
	[Created] datetime2(0) NOT NULL DEFAULT GETDATE()
)
