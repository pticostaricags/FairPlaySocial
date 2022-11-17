CREATE TABLE [dbo].[Post]
(
	[PostId] BIGINT NOT NULL CONSTRAINT PK_Post PRIMARY KEY IDENTITY, 
    [Text] NVARCHAR(500) NOT NULL, 
    [OwnerApplicationUserId] BIGINT NOT NULL, 
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
    CONSTRAINT [FK_Post_ApplicationUser] FOREIGN KEY ([OwnerApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)
