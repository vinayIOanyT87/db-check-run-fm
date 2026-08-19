CREATE PROCEDURE [dbo].[gsp_EquipmentInsertByPK]
(
		@EquipmentGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(30)=NULL
	,	@Description nvarchar(50)=NULL
	,	@Make nvarchar(20)=NULL
	,	@Model nvarchar(50)=NULL
	,	@Year int=NULL
	,	@IssPtNum nvarchar(20)=NULL
	,	@Fixed bit=NULL
	,	@StorageType nvarchar(2)=NULL
	,	@InUse bit=NULL
	,	@FixedVolume bit=NULL
	,	@IntoPlane bit=NULL
	,	@Mobile bit=NULL
	,	@AttachedTo nvarchar(6)=NULL
	,	@MediaType char=NULL
	,	@Meters int=NULL
	,	@DefuelMeterForwards bit=NULL
	,	@PulseRatio float=NULL
	,	@Round bit=NULL
	,	@Xref nvarchar(10)=NULL
	,	@LowStockWarning float=NULL
	,	@StockTrack bit=NULL
	,	@Totalisor1 nvarchar(10)=NULL
	,	@Totalisor2 nvarchar(10)=NULL
	,	@FuelingState nvarchar(10)=NULL
	,	@Volume float=NULL
	,	@MeterReading float=NULL
	,	@Consecutive_OOS_Variance int=NULL
	,	@Notes nvarchar(1000)=NULL
	,	@Capacity float=NULL
	,	@SafeFill float=NULL
	,	@VolumeUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@MassUnitIndex int=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@EquipmentSequence nvarchar(50)=NULL
	,	@LockedOut bit=NULL
	,	@LockedOutReason nvarchar(80)=NULL
	,	@LockedOutDate datetimeoffset(7)=NULL
	,	@SerialNumber nvarchar(30)=NULL
	,	@CompanyEquipmentID nvarchar(30)=NULL
	,	@TruckCardNumber nvarchar(32)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@RatedGPM float=NULL
	,	@ActualGPM float=NULL
	,	@FuelAdditiveFlag bit=NULL
	,	@ManufactureDate datetimeoffset(7)=NULL
	,	@InstallationDate datetimeoffset(7)=NULL
	,	@InspectionDate datetimeoffset(7)=NULL
	,	@CalibrationDate datetimeoffset(7)=NULL
	,	@QCDate datetimeoffset(7)=NULL
	,	@SecondaryStorageFlag bit=NULL
	,	@ManagedEquipmentFlag bit=NULL
	,	@FuelingType smallint=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@UserData9 nvarchar(60)=NULL
	,	@UserData10 nvarchar(60)=NULL
	,	@UserData11 nvarchar(60)=NULL
	,	@UserData12 nvarchar(60)=NULL
	,	@UserData13 nvarchar(60)=NULL
	,	@UserData14 nvarchar(60)=NULL
	,	@UserData15 nvarchar(60)=NULL
	,	@UserData16 nvarchar(60)=NULL
	,	@UserData17 nvarchar(60)=NULL
	,	@UserData18 nvarchar(60)=NULL
	,	@UserData19 nvarchar(60)=NULL
	,	@UserData20 nvarchar(60)=NULL
	,	@UserData21 nvarchar(60)=NULL
	,	@UserData22 nvarchar(60)=NULL
	,	@UserData23 nvarchar(60)=NULL
	,	@UserData24 nvarchar(60)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@CompanyGuid uniqueidentifier=NULL
	,	@ParentEquipmentGuid uniqueidentifier=NULL
	,	@EquipmentTypeGuid uniqueidentifier=NULL
	,	@FuelCardGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@AssignedToMeterGuid uniqueidentifier=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_EquipmentInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1432767 -05:00
	-- Purpose: Insert into table [dbo].[tblEquipment]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @EquipmentGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblEquipment] 
		(
			[EquipmentGuid]
		,	[ID]
		,	[Description]
		,	[Make]
		,	[Model]
		,	[Year]
		,	[IssPtNum]
		,	[Fixed]
		,	[StorageType]
		,	[InUse]
		,	[FixedVolume]
		,	[IntoPlane]
		,	[Mobile]
		,	[AttachedTo]
		,	[MediaType]
		,	[Meters]
		,	[DefuelMeterForwards]
		,	[PulseRatio]
		,	[Round]
		,	[Xref]
		,	[LowStockWarning]
		,	[StockTrack]
		,	[Totalisor1]
		,	[Totalisor2]
		,	[FuelingState]
		,	[Volume]
		,	[MeterReading]
		,	[Consecutive_OOS_Variance]
		,	[Notes]
		,	[Capacity]
		,	[SafeFill]
		,	[VolumeUnitIndex]
		,	[TemperatureUnitIndex]
		,	[DensityUnitIndex]
		,	[MassUnitIndex]
		,	[VolumeDecimalPlaces]
		,	[TemperatureDecimalPlaces]
		,	[DensityDecimalPlaces]
		,	[MassDecimalPlaces]
		,	[EquipmentSequence]
		,	[LockedOut]
		,	[LockedOutReason]
		,	[LockedOutDate]
		,	[SerialNumber]
		,	[CompanyEquipmentID]
		,	[TruckCardNumber]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[RatedGPM]
		,	[ActualGPM]
		,	[FuelAdditiveFlag]
		,	[ManufactureDate]
		,	[InstallationDate]
		,	[InspectionDate]
		,	[CalibrationDate]
		,	[QCDate]
		,	[SecondaryStorageFlag]
		,	[ManagedEquipmentFlag]
		,	[FuelingType]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[UserData9]
		,	[UserData10]
		,	[UserData11]
		,	[UserData12]
		,	[UserData13]
		,	[UserData14]
		,	[UserData15]
		,	[UserData16]
		,	[UserData17]
		,	[UserData18]
		,	[UserData19]
		,	[UserData20]
		,	[UserData21]
		,	[UserData22]
		,	[UserData23]
		,	[UserData24]
		,	[SiteGuid]
		,	[CompanyGuid]
		,	[ParentEquipmentGuid]
		,	[EquipmentTypeGuid]
		,	[FuelCardGuid]
		,	[ProductGuid]
		,	[AssignedToMeterGuid]
		,	[_MasterRecordGuid]
		)
		VALUES
		(
			@EquipmentGuid
		,	@ID
		,	@Description
		,	@Make
		,	@Model
		,	@Year
		,	@IssPtNum
		,	@Fixed
		,	@StorageType
		,	@InUse
		,	@FixedVolume
		,	@IntoPlane
		,	@Mobile
		,	@AttachedTo
		,	@MediaType
		,	@Meters
		,	@DefuelMeterForwards
		,	@PulseRatio
		,	@Round
		,	@Xref
		,	@LowStockWarning
		,	@StockTrack
		,	@Totalisor1
		,	@Totalisor2
		,	@FuelingState
		,	@Volume
		,	@MeterReading
		,	@Consecutive_OOS_Variance
		,	@Notes
		,	@Capacity
		,	@SafeFill
		,	@VolumeUnitIndex
		,	@TemperatureUnitIndex
		,	@DensityUnitIndex
		,	@MassUnitIndex
		,	@VolumeDecimalPlaces
		,	@TemperatureDecimalPlaces
		,	@DensityDecimalPlaces
		,	@MassDecimalPlaces
		,	@EquipmentSequence
		,	@LockedOut
		,	@LockedOutReason
		,	@LockedOutDate
		,	@SerialNumber
		,	@CompanyEquipmentID
		,	@TruckCardNumber
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@RatedGPM
		,	@ActualGPM
		,	@FuelAdditiveFlag
		,	@ManufactureDate
		,	@InstallationDate
		,	@InspectionDate
		,	@CalibrationDate
		,	@QCDate
		,	@SecondaryStorageFlag
		,	@ManagedEquipmentFlag
		,	@FuelingType
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@UserData9
		,	@UserData10
		,	@UserData11
		,	@UserData12
		,	@UserData13
		,	@UserData14
		,	@UserData15
		,	@UserData16
		,	@UserData17
		,	@UserData18
		,	@UserData19
		,	@UserData20
		,	@UserData21
		,	@UserData22
		,	@UserData23
		,	@UserData24
		,	@SiteGuid
		,	@CompanyGuid
		,	@ParentEquipmentGuid
		,	@EquipmentTypeGuid
		,	@FuelCardGuid
		,	@ProductGuid
		,	@AssignedToMeterGuid
		,	@_MasterRecordGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblEquipment]           
		WHERE EquipmentGuid=@EquipmentGuid;
	
 
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
						+ 'Procedure Name: gsp_EquipmentInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
