CREATE TABLE [dbo].[Photo]
(
	[PhotoId] BIGINT NOT NULL CONSTRAINT PK_Photo PRIMARY KEY IDENTITY, 
    [Filename] NVARCHAR(50) NOT NULL, 
    [ImageType] NVARCHAR(10) NOT NULL, 
    [ImageBytes] VARBINARY(MAX) NOT NULL,
    [AlternativeText] NVARCHAR(50) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
)
