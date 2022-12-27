CREATE TABLE [dbo].[GroupMember]
(
    [GroupMemberId] BIGINT NOT NULL CONSTRAINT PK_GroupMember PRIMARY KEY IDENTITY,
    [MemberApplicationUserId]  BIGINT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_GroupMember_ApplicationUser] FOREIGN KEY ([MemberApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_GroupMember_GroupMemberId_MemberApplicationUserId] ON [dbo].[GroupMember] ([GroupMemberId], [MemberApplicationUserId])
