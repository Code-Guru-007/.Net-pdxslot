CREATE TABLE [dbo].[GameFeature]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[OperatorId] INT NULL,
	[GameId] INT NULL,
	[GameMathId] INT NULL,
	[UserId] NVARCHAR(500) NULL,
	[Feature] NVARCHAR(500) NOT NULL,
	[Value] DECIMAL(16,4) NOT NULL,
	[IsLiability] BIT NOT NULL,
	[OperatorEnabled] BIT NOT NULL,
	[UserEnabled] BIT NOT NULL,
	CONSTRAINT [FK_GameFeature_Game] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_GameFeature_GameMath] FOREIGN KEY ([GameMathId]) REFERENCES [dbo].[GameMath] ([Id]),
	CONSTRAINT [FK_GameFeature_Operator] FOREIGN KEY ([OperatorId]) REFERENCES [dbo].[Operator] ([Id])
)
