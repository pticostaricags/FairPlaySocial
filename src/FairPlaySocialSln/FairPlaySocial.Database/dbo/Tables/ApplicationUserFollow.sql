CREATE TABLE [dbo].[ApplicationUserFollow]
(
	[ApplicationUserFollowId] BIGINT NOT NULL CONSTRAINT PK_ApplicationUserFollow PRIMARY KEY IDENTITY, 
    [FollowerApplicationUserId] BIGINT NOT NULL, 
    [FollowedApplicationUserId] BIGINT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_ApplicationUserFollow_FollowerApplicationUser] FOREIGN KEY ([FollowerApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]),
    CONSTRAINT [FK_ApplicationUserFollow_FollowedApplicationUser] FOREIGN KEY ([FollowedApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_ApplicationUserFollow_FollowerApplicationUserId_FollowedApplicationUserId] ON [dbo].[ApplicationUserFollow] ([FollowerApplicationUserId], [FollowedApplicationUserId])
