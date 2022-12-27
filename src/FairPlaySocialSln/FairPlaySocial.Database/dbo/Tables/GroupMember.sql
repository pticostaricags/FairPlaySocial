CREATE TABLE [dbo].[Group]
(
    [GroupId] BIGINT NOT NULL CONSTRAINT PK_Group_M PRIMARY KEY IDENTITY,
    [MemberApplicationUserId]  BIGINT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
);
