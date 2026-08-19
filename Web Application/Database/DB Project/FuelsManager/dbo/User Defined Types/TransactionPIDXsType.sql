CREATE TYPE [dbo].[TransactionPIDXsType] AS TABLE
(
	TransactionPIDXGuid UNIQUEIDENTIFIER NULL,
	TransactionGuid UNIQUEIDENTIFIER NULL,
	AuthorizationNumber NVARCHAR(8) NULL,
	SentFlag BIT NULL,
	DateSent DATETIMEOFFSET(7) NULL,
	BrokenBlend BIT NULL,
	PIDXProfileGuid UNIQUEIDENTIFIER NULL,
	CompanyPersonnelToShipToBillToGuid UNIQUEIDENTIFIER NULL,
	CreatedUpdatedBy udtUserID NOT NULL,
	BOLVersion INT NULL
)
