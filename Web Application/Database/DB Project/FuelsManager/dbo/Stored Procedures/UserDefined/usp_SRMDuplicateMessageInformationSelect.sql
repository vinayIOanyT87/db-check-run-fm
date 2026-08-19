
/*
=============================================
Author: Ryan Hill
Create date: 9/7/12
Description:

Get SRM duplicate message information that matches key fields and is not older than a provided number of hours,
Or get all duplicate message information that is not older than a provided number of hours
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMDuplicateMessageInformationSelect]
(
	@HoursOld INT = NULL,
	@FlightNumber NVARCHAR(10) = NULL,
	@FlightOriginationDate DATETIMEOFFSET(7) = NULL,
	@OriginIATACode NVARCHAR(10) = NULL,
	@DestinationIATACode NVARCHAR(10) = NULL,
	@AirlineIATACode NVARCHAR(10) = NULL,
	@TimesLegFlown NVARCHAR(10) = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	IF(@FlightNumber IS NOT NULL)
	BEGIN
		SELECT
			SRMDuplicateMessageInformationGuid,
			MessageSequenceNumber, 
			FlightNumber,
			FlightOriginationDate,
			OriginIATACode,
			DestinationIATACode,
			AirlineIATACode,
			TimesLegFlown,
			HashValue,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMDuplicateMessageInformation WITH(NOLOCK)
		WHERE FlightNumber = @FlightNumber
			AND FlightOriginationDate = @FlightOriginationDate
			AND OriginIATACode = @OriginIATACode
			AND DestinationIATACode = @DestinationIATACode
			AND AirlineIATACode = @AirlineIATACode
			AND TimesLegFlown = @TimesLegFlown
	END
	ELSE IF (@HoursOld IS NOT NULL)
	BEGIN
		SET @HoursOld = @HoursOld * -1

		SELECT	
			SRMDuplicateMessageInformationGuid,
			MessageSequenceNumber, 
			FlightNumber,
			FlightOriginationDate,
			OriginIATACode,
			DestinationIATACode,
			AirlineIATACode,
			TimesLegFlown,
			HashValue,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMDuplicateMessageInformation WITH(NOLOCK)
		WHERE CreatedDate >= DATEADD(HOUR, @HoursOld, SYSDATETIMEOFFSET())
	END
END