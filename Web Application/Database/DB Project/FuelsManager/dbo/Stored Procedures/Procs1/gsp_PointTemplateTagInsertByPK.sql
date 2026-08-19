CREATE PROCEDURE [dbo].[gsp_PointTemplateTagInsertByPK]
(
		@PointTemplateTagGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(50)=NULL
	,	@EngineeringUnitsType int=NULL
	,	@EngineeringUnitsIndex int=NULL
	,	@DecimalPlaces tinyint=NULL
	,	@ServerEngineeringUnitsIndex int=NULL
	,	@ValueType nvarchar(max)=NULL
	,	@Value xml=NULL
	,	@Maximum float=NULL
	,	@Minimum float=NULL
	,	@PointTagInputOutputTypeIndex int=NULL
	,	@Input bit=NULL
	,	@AlarmStatus bit=NULL
	,	@ApplyPointTemplateEngineeringUnits bit=NULL
	,	@ApplyPointTemplateDecimalPlaces bit=NULL
	,	@ApplyPointTemplateMaximum bit=NULL
	,	@ApplyPointTemplateMinimum bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@PointTemplateGuid uniqueidentifier=NULL
	,	@WellKnownIdentityGuid uniqueidentifier=NULL
	,	@AlarmsEnabled bit=NULL
	,	@InhibitInputOutputTypeConfiguration BIT = NULL
	,	@InhibitOverride BIT = NULL
	,	@Module BIT = NULL
	,	@Archived BIT = NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PointTemplateTagInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-12-10 07:46:25.0717151 -05:00
	-- Purpose: Insert into table [dbo].[tblPointTemplateTag]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF @PointTemplateTagGuid IS NULL
		BEGIN
			SET @PointTemplateTagGuid=NEWID();
		END
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPointTemplateTag] 
		(
			[PointTemplateTagGuid]
		,	[ID]
		,	[EngineeringUnitsType]
		,	[EngineeringUnitsIndex]
		,	[DecimalPlaces]
		,	[ServerEngineeringUnitsIndex]
		,	[ValueType]
		,	[Value]
		,	[Maximum]
		,	[Minimum]
		,	[PointTagInputOutputTypeIndex]
		,	[Input]
		,	[AlarmStatus]
		,	[ApplyPointTemplateEngineeringUnits]
		,	[ApplyPointTemplateDecimalPlaces]
		,	[ApplyPointTemplateMaximum]
		,	[ApplyPointTemplateMinimum]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PointTemplateGuid]
		,	[WellKnownIdentityGuid]
		,	[AlarmsEnabled]
		,	[InhibitInputOutputTypeConfiguration]
		,	[InhibitOverride]
		,	[Module]
		,	[Archived]
		)
		VALUES
		(
			@PointTemplateTagGuid
		,	@ID
		,	@EngineeringUnitsType
		,	@EngineeringUnitsIndex
		,	@DecimalPlaces
		,	@ServerEngineeringUnitsIndex
		,	@ValueType
		,	@Value
		,	@Maximum
		,	@Minimum
		,	@PointTagInputOutputTypeIndex
		,	@Input
		,	@AlarmStatus
		,	@ApplyPointTemplateEngineeringUnits
		,	@ApplyPointTemplateDecimalPlaces
		,	@ApplyPointTemplateMaximum
		,	@ApplyPointTemplateMinimum
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@PointTemplateGuid
		,	@WellKnownIdentityGuid
		,  @AlarmsEnabled
		,	@InhibitInputOutputTypeConfiguration
		,	@InhibitOverride
		,	@Module
		,	@Archived
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblPointTemplateTag]           
		WHERE PointTemplateTagGuid=@PointTemplateTagGuid;
	
 
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
						+ 'Procedure Name: gsp_PointTemplateTagInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END    