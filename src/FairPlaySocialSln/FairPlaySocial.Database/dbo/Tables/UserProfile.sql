CREATE TABLE [dbo].[UserProfile]
(
	[UserProfileId] BIGINT NOT NULL CONSTRAINT PK_UserProfile PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [Bio] NVARCHAR(500) NOT NULL, 
    [LinkedInNickname] NVARCHAR(50) NULL, 
    [TwitterNickname] NVARCHAR(50) NULL, 
    [FacebookNickname] NVARCHAR(50) NULL, 
    [InstagramNickname] NVARCHAR(50) NULL, 
    [YouTubeNickname] NVARCHAR(50) NULL, 
    [BuyMeACoffeeNickname] NVARCHAR(50) NULL, 
    CONSTRAINT [FK_UserProfile_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_UserProfile_ApplicationUserId] ON [dbo].[UserProfile] ([ApplicationUserId])
