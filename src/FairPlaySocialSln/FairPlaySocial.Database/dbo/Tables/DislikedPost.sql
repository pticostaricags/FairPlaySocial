CREATE TABLE [dbo].[DislikedPost]
(
	[DislikedPostId] BIGINT NOT NULL CONSTRAINT PK_DislikedPost PRIMARY KEY IDENTITY, 
    [PostId] BIGINT NOT NULL, 
    [DislikingApplicationUserId] BIGINT NOT NULL, 
    CONSTRAINT [FK_DislikedPost_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]), 
    CONSTRAINT [FK_DislikedPost_ApplicationUser] FOREIGN KEY ([DislikingApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_DislikedPost_PoistId_DislikingApplicationUserId] ON [dbo].[DislikedPost] ([PostId], [DislikingApplicationUserId])
