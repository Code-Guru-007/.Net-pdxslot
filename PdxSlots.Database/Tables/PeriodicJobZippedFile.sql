CREATE TABLE [dbo].[PeriodicJobZippedFile]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[PeriodicJobId] INT NOT NULL,
	[ZippedFileId] INT NOT NULL,
	[OriginalHash] NVARCHAR(MAX) NOT NULL,
	[CurrentHash] NVARCHAR(MAX) NOT NULL,
	[HashEquals] BIT NOT NULL,
	[HashCheck] BIT NOT NULL,
	[Created] DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT [FK_PeriodicJobZippedFile_ZippedFile] FOREIGN KEY ([ZippedFileId]) REFERENCES [dbo].[ZippedFile] ([Id]),
	CONSTRAINT [FK_PeriodicJobZippedFile_PeriodicJob] FOREIGN KEY ([PeriodicJobId]) REFERENCES [dbo].[PeriodicJob] ([Id])
)
