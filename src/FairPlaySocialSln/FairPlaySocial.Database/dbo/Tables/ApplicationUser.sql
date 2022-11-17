CREATE TABLE [dbo].[ApplicationUser] (
    [ApplicationUserId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_ApplicationUser PRIMARY KEY, 
    [FullName] NVARCHAR(150) NOT NULL, 
    [EmailAddress] NVARCHAR(150) NOT NULL, 
    [LastLogIn] DATETIMEOFFSET NOT NULL, 
    [AzureAdB2CObjectId] UNIQUEIDENTIFIER NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
);


GO

CREATE UNIQUE INDEX [UI_ApplicationUser_AzureAdB2CObjectId] ON [dbo].[ApplicationUser] ([AzureAdB2CObjectId])
GO

CREATE UNIQUE INDEX [UI_ApplicationUser_EmailAddress] ON [dbo].[ApplicationUser] ([EmailAddress])
