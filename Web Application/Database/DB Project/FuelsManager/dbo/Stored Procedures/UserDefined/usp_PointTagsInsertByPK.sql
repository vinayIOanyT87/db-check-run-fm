CREATE PROCEDURE [dbo].[usp_PointTagsInsertByPK]
(
	@PointTags dbo.PointTagType READONLY
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_PointTagsInsertByPK]
	-- Author: C. Townsend
	-- Version/Date: 1.0 / 2018-7-31 
	-- Purpose: Insert into table [dbo].[tblPointTag]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
  
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
		,	[AlarmsEnabled]
		,	[InhibitInputOutputTypeConfiguration]
		,	[InhibitOverride]
		,	[Deadband]
		,	[Holdoff]
		,	[Archived]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PointGuid]
		,	[PointTemplateTagGuid]
		)
		SELECT
			t.PointTagGuid
		,	t.ID
		,	t.EngineeringUnitsType
		,	t.EngineeringUnitsIndex
		,	t.DecimalPlaces
		,	t.ServerEngineeringUnitsIndex
		,	t.ValueType
		,	t.Status
		,	t.Value
		,	t.ServerTimeStamp
		,	t.SourceTimeStamp
		,	t.Maximum
		,	t.Minimum
		,	t.PointTagInputOutputTypeIndex
		,	t.Input
		,	t.AlarmStatus
		,	t.ApplyPointEngineeringUnits
		,	t.ApplyPointDecimalPlaces
		,	t.ApplyPointMaximum
		,	t.ApplyPointMinimum
		,	t.OpcUaServerGuid
		,	t.OpcUaBrowsePath
		,	t.OpcUaNamespaceUri
		,	t.OpcUaPublishingInterval
		,	t.OpcUaNodeId
		,	t.OpcUaIsReadable
		,	t.OpcUaServerDataType
		,	t.OpcUaWriteHoldoffTime
		,	t.OpcUaWritePeriodicUpdateInterval
		,	t.AlarmsEnabled
		,	t.InhibitInputOutputTypeConfiguration
		,	t.InhibitOverride
		,	t.Deadband
		,	t.Holdoff
		,	t.Archived
		,	t.CreatedDate
		,	t.CreatedBy
		,	t.UpdatedDate
		,	t.UpdatedBy
		,	t.PointGuid
		,	t.PointTemplateTagGuid
FROM @PointTags t
	
 
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
						+ 'Procedure Name: usp_PointTagsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
