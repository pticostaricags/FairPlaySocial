CREATE TABLE [dbo].[ExternalReport]
(
	[ExternalReportId] INT NOT NULL CONSTRAINT PK_ExternalReport PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ExternalUrl] NVARCHAR(MAX) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_ExternalReport_Name] ON [dbo].[ExternalReport] ([Name])
