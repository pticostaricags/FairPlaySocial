CREATE TABLE [dbo].[PostKeyPhrase]
(
	[PostKeyPhraseId] BIGINT NOT NULL CONSTRAINT PK_PostKeyPhrase PRIMARY KEY IDENTITY, 
    [PostId] BIGINT NOT NULL, 
    [Phrase] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_PostKeyPhrase_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId])
)

GO

CREATE UNIQUE INDEX [UI_PostKeyPhrase_PostId_Phrase] ON [dbo].[PostKeyPhrase] ([PostId], [Phrase])
