CREATE TYPE [dbo].[ExternalStationTransactionErrorType] AS TABLE
(
	ExternalStationTransactionErrorGuid UNIQUEIDENTIFIER NOT NULL, 
    ExternalStationTransactionGuid UNIQUEIDENTIFIER NOT NULL,  
	Error NVARCHAR(1000) NOT NULL,
	CreatedUpdatedBy dbo.udtUserID NOT NULL
)
