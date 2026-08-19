CREATE PROCEDURE [dbo].[gsp_PointTagInsertByPK]
(
		@PointTagGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(50)=NULL
	,	@EngineeringUnitsType int=NULL
	,	@EngineeringUnitsIndex int=NULL
	,	@DecimalPlaces tinyint=NULL
	,	@ServerEngineeringUnitsIndex int=NULL
	,	@ValueType nvarchar(max)=NULL
	,	@Status bigint=NULL
	,	@Value xml=NULL
	,	@ServerTimeStamp datetimeoffset(7)=NULL
	,	@SourceTimeStamp datetimeoffset(7)=NULL
	,	@Maximum float=NULL
	,	@Minimum float=NULL
	,	@PointTagInputOutputTypeIndex int=NULL
	,	@Input bit=NULL
	,	@AlarmStatus bit=NULL
	,	@ApplyPointEngineeringUnits bit=NULL
	,	@ApplyPointDecimalPlaces bit=NULL
	,	@ApplyPointMaximum bit=NULL
	,	@ApplyPointMinimum bit=NULL
	,	@OpcUaServerGuid UNIQUEIDENTIFIER = NULL
	,	@OpcUaBrowsePath NVARCHAR (250)	= NULL
	,	@OpcUaNamespaceUri NVARCHAR (250) = NULL
	,	@OpcUaPublishingInterval INT = 0
	,	@OpcUaNodeId NVARCHAR (250)	= NULL
	,	@OpcUaIsReadable BIT = 0
	,	@OpcUaServerDataType INT = NULL
	,	@OpcUaWriteHoldoffTime INT = NULL
	,	@OpcUaWritePeriodicUpdateInterval INT = NULL
	,	@AlarmsEnabled BIT = NULL
	,	@InhibitInputOutputTypeConfiguration BIT = NULL
	,	@InhibitOverride BIT = NULL
	,	@Deadband FLOAT = NULL
	,	@Holdoff INT = NULL
	,	@Archived BIT = NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@PointGuid uniqueidentifier=NULL
	,	@PointTemplateTagGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PointTagInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-12-10 07:45:15.4337520 -05:00
	-- Purpose: Insert into table [dbo].[tblPointTag]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @PointTagGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPointTag] 
		(
			[PointTagGuid]
		,	[ID]
		,	[EngineeringUnitsType]
		,	[EngineeringUnitsIndex]
		,	[DecimalPlaces]
		,	[ServerEngineeringUnitsIndex]
		,	[ValueType]
		,	[Status]
		,	[Value]
		,	[ServerTimeStamp]
		,	[SourceTimeStamp]
		,	[Maximum]
		,	[Minimum]
		,	[PointTagInputOutputTypeIndex]
		,	[Input]
		,	[AlarmStatus]
		,	[ApplyPointEngineeringUnits]
		,	[ApplyPointDecimalPlaces]
		,	[ApplyPointMaximum]
		,	[ApplyPointMinimum]
		,	[OpcUaServerGuid]
		,	[OpcUaBrowsePath]
		,	[OpcUaNamespaceUri]
		,	[OpcUaPublishingInterval]
		,	[OpcUaNodeId]
		,	[OpcUaIsReadable]
		,	[OpcUaServerDataType]
		,	[OpcUaWriteHoldoffTime]
		,	[OpcUaWritePeriodicUpdateInterval]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PointGuid]
		,	[PointTemplateTagGuid]
		,	[AlarmsEnabled]
		,	[InhibitInputOutputTypeConfiguration]
		,	[InhibitOverride]
		,	[Deadband]
		,	[Holdoff]
		,	[Archived]
		)
		VALUES
		(
			@PointTagGuid
		,	@ID
		,	@EngineeringUnitsType
		,	@EngineeringUnitsIndex
		,	@DecimalPlaces
		,	@ServerEngineeringUnitsIndex
		,	@ValueType
		,	@Status
		,	@Value
		,	@ServerTimeStamp
		,	@SourceTimeStamp
		,	@Maximum
		,	@Minimum
		,	@PointTagInputOutputTypeIndex
		,	@Input
		,	@AlarmStatus
		,	@ApplyPointEngineeringUnits
		,	@ApplyPointDecimalPlaces
		,	@ApplyPointMaximum
		,	@ApplyPointMinimum
		,	@OpcUaServerGuid
		,	@OpcUaBrowsePath
		,	@OpcUaNamespaceUri
		,	@OpcUaPublishingInterval
		,	@OpcUaNodeId
		,	@OpcUaIsReadable
		,	@OpcUaServerDataType
		,	@OpcUaWriteHoldoffTime
		,	@OpcUaWritePeriodicUpdateInterval
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@PointGuid
		,	@PointTemplateTagGuid
		,	@AlarmsEnabled
		,	@InhibitInputOutputTypeConfiguration
		,	@InhibitOverride
		,	@Deadband
		,	@Holdoff
		,	@Archived
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblPointTag]           
		WHERE PointTagGuid=@PointTagGuid;
	
 
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
						+ 'Procedure Name: gsp_PointTagInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
