CREATE TYPE [dbo].[ExternalStationTransactionFailedStatusType] AS TABLE
(
	ExternalStationTransactionGuid UNIQUEIDENTIFIER,
	CreatedUpdatedBy dbo.udtUserID,
	LookupExternalStationTransactionFailedStatusIndex INT
)
