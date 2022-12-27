CREATE TABLE [dbo].[Group]
(
    [GroupId] BIGINT NOT NULL CONSTRAINT PK_Group PRIMARY KEY IDENTITY,
    [OwnerApplicationUserId]  BIGINT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(250) NOT NULL,
    [TopicTag] NVARCHAR(100) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
);
