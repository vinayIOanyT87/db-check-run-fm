CREATE PROCEDURE [dbo].[gsp_MeterInsertByPK]
(
		@MeterGuid uniqueidentifier=NULL OUTPUT
	,	@SiteGuid uniqueidentifier=NULL
	,	@MeterID nvarchar(30)=NULL
	,	@NumberOfDigits tinyint=NULL
	,	@RotatesBackwardsFlag bit=NULL
	,	@ReceiptMeterFlag bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@DcuID nvarchar(50)=NULL
	,	@DcuBatteryVoltage float=NULL
	,	@DcuBatteryCurrent float=NULL
	,	@DcuTemperature float=NULL
	,	@DcuResets int=NULL
	,	@DcuUpdateDate datetimeoffset(7)=NULL
	,	@DcuConfigurationDate datetimeoffset(7)=NULL
	,	@DcuFirmwareVersion nvarchar(50)=NULL
	,	@DcuBluetoothAddress nvarchar(50)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MeterInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2772767 -05:00
	-- Purpose: Insert into table [dbo].[tblMeter]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MeterGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblMeter] 
		(
			[MeterGuid]
		,	[SiteGuid]
		,	[MeterID]
		,	[NumberOfDigits]
		,	[RotatesBackwardsFlag]
		,	[ReceiptMeterFlag]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[DcuID]
		,	[DcuBatteryVoltage]
		,	[DcuBatteryCurrent]
		,	[DcuTemperature]
		,	[DcuResets]
		,	[DcuUpdateDate]
		,	[DcuConfigurationDate]
		,	[DcuFirmwareVersion]
		,	[DcuBluetoothAddress]
		)
		VALUES
		(
			@MeterGuid
		,	@SiteGuid
		,	@MeterID
		,	@NumberOfDigits
		,	@RotatesBackwardsFlag
		,	@ReceiptMeterFlag
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@DcuID
		,	@DcuBatteryVoltage
		,	@DcuBatteryCurrent
		,	@DcuTemperature
		,	@DcuResets
		,	@DcuUpdateDate
		,	@DcuConfigurationDate
		,	@DcuFirmwareVersion
		,	@DcuBluetoothAddress
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblMeter]           
		WHERE MeterGuid=@MeterGuid;
	
 
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
						+ 'Procedure Name: gsp_MeterInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
