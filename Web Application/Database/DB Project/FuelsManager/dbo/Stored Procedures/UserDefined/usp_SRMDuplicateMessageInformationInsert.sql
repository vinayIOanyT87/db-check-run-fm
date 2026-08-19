

/*
=============================================
Author: Ryan Hill
Create date: 9/7/12
Description:

Create or update Service Request Messaging Duplicate Message record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMDuplicateMessageInformationInsert]
(
	@MessageSequenceNumber NVARCHAR(100), 
	@FlightNumber NVARCHAR(10),
	@FlightOriginationDate DATETIMEOFFSET(7),
	@OriginIATACode NVARCHAR(4),
	@DestinationIATACode NVARCHAR(10),
	@AirlineIATACode NVARCHAR(10),
	@TimesLegFlown NVARCHAR(10),
	@HashValue NVARCHAR(32),
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMDuplicateMessageInformationGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	
	SET @SRMDuplicateMessageInformationGuid = NEWID()

	-- We use the SQL Server MERGE command to either insert or update the
	-- duplicate message information. Using IF EXISTS (Update) ELSE (Insert) is not
	-- thread safe.
	-- According to this article, we must specify the HOLDLOCK hint to avoid any potential concurrency issues
	-- http://weblogs.sqlteam.com/dang/archive/2009/01/31/UPSERT-Race-Condition-With-MERGE.aspx
	MERGE tblSRMDuplicateMessageInformation WITH(HOLDLOCK) AS target
    USING (SELECT @FlightNumber AS FlightNumber, 
		@FlightOriginationDate AS FlightOriginationDate,  
		@OriginIATACode AS OriginIATACode, 
		@DestinationIATACode AS DestinationIATACode, 
		@AirlineIATACode AS AirlineIATACode, 
		@TimesLegFlown AS TimesLegFlown) AS source
		ON source.FlightNumber = target.FlightNumber
			AND source.FlightOriginationDate = target.FlightOriginationDate
			AND source.OriginIATACode = target.OriginIATACode
			AND source.DestinationIATACode = target.DestinationIATACode
			AND source.AirlineIATACode = target.AirlineIATACode
			AND source.TimesLegFlown = target.TimesLegFlown
    WHEN MATCHED THEN 
		UPDATE 
		SET
			MessageSequenceNumber = @MessageSequenceNumber, 
			FlightNumber = @FlightNumber,
			FlightOriginationDate = @FlightOriginationDate,
			OriginIATACode = @OriginIATACode,
			DestinationIATACode = @DestinationIATACode,
			AirlineIATACode = @AirlineIATACode,
			TimesLegFlown = @TimesLegFlown,
			HashValue = @HashValue,
			CreatedDate = @CreatedDate,
			CreatedBy = @CreatedBy,
			UpdatedDate = @UpdatedDate,
			UpdatedBy = @UpdatedBy
	WHEN NOT MATCHED THEN	
		INSERT  
		(
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
		)
		VALUES
		(
			@SRMDuplicateMessageInformationGuid,
			@MessageSequenceNumber, 
			@FlightNumber,
			@FlightOriginationDate,
			@OriginIATACode,
			@DestinationIATACode,
			@AirlineIATACode,
			@TimesLegFlown,
			@HashValue,
			@CreatedDate,
			@CreatedBy,
			@UpdatedDate,
			@UpdatedBy
		);
END