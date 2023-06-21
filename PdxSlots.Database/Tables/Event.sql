CREATE TABLE [dbo].[Event]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId] INT NULL,
	[DeviceId] INT NULL,
	[GameId] INT NULL,
	[GameMathId] INT NULL,
	[OperatorId] INT NULL,
	[RoundId] INT NULL,
	[PeriodicJobId] INT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Created] DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT [FK_Event_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
	CONSTRAINT [FK_Event_Device] FOREIGN KEY ([DeviceId]) REFERENCES [dbo].[Device] ([Id]),
	CONSTRAINT [FK_Event_Game] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_Event_GameMath] FOREIGN KEY ([GameMathId]) REFERENCES [dbo].[GameMath] ([Id]),
	CONSTRAINT [FK_Event_Operator] FOREIGN KEY ([OperatorId]) REFERENCES [dbo].[Operator] ([Id]),
	CONSTRAINT [FK_Event_Round] FOREIGN KEY ([RoundId]) REFERENCES [dbo].[Round] ([Id]),
	CONSTRAINT [FK_Event_PeriodicJob] FOREIGN KEY ([PeriodicJobId]) REFERENCES [dbo].[PeriodicJob] ([Id])
)
