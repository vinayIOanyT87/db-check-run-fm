CREATE PROCEDURE [dbo].[gsp_EquipmentTypesInsertByPK]
(
		@EquipmentTypeGuid uniqueidentifier=NULL OUTPUT
	,	@EqTypeName nvarchar(50)=NULL
	,	@EqTypeDescription nvarchar(50)=NULL
	,	@Capacity float=NULL
	,	@SafeFill float=NULL
	,	@Make nvarchar(20)=NULL
	,	@Model nvarchar(32)=NULL
	,	@Year smallint=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@DeleteFlag bit=NULL
	,	@IssPt nvarchar(20)=NULL
	,	@MultiCompartment bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupEquipmentTypeIndex int=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@CustomerDesignator nvarchar(128)=NULL
	,	@ServiceTime float=NULL
	,	@VolumeUnits int=NULL
	,	@VolumeDecimalPlaces smallint=NULL
	,	@MassUnits int=NULL
	,	@MassDecimalPlaces smallint=NULL
	,	@WingToWingToleranceType smallint=NULL
	,	@WingToWingToleranceValue float=NULL
	,	@TankToTankToleranceType smallint=NULL
	,	@TankToTankToleranceValue float=NULL
	,	@FuelServiceToleranceType smallint=NULL
	,	@FuelServiceToleranceValue float=NULL
	,	@FuelServiceToleranceMaxType smallint=NULL
	,	@FuelServiceToleranceMaxValue float=NULL
	,	@AllowFuelingByWeight bit=NULL
	,	@LookupCompanyRoleIndex int=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_EquipmentTypesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1592767 -05:00
	-- Purpose: Insert into table [dbo].[tblEquipmentTypes]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @EquipmentTypeGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblEquipmentTypes] 
		(
			[EquipmentTypeGuid]
		,	[EqTypeName]
		,	[EqTypeDescription]
		,	[Capacity]
		,	[SafeFill]
		,	[Make]
		,	[Model]
		,	[Year]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[DeleteFlag]
		,	[IssPt]
		,	[MultiCompartment]
		,	[SiteGuid]
		,	[LookupEquipmentTypeIndex]
		,	[ProductGuid]
		,	[CustomerDesignator]
		,	[ServiceTime]
		,	[VolumeUnits]
		,	[VolumeDecimalPlaces]
		,	[MassUnits]
		,	[MassDecimalPlaces]
		,	[WingToWingToleranceType]
		,	[WingToWingToleranceValue]
		,	[TankToTankToleranceType]
		,	[TankToTankToleranceValue]
		,	[FuelServiceToleranceType]
		,	[FuelServiceToleranceValue]
		,	[FuelServiceToleranceMaxType]
		,	[FuelServiceToleranceMaxValue]
		,	[AllowFuelingByWeight]
		,	[LookupCompanyRoleIndex]
		)
		VALUES
		(
			@EquipmentTypeGuid
		,	@EqTypeName
		,	@EqTypeDescription
		,	@Capacity
		,	@SafeFill
		,	@Make
		,	@Model
		,	@Year
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@DeleteFlag
		,	@IssPt
		,	@MultiCompartment
		,	@SiteGuid
		,	@LookupEquipmentTypeIndex
		,	@ProductGuid
		,	@CustomerDesignator
		,	@ServiceTime
		,	@VolumeUnits
		,	@VolumeDecimalPlaces
		,	@MassUnits
		,	@MassDecimalPlaces
		,	@WingToWingToleranceType
		,	@WingToWingToleranceValue
		,	@TankToTankToleranceType
		,	@TankToTankToleranceValue
		,	@FuelServiceToleranceType
		,	@FuelServiceToleranceValue
		,	@FuelServiceToleranceMaxType
		,	@FuelServiceToleranceMaxValue
		,	@AllowFuelingByWeight
		,	@LookupCompanyRoleIndex
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblEquipmentTypes]           
		WHERE EquipmentTypeGuid=@EquipmentTypeGuid;
	
 
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
						+ 'Procedure Name: gsp_EquipmentTypesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
