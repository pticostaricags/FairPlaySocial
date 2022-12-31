CREATE TABLE [dbo].[PostReach] (
	[PostReachId] BIGINT NOT NULL CONSTRAINT PK_PostReach PRIMARY KEY IDENTITY, 
    [ReachedByApplicationUserId] BIGINT NOT NULL, 
    [PostId] BIGINT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_PostReach_ApplicationUser] FOREIGN KEY ([ReachedByApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]),
    CONSTRAINT [FK_PostReach_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId])
)

