CREATE TABLE [dbo].[ForbiddenUrl]
(
	[ForbiddenUrlId] BIGINT NOT NULL CONSTRAINT PK_ForbiddenUrl PRIMARY KEY IDENTITY, 
    [Url] NVARCHAR(1000) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_ForbiddenUrl_Url] ON [dbo].[ForbiddenUrl] ([Url])
