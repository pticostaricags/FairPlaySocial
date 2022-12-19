﻿CREATE TABLE [dbo].[Post]
(
	[PostId] BIGINT NOT NULL CONSTRAINT PK_Post PRIMARY KEY IDENTITY, 
    [PostVisibilityId] SMALLINT NOT NULL DEFAULT 1, 
    [PhotoId] BIGINT NULL,
    [PostTypeId] TINYINT NOT NULL DEFAULT 1, 
    [ReplyToPostId] BIGINT NULL,
    [Text] NVARCHAR(500) NOT NULL,
    [OwnerApplicationUserId] BIGINT NOT NULL, 
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo),
    [CreatedFromPostId] BIGINT NULL, 
    CONSTRAINT [FK_Post_ApplicationUser] FOREIGN KEY ([OwnerApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]),  
    CONSTRAINT [FK_Post_Photo] FOREIGN KEY ([PhotoId]) REFERENCES [Photo]([PhotoId]), 
    CONSTRAINT [FK_Post_PostVisibility] FOREIGN KEY ([PostVisibilityId]) REFERENCES [PostVisibility]([PostVisibilityId]), 
    CONSTRAINT [FK_Post_Post] FOREIGN KEY ([CreatedFromPostId]) REFERENCES [Post]([PostId]), 
    CONSTRAINT [FK_Post_Post_ReplyToPostId] FOREIGN KEY ([ReplyToPostId]) REFERENCES [Post]([PostId]), 
    CONSTRAINT [FK_Post_PostType] FOREIGN KEY ([PostTypeId]) REFERENCES [PostType]([PostTypeId])
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.PostHistory))