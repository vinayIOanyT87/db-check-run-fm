CREATE TYPE [dbo].[TransactionTransportLineItemsType] AS TABLE
(
	TransactionTransportLineItemGuid UNIQUEIDENTIFIER NULL,
	TransactionGuid UNIQUEIDENTIFIER NULL,
	TransportOrderNumber NVARCHAR(50) NULL,
	TransVersion BIGINT NULL,
	LocationName NVARCHAR(30) NULL,
	Address1 NVARCHAR(60) NULL,
	Address2 NVARCHAR(60) NULL,
	City NVARCHAR(60) NULL,
	[State] NVARCHAR(20) NULL,
	Zip NVARCHAR(11) NULL,
	POCName NVARCHAR(50) NULL,
	POCPhone NVARCHAR(20) NULL,
	CreatedUpdatedBy udtUserID NOT NULL
)
