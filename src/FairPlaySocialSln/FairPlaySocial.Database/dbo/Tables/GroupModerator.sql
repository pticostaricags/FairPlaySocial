CREATE TABLE [dbo].[GroupModerator]
(
	[GroupModeratorId] BIGINT NOT NULL CONSTRAINT PK_GroupModerator PRIMARY KEY IDENTITY, 
    [GroupId] BIGINT NOT NULL, 
    [ModeratorApplicationUserId] BIGINT NOT NULL, 
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_GroupModerator_Group] FOREIGN KEY ([GroupId]) REFERENCES [Group]([GroupId]),
    CONSTRAINT [FK_GroupModerator_ApplicationUser] FOREIGN KEY ([ModeratorApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

