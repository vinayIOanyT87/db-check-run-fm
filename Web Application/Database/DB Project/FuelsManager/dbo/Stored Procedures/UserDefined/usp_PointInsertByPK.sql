CREATE PROCEDURE [dbo].[usp_PointInsertByPK]
(
		@PointGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(30)=NULL
	,	@Enabled bit=0
	,	@Description nvarchar(50)=NULL
	,	@Standard bit=NULL
	,	@ExecutionInterval int=NULL
	,	@LevelUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@PressureUnitIndex int=NULL
	,	@FlowUnitIndex int=NULL
	,	@VolumeUnitIndex int=NULL
	,	@MassUnitIndex int=NULL
	,	@VelocityUnitIndex int=NULL
	,	@MassFlowUnitIndex int=NULL
	,	@LevelDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@PressureDecimalPlaces tinyint=NULL
	,	@FlowDecimalPlaces tinyint=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@VelocityDecimalPlaces tinyint=NULL
	,	@MassFlowDecimalPlaces tinyint=NULL
	,	@LevelMaximum float=NULL
	,	@LevelMinimum float=NULL
	,	@TemperatureMaximum float=NULL
	,	@TemperatureMinimum float=NULL
	,	@DensityMaximum float=NULL
	,	@DensityMinimum float=NULL
	,	@PressureMaximum float=NULL
	,	@PressureMinimum float=NULL
	,	@VolumetricFlowMaximum float=NULL
	,	@VolumetricFlowMinimum float=NULL
	,	@VolumeMaximum float=NULL
	,	@VolumeMinimum float=NULL
	,	@MassMaximum float=NULL
	,	@MassMinimum float=NULL
	,	@VelocityMaximum float=NULL
	,	@VelocityMinimum float=NULL
	,	@MassFlowMaximum float=NULL
	,	@MassFlowMinimum float=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@PointTemplateGuid uniqueidentifier=NULL
	,	@ProfileImageGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@Notes nvarchar(255)=null
	,  @OverrideDefaultDrawingGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PointInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-11-24 10:20:48.9719729 -05:00
	-- Purpose: Insert into table [dbo].[tblPoint]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		IF @PointGuid is null
		SET @PointGuid=NEWID();

		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPoint] 
		(
			[PointGuid]
		,	[ID]
		,   [Enabled]
		,	[Description]
		,	[Standard]
		,	[ExecutionInterval]
		,	[LevelUnitIndex]
		,	[TemperatureUnitIndex]
		,	[DensityUnitIndex]
		,	[PressureUnitIndex]
		,	[FlowUnitIndex]
		,	[VolumeUnitIndex]
		,	[MassUnitIndex]
		,	[VelocityUnitIndex]
		,	[MassFlowUnitIndex]
		,	[LevelDecimalPlaces]
		,	[TemperatureDecimalPlaces]
		,	[DensityDecimalPlaces]
		,	[PressureDecimalPlaces]
		,	[FlowDecimalPlaces]
		,	[VolumeDecimalPlaces]
		,	[MassDecimalPlaces]
		,	[VelocityDecimalPlaces]
		,	[MassFlowDecimalPlaces]
		,	[LevelMaximum]
		,	[LevelMinimum]
		,	[TemperatureMaximum]
		,	[TemperatureMinimum]
		,	[DensityMaximum]
		,	[DensityMinimum]
		,	[PressureMaximum]
		,	[PressureMinimum]
		,	[VolumetricFlowMaximum]
		,	[VolumetricFlowMinimum]
		,	[VolumeMaximum]
		,	[VolumeMinimum]
		,	[MassMaximum]
		,	[MassMinimum]
		,	[VelocityMaximum]
		,	[VelocityMinimum]
		,	[MassFlowMaximum]
		,	[MassFlowMinimum]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[PointTemplateGuid]
		,	[ProfileImageGuid]
		,	[ProductGuid]
		,	[Notes]
		,  [OverrideDefaultDrawingGuid]
		,	[PointTemplateVersion]
		)
		VALUES
		(
			@PointGuid
		,	@ID
		,	@Enabled
		,	@Description
		,	@Standard
		,	@ExecutionInterval
		,	@LevelUnitIndex
		,	@TemperatureUnitIndex
		,	@DensityUnitIndex
		,	@PressureUnitIndex
		,	@FlowUnitIndex
		,	@VolumeUnitIndex
		,	@MassUnitIndex
		,	@VelocityUnitIndex
		,	@MassFlowUnitIndex
		,	@LevelDecimalPlaces
		,	@TemperatureDecimalPlaces
		,	@DensityDecimalPlaces
		,	@PressureDecimalPlaces
		,	@FlowDecimalPlaces
		,	@VolumeDecimalPlaces
		,	@MassDecimalPlaces
		,	@VelocityDecimalPlaces
		,	@MassFlowDecimalPlaces
		,	@LevelMaximum
		,	@LevelMinimum
		,	@TemperatureMaximum
		,	@TemperatureMinimum
		,	@DensityMaximum
		,	@DensityMinimum
		,	@PressureMaximum
		,	@PressureMinimum
		,	@VolumetricFlowMaximum
		,	@VolumetricFlowMinimum
		,	@VolumeMaximum
		,	@VolumeMinimum
		,	@MassMaximum
		,	@MassMinimum
		,	@VelocityMaximum
		,	@VelocityMinimum
		,	@MassFlowMaximum
		,	@MassFlowMinimum
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@PointTemplateGuid
		,	@ProfileImageGuid
		,	@ProductGuid
		,	@Notes
		,  @OverrideDefaultDrawingGuid
		,	(SELECT [Version] FROM dbo.tblPointTemplate WHERE PointTemplateGuid = @PointTemplateGuid)
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblPoint]           
		WHERE PointGuid=@PointGuid;
	
 
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
						+ 'Procedure Name: gsp_PointInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END