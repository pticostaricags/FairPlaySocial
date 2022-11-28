CREATE TABLE [dbo].[PostUrl]
(
	[PostUrlId] BIGINT NOT NULL CONSTRAINT PK_PostUrl PRIMARY KEY IDENTITY, 
    [PostId] BIGINT NOT NULL, 
    [Url] NVARCHAR(1000) NOT NULL, 
    CONSTRAINT [FK_PostUrl_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId])
)

GO

CREATE UNIQUE INDEX [UI_PostUrl_PostId_Url] ON [dbo].[PostUrl] ([PostId], [Url])
