CREATE TABLE [dbo].[UserPreference]
(
	[UserPreferenceId] BIGINT NOT NULL CONSTRAINT PK_UserPreference PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [EnableAudibleCuesInMobile] BIT NOT NULL, 
    [EnableAudibleCuesInWeb] BIT NOT NULL, 
    CONSTRAINT [FK_UserPreference_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]),
)

GO

CREATE UNIQUE INDEX [UI_UserPreference_ApplicationUserId] ON [dbo].[UserPreference] ([ApplicationUserId])
