

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Create a Service Request Messaging Archived Message record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMMessageInsert]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER,
	@ReceiptDateTime DATETIMEOFFSET(7),
	@ExternalSourceIdentifier NVARCHAR(100),
	@FlightNumber NVARCHAR(10) = NULL,
	@FlightOriginationDate DATETIMEOFFSET(7) = NULL,
	@OriginIATACode NVARCHAR(10) = NULL,
	@DestinationIATACode NVARCHAR(10) = NULL,
	@AirlineIATACode NVARCHAR(10) = NULL,
	@TimesLegFlown NVARCHAR(10) = NULL,
	@MessageText NVARCHAR(MAX),
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMMessageGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMMessageGuid = NEWID()

	INSERT INTO tblSRMMessage
	(
		SRMMessageGuid,
		SRMAdaptorGuid,
		ReceiptDateTime,
		ExternalSourceIdentifier,
		FlightNumber,
		FlightOriginationDate,
		OriginIATACode,
		DestinationIATACode,
		AirlineIATACode,
		TimesLegFlown,
		MessageText,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@SRMMessageGuid,
		@SRMAdaptorGuid,
		@ReceiptDateTime,
		@ExternalSourceIdentifier,
		@FlightNumber,
		@FlightOriginationDate,
		@OriginIATACode,
		@DestinationIATACode,
		@AirlineIATACode,
		@TimesLegFlown,
		@MessageText,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END