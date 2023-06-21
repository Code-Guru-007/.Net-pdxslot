CREATE TABLE [dbo].[GameMath]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[GameId] INT NOT NULL,
	[OperatorId] INT NOT NULL,
	[PayTable] NVARCHAR(500) NULL,
	[ExternalGameId] varchar(55) NOT NULL,
	[ExternalOperatorId] varchar(500) NOT NULL,
	[MaxBet] MONEY NOT NULL DEFAULT 0,
	[MaxLiability] MONEY NOT NULL DEFAULT 0,
	[Bets] NVARCHAR(500) NULL,
	[Denominations] NVARCHAR(500) NULL,
	[MathFileUrl] varchar(500) NULL,
	[MathFileUploadId] INT NULL,
	[AvailableDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[EndDate] DATETIME NULL,
	[Active] BIT NOT NULL DEFAULT 0,
	CONSTRAINT [FK_GameMath_Game] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_GameMath_Operator] FOREIGN KEY ([OperatorId]) REFERENCES [dbo].[Operator] ([Id]),
	CONSTRAINT [FK_GameMath_MathFileUpload] FOREIGN KEY ([MathFileUploadId]) REFERENCES [dbo].[ZipFileUpload] ([Id])
)
