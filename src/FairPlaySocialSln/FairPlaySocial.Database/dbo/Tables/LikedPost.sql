CREATE TABLE [dbo].[LikedPost]
(
	[LikedPostId] BIGINT NOT NULL CONSTRAINT PK_LikedPost PRIMARY KEY IDENTITY, 
    [PostId] BIGINT NOT NULL, 
    [LikingApplicationUserId] BIGINT NOT NULL, 
    CONSTRAINT [FK_LikedPost_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]), 
    CONSTRAINT [FK_LikedPost_ApplicationUser] FOREIGN KEY ([LikingApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_LikedPost_PoistId_LikingApplicationUserId] ON [dbo].[LikedPost] ([PostId], [LikingApplicationUserId])
