CREATE PROCEDURE [dbo].[gsp_StationsInsertByPK]
(
		@StationGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(50)=NULL
	,	@SwingArmPosition bit=NULL
	,	@VaporRecovery bit=NULL
	,	@Enabled bit=NULL
	,	@BOLPrinter nvarchar(80)=NULL
	,	@PreloadPrinter nvarchar(80)=NULL
	,	@BOLAgeInMinutes int=NULL
	,	@CardReader bit=NULL
	,	@ThirtyFiveBitCardSupport bit=NULL
	,	@NumberOfCopies int=NULL
	,	@NumberOfPreloadCopies int=NULL
	,	@InhibitLoadingByLoadID bit=NULL
	,	@InhibitOperatingModePrompt bit=NULL
	,	@SynchronizeReferenceDensity bit=NULL
	,	@SignatureDevice nvarchar(20)=NULL
	,	@SetDefaultPresetToZero bit=NULL
	,	@ArmsServiced nvarchar(100)=NULL
	,	@InhibitSettingRecipeNames bit=NULL
	,	@SignatureDevicePort int=NULL
	,	@SignatureDeviceBaudRate int=NULL
	,	@MeterRecircCardNumber nvarchar(30)=NULL
	,	@TouchKeyReader bit=NULL
	,	@OffLoadByOffLoadID bit=NULL
	,	@UseManualMeterData bit=NULL
	,	@PromptForBOLNumber bit=NULL
	,	@LastTransactionNumber int=NULL
	,	@LastTransactionNumberDateTime datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupStationTypeIndex int=NULL
	,	@LookupStationInterfaceTypeIndex int=NULL
	,	@TankGuid uniqueidentifier=NULL
	,	@IssueByVolumeTransactionAliasGuid uniqueidentifier=NULL
	,	@IssueByWeightTransactionAliasGuid uniqueidentifier=NULL
	,	@ReceiptByVolumeTransactionAliasGuid uniqueidentifier=NULL
	,	@ReceiptByWeightTransactionAliasGuid uniqueidentifier=NULL
	,	@RecircTransactionAliasGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_StationsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4822767 -05:00
	-- Purpose: Insert into table [dbo].[tblStations]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @StationGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblStations] 
		(
			[StationGuid]
		,	[ID]
		,	[SwingArmPosition]
		,	[VaporRecovery]
		,	[Enabled]
		,	[BOLPrinter]
		,	[PreloadPrinter]
		,	[BOLAgeInMinutes]
		,	[CardReader]
		,	[ThirtyFiveBitCardSupport]
		,	[NumberOfCopies]
		,	[NumberOfPreloadCopies]
		,	[InhibitLoadingByLoadID]
		,	[InhibitOperatingModePrompt]
		,	[SynchronizeReferenceDensity]
		,	[SignatureDevice]
		,	[SetDefaultPresetToZero]
		,	[ArmsServiced]
		,	[InhibitSettingRecipeNames]
		,	[SignatureDevicePort]
		,	[SignatureDeviceBaudRate]
		,	[MeterRecircCardNumber]
		,	[TouchKeyReader]
		,	[OffLoadByOffLoadID]
		,	[UseManualMeterData]
		,	[PromptForBOLNumber]
		,	[LastTransactionNumber]
		,	[LastTransactionNumberDateTime]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[LookupStationTypeIndex]
		,	[LookupStationInterfaceTypeIndex]
		,	[TankGuid]
		,	[IssueByVolumeTransactionAliasGuid]
		,	[IssueByWeightTransactionAliasGuid]
		,	[ReceiptByVolumeTransactionAliasGuid]
		,	[ReceiptByWeightTransactionAliasGuid]
		,	[RecircTransactionAliasGuid]
		)
		VALUES
		(
			@StationGuid
		,	@ID
		,	@SwingArmPosition
		,	@VaporRecovery
		,	@Enabled
		,	@BOLPrinter
		,	@PreloadPrinter
		,	@BOLAgeInMinutes
		,	@CardReader
		,	@ThirtyFiveBitCardSupport
		,	@NumberOfCopies
		,	@NumberOfPreloadCopies
		,	@InhibitLoadingByLoadID
		,	@InhibitOperatingModePrompt
		,	@SynchronizeReferenceDensity
		,	@SignatureDevice
		,	@SetDefaultPresetToZero
		,	@ArmsServiced
		,	@InhibitSettingRecipeNames
		,	@SignatureDevicePort
		,	@SignatureDeviceBaudRate
		,	@MeterRecircCardNumber
		,	@TouchKeyReader
		,	@OffLoadByOffLoadID
		,	@UseManualMeterData
		,	@PromptForBOLNumber
		,	@LastTransactionNumber
		,	@LastTransactionNumberDateTime
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@LookupStationTypeIndex
		,	@LookupStationInterfaceTypeIndex
		,	@TankGuid
		,	@IssueByVolumeTransactionAliasGuid
		,	@IssueByWeightTransactionAliasGuid
		,	@ReceiptByVolumeTransactionAliasGuid
		,	@ReceiptByWeightTransactionAliasGuid
		,	@RecircTransactionAliasGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblStations]           
		WHERE StationGuid=@StationGuid;
	
 
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
						+ 'Procedure Name: gsp_StationsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
