/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
BEGIN TRANSACTION
--START OF DEFAULT APPLICATION ROLES
SET IDENTITY_INSERT [dbo].[ApplicationRole] ON
DECLARE @ROLE_USER NVARCHAR(50)  = 'User'
IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationRole] AR WHERE [AR].[Name] = @ROLE_USER)
BEGIN 
    INSERT INTO [dbo].[ApplicationRole]([ApplicationRoleId],[Name],[Description]) VALUES(1, @ROLE_USER, 'Normal Users')
END
SET @ROLE_USER = 'Admin'
IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationRole] AR WHERE [AR].[Name] = @ROLE_USER)
BEGIN 
    INSERT INTO [dbo].[ApplicationRole]([ApplicationRoleId],[Name],[Description]) VALUES(3, @ROLE_USER, 'Admin Users')
END
SET IDENTITY_INSERT [dbo].[ApplicationRole] OFF
--END OF DEFAULT APPLICATION ROLES
--START OF DEFAULT POST VISIBILITY
SET IDENTITY_INSERT [dbo].[PostVisibility] ON
DECLARE @POST_VISIBILITY_ID SMALLINT = 1
IF NOT EXISTS (SELECT * FROM [dbo].[PostVisibility] PV WHERE [PV].[Name] = 'Public')
BEGIN
    INSERT INTO [dbo].[PostVisibility]([PostVisibilityId],[Name],[Description])
    VALUES (@POST_VISIBILITY_ID, 'Public', 'Visible to everyone')
END
SET @POST_VISIBILITY_ID = 2
IF NOT EXISTS (SELECT * FROM [dbo].[PostVisibility] PV WHERE [PV].[Name] = 'Subscribers')
BEGIN
    INSERT INTO [dbo].[PostVisibility]([PostVisibilityId],[Name],[Description])
    VALUES (@POST_VISIBILITY_ID, 'Subscribers', 'Visible to subscribers only')
END
SET IDENTITY_INSERT [dbo].[PostVisibility] OFF
--END OF DEFAULT POST VISIBILITY
--START OF DEFAULT POST TYPW
SET IDENTITY_INSERT [dbo].[PostType] ON
DECLARE @POST_TYPE_ID TiNYINT = 1
IF NOT EXISTS (SELECT * FROM [dbo].[PostType] PV WHERE [PV].[Name] = 'Post')
BEGIN
    INSERT INTO [dbo].[PostType]([PostTypeId],[Name])
    VALUES (@POST_TYPE_ID, 'Post')
END
SET @POST_TYPE_ID = 2
IF NOT EXISTS (SELECT * FROM [dbo].[PostType] PV WHERE [PV].[Name] = 'Comment')
BEGIN
    INSERT INTO [dbo].[PostType]([PostTypeId],[Name])
    VALUES (@POST_TYPE_ID, 'Comment')
END
SET IDENTITY_INSERT [dbo].[PostType] OFF
--END OF DEFAULT POST TYPE
--START OF DEFAULT CULTURES
SET IDENTITY_INSERT [dbo].[Culture] ON
DECLARE @CULTURE NVARCHAR(50) = 'en-US'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(1, @CULTURE)
END
SET @CULTURE='es-CR'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(2, @CULTURE)
END
SET @CULTURE='fr-CA'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(3, @CULTURE)
END
SET @CULTURE='it'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(4, @CULTURE)
END
SET IDENTITY_INSERT [dbo].[Culture] OFF
--END OF DEFAULT CULTURES
COMMIT