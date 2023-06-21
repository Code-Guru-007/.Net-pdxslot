CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Email] NVARCHAR(320) NOT NULL, 
    [FirstName] NVARCHAR(100) NULL, 
    [LastName] NVARCHAR(100) NULL, 
    [Title] NVARCHAR(100) NULL, 
    [UserIdentityId] NVARCHAR(100) NOT NULL,
    [Created] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UC_User_UserIdentity] UNIQUE ([UserIdentityId])
)

GO
CREATE INDEX [IX_User_UserIdentityId] ON [dbo].[User] ([UserIdentityId])