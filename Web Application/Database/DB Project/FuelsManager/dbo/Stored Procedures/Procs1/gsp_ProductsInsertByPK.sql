CREATE PROCEDURE [dbo].[gsp_ProductsInsertByPK]
(
		@ProductGuid uniqueidentifier=NULL OUTPUT
	,	@ProductID nvarchar(30)=NULL
	,	@Description nvarchar(50)=NULL
	,	@GenericType nvarchar(10)=NULL
	,	@StockResetDate datetimeoffset(7)=NULL
	,	@StockTrack bit=NULL
	,	@DensityHighLimit float=NULL
	,	@DensityLowLimit float=NULL
	,	@DensityDeadband float=NULL
	,	@ApplyDensityLimits bit=NULL
	,	@TemperatureHiHiLimit float=NULL
	,	@TemperatureHighLimit float=NULL
	,	@TemperatureLowLimit float=NULL
	,	@TemperatureLoLoLimit float=NULL
	,	@TemperatureDeadband float=NULL
	,	@ApplyTemperatureLimits bit=NULL
	,	@Bonded bit=NULL
	,	@LowStockWarning float=NULL
	,	@GroundFuel bit=NULL
	,	@ProductCode nvarchar(15)=NULL
	,	@Price money=NULL
	,	@AviationFuelFlag bit=NULL
	,	@LookupMinorCorrectionMethodIndex int=NULL
	,	@CorrectionFactor0 float=NULL
	,	@CorrectionFactor1 float=NULL
	,	@CorrectionFactor2 float=NULL
	,	@CorrectionFactor3 float=NULL
	,	@CorrectionFactor4 float=NULL
	,	@StandardDensity float=NULL
	,	@StandardTemperature float=NULL
	,	@AlternateTemperature float=NULL
	,	@AlternatePressure float=NULL
	,	@ApplyVolumeCorrection bit=NULL
	,	@VolumeUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@Capitalize bit=NULL
	,	@OctaneNumber float=NULL
	,	@ReidVaporPressure float=NULL
	,	@HazardousMaterial bit=NULL
	,	@RegulatoryClass int=NULL
	,	@LoadRackDisplayText nvarchar(10)=NULL
	,	@ComponentTolerance float=NULL
	,	@VaporRecovery bit=NULL
	,	@LockedOut bit=NULL
	,	@LockedOutReason nvarchar(80)=NULL
	,	@LockedOutDate datetimeoffset(7)=NULL
	,	@VarianceTolerance float=NULL
	,	@LoadByWeight bit=NULL
	,	@PIDXCode nvarchar(3)=NULL
	,	@ContaminationPromptLoadRackText nvarchar(10)=NULL
	,	@InhibitAccounting bit=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@MassUnitIndex int=NULL
	,	@LevelUnitIndex int=NULL
	,	@FlowUnitIndex int=NULL
	,	@PressureUnitIndex int=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@LevelDecimalPlaces tinyint=NULL
	,	@FlowDecimalPlaces tinyint=NULL
	,	@PressureDecimalPlaces tinyint=NULL
	,	@VolumePackageSize float=NULL
	,	@MassPackageSize float=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupProductTypeIndex int=NULL
	,	@LookupMajorCorrectionMethodIndex int=NULL
	,	@TrackingProductGuid uniqueidentifier=NULL
	,	@TaxCode nvarchar(10)=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ProductsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.3922767 -05:00
	-- Purpose: Insert into table [dbo].[tblProducts]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ProductGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblProducts] 
		(
			[ProductGuid]
		,	[ProductID]
		,	[Description]
		,	[GenericType]
		,	[StockResetDate]
		,	[StockTrack]
		,	[DensityHighLimit]
		,	[DensityLowLimit]
		,	[DensityDeadband]
		,	[ApplyDensityLimits]
		,	[TemperatureHiHiLimit]
		,	[TemperatureHighLimit]
		,	[TemperatureLowLimit]
		,	[TemperatureLoLoLimit]
		,	[TemperatureDeadband]
		,	[ApplyTemperatureLimits]
		,	[Bonded]
		,	[LowStockWarning]
		,	[GroundFuel]
		,	[ProductCode]
		,	[Price]
		,	[AviationFuelFlag]
		,	[LookupMinorCorrectionMethodIndex]
		,	[CorrectionFactor0]
		,	[CorrectionFactor1]
		,	[CorrectionFactor2]
		,	[CorrectionFactor3]
		,	[CorrectionFactor4]
		,	[StandardDensity]
		,	[StandardTemperature]
		,	[AlternateTemperature]
		,	[AlternatePressure]
		,	[ApplyVolumeCorrection]
		,	[VolumeUnitIndex]
		,	[TemperatureUnitIndex]
		,	[DensityUnitIndex]
		,	[VolumeDecimalPlaces]
		,	[TemperatureDecimalPlaces]
		,	[DensityDecimalPlaces]
		,	[Capitalize]
		,	[OctaneNumber]
		,	[ReidVaporPressure]
		,	[HazardousMaterial]
		,	[RegulatoryClass]
		,	[LoadRackDisplayText]
		,	[ComponentTolerance]
		,	[VaporRecovery]
		,	[LockedOut]
		,	[LockedOutReason]
		,	[LockedOutDate]
		,	[VarianceTolerance]
		,	[LoadByWeight]
		,	[PIDXCode]
		,	[ContaminationPromptLoadRackText]
		,	[InhibitAccounting]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[MassUnitIndex]
		,	[LevelUnitIndex]
		,	[FlowUnitIndex]
		,	[PressureUnitIndex]
		,	[MassDecimalPlaces]
		,	[LevelDecimalPlaces]
		,	[FlowDecimalPlaces]
		,	[PressureDecimalPlaces]
		,	[VolumePackageSize]
		,	[MassPackageSize]
		,	[SiteGuid]
		,	[LookupProductTypeIndex]
		,	[LookupMajorCorrectionMethodIndex]
		,	[TrackingProductGuid]
		,	[TaxCode]
		,	[_MasterRecordGuid]
		)
		VALUES
		(
			@ProductGuid
		,	@ProductID
		,	@Description
		,	@GenericType
		,	@StockResetDate
		,	@StockTrack
		,	@DensityHighLimit
		,	@DensityLowLimit
		,	@DensityDeadband
		,	@ApplyDensityLimits
		,	@TemperatureHiHiLimit
		,	@TemperatureHighLimit
		,	@TemperatureLowLimit
		,	@TemperatureLoLoLimit
		,	@TemperatureDeadband
		,	@ApplyTemperatureLimits
		,	@Bonded
		,	@LowStockWarning
		,	@GroundFuel
		,	@ProductCode
		,	@Price
		,	@AviationFuelFlag
		,	@LookupMinorCorrectionMethodIndex
		,	@CorrectionFactor0
		,	@CorrectionFactor1
		,	@CorrectionFactor2
		,	@CorrectionFactor3
		,	@CorrectionFactor4
		,	@StandardDensity
		,	@StandardTemperature
		,	@AlternateTemperature
		,	@AlternatePressure
		,	@ApplyVolumeCorrection
		,	@VolumeUnitIndex
		,	@TemperatureUnitIndex
		,	@DensityUnitIndex
		,	@VolumeDecimalPlaces
		,	@TemperatureDecimalPlaces
		,	@DensityDecimalPlaces
		,	@Capitalize
		,	@OctaneNumber
		,	@ReidVaporPressure
		,	@HazardousMaterial
		,	@RegulatoryClass
		,	@LoadRackDisplayText
		,	@ComponentTolerance
		,	@VaporRecovery
		,	@LockedOut
		,	@LockedOutReason
		,	@LockedOutDate
		,	@VarianceTolerance
		,	@LoadByWeight
		,	@PIDXCode
		,	@ContaminationPromptLoadRackText
		,	@InhibitAccounting
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@MassUnitIndex
		,	@LevelUnitIndex
		,	@FlowUnitIndex
		,	@PressureUnitIndex
		,	@MassDecimalPlaces
		,	@LevelDecimalPlaces
		,	@FlowDecimalPlaces
		,	@PressureDecimalPlaces
		,	@VolumePackageSize
		,	@MassPackageSize
		,	@SiteGuid
		,	@LookupProductTypeIndex
		,	@LookupMajorCorrectionMethodIndex
		,	@TrackingProductGuid
		,	@TaxCode
		,	@_MasterRecordGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblProducts]           
		WHERE ProductGuid=@ProductGuid;
	
 
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
						+ 'Procedure Name: gsp_ProductsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
