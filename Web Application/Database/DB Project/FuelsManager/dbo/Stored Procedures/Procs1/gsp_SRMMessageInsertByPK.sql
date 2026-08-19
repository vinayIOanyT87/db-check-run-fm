CREATE PROCEDURE [dbo].[gsp_SRMMessageInsertByPK]
(
		@SRMMessageGuid uniqueidentifier=NULL OUTPUT
	,	@SRMAdaptorGuid uniqueidentifier=NULL
	,	@ReceiptDateTime datetimeoffset(7)=NULL
	,	@ExternalSourceIdentifier nvarchar(100)=NULL
	,	@FlightNumber nvarchar(10)=NULL
	,	@FlightOriginationDate datetimeoffset(7)=NULL
	,	@OriginIATACode nvarchar(10)=NULL
	,	@DestinationIATACode nvarchar(10)=NULL
	,	@MessageText nvarchar(max)=NULL
	,	@ConvertedMessageXML nvarchar(max)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@AirlineIATACode nvarchar(10)=NULL
	,	@TimesLegFlown nvarchar(10)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SRMMessageInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4752767 -05:00
	-- Purpose: Insert into table [dbo].[tblSRMMessage]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SRMMessageGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSRMMessage] 
		(
			[SRMMessageGuid]
		,	[SRMAdaptorGuid]
		,	[ReceiptDateTime]
		,	[ExternalSourceIdentifier]
		,	[FlightNumber]
		,	[FlightOriginationDate]
		,	[OriginIATACode]
		,	[DestinationIATACode]
		,	[MessageText]
		,	[ConvertedMessageXML]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[AirlineIATACode]
		,	[TimesLegFlown]
		)
		VALUES
		(
			@SRMMessageGuid
		,	@SRMAdaptorGuid
		,	@ReceiptDateTime
		,	@ExternalSourceIdentifier
		,	@FlightNumber
		,	@FlightOriginationDate
		,	@OriginIATACode
		,	@DestinationIATACode
		,	@MessageText
		,	@ConvertedMessageXML
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@AirlineIATACode
		,	@TimesLegFlown
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSRMMessage]           
		WHERE SRMMessageGuid=@SRMMessageGuid;
	
 
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: gsp_SRMMessageInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
