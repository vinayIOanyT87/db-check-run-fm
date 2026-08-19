CREATE PROCEDURE [dbo].[gsp_TransactionAliasesInsertByPK]
(
		@TransactionAliasGuid uniqueidentifier=NULL OUTPUT
	,	@AliasName nvarchar(32)=NULL
	,	@MeterCloseout bit=NULL
	,	@BulkShipment bit=NULL
	,	@DistributedImpact bit=NULL
	,	@MultipleLineItems bit=NULL
	,	@LimitSelectionsBasedOnHierarchy bit=NULL
	,	@LineItemEditControl bit=NULL
	,	@MultipleWeightReadings bit=NULL
	,	@WeightReadingEditControl bit=NULL
	,	@AssociatedReport nvarchar(80)=NULL
	,	@AssociatedPreloadReport nvarchar(80)=NULL
	,	@DestinationEquipmentTypes1 bigint=NULL
	,	@DestinationEquipmentTypes2 bigint=NULL
	,	@DestinationEquipmentTypes3 bigint=NULL
	,	@SourceEquipmentTypes1 bigint=NULL
	,	@SourceEquipmentTypes2 bigint=NULL
	,	@SourceEquipmentTypes3 bigint=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@ShowCompanyName smallint=NULL
	,	@AggregateAssocTrans bit=NULL
	,	@EnableTotalQuantityExceededWarning bit=NULL
	,	@EnableQuantityToleranceExceededWarning bit=NULL
	,	@EnableTotalValueExceededWarning bit=NULL
	,	@EnableValueToleranceExceededWarning bit=NULL
	,	@LevelUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@PressureUnitIndex int=NULL
	,	@FlowUnitIndex int=NULL
	,	@VolumeUnitIndex int=NULL
	,	@MassUnitIndex int=NULL
	,	@AdditiveVolumeUnitIndex int=NULL
	,	@AdditiveProfileCycleAmountUnitIndex int=NULL
	,	@AdditiveProfileRateUnitIndex int=NULL
	,	@LevelDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@PressureDecimalPlaces tinyint=NULL
	,	@FlowDecimalPlaces tinyint=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@AdditiveVolumeDecimalPlaces tinyint=NULL
	,	@UseComboBoxControls bit=NULL
	,	@MultipleTransportLineItems bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupTransTypeIndex smallint=NULL
	,	@LookupDefaultStatusIndex int=NULL
	,	@AssociatedTransactionAliasGuid uniqueidentifier=NULL
	,	@IncludeInDispatch bit=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@EnableAutoCompleteControls bit=NULL
	,	@PermitNonReferenceData bit=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionAliasesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5172767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionAliases]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionAliasGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionAliases] 
		(
			[TransactionAliasGuid]
		,	[AliasName]
		,	[MeterCloseout]
		,	[BulkShipment]
		,	[DistributedImpact]
		,	[MultipleLineItems]
		,	[LimitSelectionsBasedOnHierarchy]
		,	[LineItemEditControl]
		,	[MultipleWeightReadings]
		,	[WeightReadingEditControl]
		,	[AssociatedReport]
		,	[AssociatedPreloadReport]
		,	[DestinationEquipmentTypes1]
		,	[DestinationEquipmentTypes2]
		,	[DestinationEquipmentTypes3]
		,	[SourceEquipmentTypes1]
		,	[SourceEquipmentTypes2]
		,	[SourceEquipmentTypes3]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[ShowCompanyName]
		,	[AggregateAssocTrans]
		,	[EnableTotalQuantityExceededWarning]
		,	[EnableQuantityToleranceExceededWarning]
		,	[EnableTotalValueExceededWarning]
		,	[EnableValueToleranceExceededWarning]
		,	[LevelUnitIndex]
		,	[TemperatureUnitIndex]
		,	[DensityUnitIndex]
		,	[PressureUnitIndex]
		,	[FlowUnitIndex]
		,	[VolumeUnitIndex]
		,	[MassUnitIndex]
		,	[AdditiveVolumeUnitIndex]
		,	[AdditiveProfileCycleAmountUnitIndex]
		,	[AdditiveProfileRateUnitIndex]
		,	[LevelDecimalPlaces]
		,	[TemperatureDecimalPlaces]
		,	[DensityDecimalPlaces]
		,	[PressureDecimalPlaces]
		,	[FlowDecimalPlaces]
		,	[VolumeDecimalPlaces]
		,	[MassDecimalPlaces]
		,	[AdditiveVolumeDecimalPlaces]
		,	[UseComboBoxControls]
		,	[MultipleTransportLineItems]
		,	[SiteGuid]
		,	[LookupTransTypeIndex]
		,	[LookupDefaultStatusIndex]
		,	[AssociatedTransactionAliasGuid]
		,	[IncludeInDispatch]
		,	[_MasterRecordGuid]
		,	[EnableAutoCompleteControls]
		,	[PermitNonReferenceData]
		)
		VALUES
		(
			@TransactionAliasGuid
		,	@AliasName
		,	@MeterCloseout
		,	@BulkShipment
		,	@DistributedImpact
		,	@MultipleLineItems
		,	@LimitSelectionsBasedOnHierarchy
		,	@LineItemEditControl
		,	@MultipleWeightReadings
		,	@WeightReadingEditControl
		,	@AssociatedReport
		,	@AssociatedPreloadReport
		,	@DestinationEquipmentTypes1
		,	@DestinationEquipmentTypes2
		,	@DestinationEquipmentTypes3
		,	@SourceEquipmentTypes1
		,	@SourceEquipmentTypes2
		,	@SourceEquipmentTypes3
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@ShowCompanyName
		,	@AggregateAssocTrans
		,	@EnableTotalQuantityExceededWarning
		,	@EnableQuantityToleranceExceededWarning
		,	@EnableTotalValueExceededWarning
		,	@EnableValueToleranceExceededWarning
		,	@LevelUnitIndex
		,	@TemperatureUnitIndex
		,	@DensityUnitIndex
		,	@PressureUnitIndex
		,	@FlowUnitIndex
		,	@VolumeUnitIndex
		,	@MassUnitIndex
		,	@AdditiveVolumeUnitIndex
		,	@AdditiveProfileCycleAmountUnitIndex
		,	@AdditiveProfileRateUnitIndex
		,	@LevelDecimalPlaces
		,	@TemperatureDecimalPlaces
		,	@DensityDecimalPlaces
		,	@PressureDecimalPlaces
		,	@FlowDecimalPlaces
		,	@VolumeDecimalPlaces
		,	@MassDecimalPlaces
		,	@AdditiveVolumeDecimalPlaces
		,	@UseComboBoxControls
		,	@MultipleTransportLineItems
		,	@SiteGuid
		,	@LookupTransTypeIndex
		,	@LookupDefaultStatusIndex
		,	@AssociatedTransactionAliasGuid
		,	@IncludeInDispatch
		,	@_MasterRecordGuid
		,	@EnableAutoCompleteControls
		,	@PermitNonReferenceData
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionAliases]           
		WHERE TransactionAliasGuid=@TransactionAliasGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionAliasesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
