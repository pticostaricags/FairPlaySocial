CREATE TABLE [dbo].[ApplicationUserRole]
(
	[ApplicationUserRoleId] BIGINT NOT NULL CONSTRAINT PK_ApplicationUserRole PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [ApplicationRoleId] SMALLINT NOT NULL, 
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_ApplicationUserRole_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]),
    CONSTRAINT [FK_ApplicationUserRole_ApplicationRole] FOREIGN KEY ([ApplicationRoleId]) REFERENCES [ApplicationRole]([ApplicationRoleId])
)

GO


CREATE UNIQUE INDEX [UI_ApplicationUserRole] ON [dbo].[ApplicationUserRole] ([ApplicationUserId], [ApplicationRoleId])
