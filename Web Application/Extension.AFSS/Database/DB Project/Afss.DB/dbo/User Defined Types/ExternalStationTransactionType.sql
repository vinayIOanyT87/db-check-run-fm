CREATE TYPE [dbo].[ExternalStationTransactionType] AS TABLE
(
	ExternalStationTransactionGuid UNIQUEIDENTIFIER,
	ExternalStationGuid UNIQUEIDENTIFIER,
	SiteGuid UNIQUEIDENTIFIER, 
	StationTransactionID NVARCHAR(20),
	RawTransactionData NVARCHAR(MAX),
	CreatedUpdatedBy dbo.udtUserID,
	LookupExternalStationTransactionStatusIndex INT,
	LookupExternalStationTransactionFailedStatusIndex INT
)
