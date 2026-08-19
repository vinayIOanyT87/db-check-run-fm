CREATE TYPE [dbo].[ExternalStationConnectionInformationType] AS TABLE
(
	ExternalStationGuid UNIQUEIDENTIFIER NOT NULL,
	LookupExternalStationStatusIndex INT NOT NULL, 
    LastSuccessfulConnection DATETIMEOFFSET NULL, 
    LastConnectionAttempt DATETIMEOFFSET NULL, 
    LastTransactionID BIGINT NULL, 
	UpdatedBy dbo.udtUserID NOT NULL
)
