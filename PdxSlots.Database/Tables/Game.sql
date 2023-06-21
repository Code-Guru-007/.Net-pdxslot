CREATE TABLE [dbo].[Game]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[ExternalGameId] varchar(55) NOT NULL,
	[DesktopFileUrl] varchar(500) NOT NULL,
	[DesktopZipFileUploadId] INT NULL,
	[MobileFileUrl] varchar(500) NOT NULL,
	[MobileZipFileUploadId] INT NULL,
	[Active] BIT NOT NULL DEFAULT 0,
	CONSTRAINT [UQ_Game] UNIQUE NONCLUSTERED ([ExternalGameId]),
	CONSTRAINT [FK_Game_DesktopZipFileUpload] FOREIGN KEY ([DesktopZipFileUploadId]) REFERENCES [dbo].[ZipFileUpload] ([Id]),
	CONSTRAINT [FK_Game_MobileZipFileUpload] FOREIGN KEY ([MobileZipFileUploadId]) REFERENCES [dbo].[ZipFileUpload] ([Id])
)

