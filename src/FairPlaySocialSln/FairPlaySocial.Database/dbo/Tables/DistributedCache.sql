CREATE TABLE [dbo].[DistributedCache]
(
	[Id] NVARCHAR(900) NOT NULL CONSTRAINT PK_DistributedCache PRIMARY KEY, 
    [Value] VARBINARY(MAX) NOT NULL, 
    [ExpiresAtTime] DATETIMEOFFSET NOT NULL, 
    [SlidingExpirationInSeconds] BIGINT NULL, 
    [AbsoluteExpiration] DATETIMEOFFSET NULL
)
