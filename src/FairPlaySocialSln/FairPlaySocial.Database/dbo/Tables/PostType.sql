﻿CREATE TABLE [dbo].[PostType]
(
	[PostTypeId] TINYINT NOT NULL CONSTRAINT PK_PostType PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(10) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_PostType_Name] ON [dbo].[PostType] ([Name])
