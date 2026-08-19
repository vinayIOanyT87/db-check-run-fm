CREATE TYPE [dbo].[ExternalStationLogType] AS TABLE
(
	ExternalStationLogGuid UNIQUEIDENTIFIER NOT NULL,
	SiteGuid UNIQUEIDENTIFIER NOT NULL,  
	ExternalStationGuid UNIQUEIDENTIFIER NOT NULL, 
    LogText NVARCHAR(MAX) NOT NULL, 
    LookupExternalStationLogTypeIndex INT NOT NULL,     
	LogDate DATETIMEOFFSET NOT NULL,
	CreatedUpdatedBy dbo.udtUserID NOT NULL
)
