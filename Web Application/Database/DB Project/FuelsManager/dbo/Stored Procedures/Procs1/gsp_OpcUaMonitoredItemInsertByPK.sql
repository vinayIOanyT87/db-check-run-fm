	
CREATE PROCEDURE [dbo].[gsp_OpcUaMonitoredItemInsertByPK]
(
		@MonitoredItemGuid uniqueidentifier=NULL OUTPUT
	,	@SubscriptionId bigint=NULL
	,	@PointValueGuid uniqueidentifier=NULL
	,	@PointValueType int=NULL
	,	@PointValuePropertyID nvarchar(30)=NULL
	,	@ClientHandle bigint=NULL
	,	@AttributeId int=null
	,	@LastValue varbinary(max)=NULL
	,	@LastServerTimeStamp datetimeoffset(7)=NULL
	,	@LastSourceTimeStamp datetimeoffset(7)=NULL
	,	@LastStatus bigint=NULL
	,	@ReadyToPublish bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_ClusterIdx bigint=NULL OUTPUT
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_OpcUaMonitoredItemInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2015-11-07 12:58:44.6753305 -05:00
	-- Purpose: Insert into table [dbo].[tblOpcUaMonitoredItem]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MonitoredItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblOpcUaMonitoredItem] 
		(
			[MonitoredItemGuid]
		,	[SubscriptionId]
		,	[PointValueGuid]
		,	[PointValueType]
		,	[PointValuePropertyID]
		,	[ClientHandle]
		,	[AttributeId]
		,	[LastValue]
		,	[LastServerTimeStamp]
		,	[LastSourceTimeStamp]
		,	[LastStatus]
		,	[ReadyToPublish]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@MonitoredItemGuid
		,	@SubscriptionId
		,	@PointValueGuid
		,	@PointValueType
		,	@PointValuePropertyID
		,	@ClientHandle
		,	@AttributeId
		,	@LastValue
		,	@LastServerTimeStamp
		,	@LastSourceTimeStamp
		,	@LastStatus
		,	@ReadyToPublish
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion = _RowVersion,@_ClusterIdx = _ClusterIdx        
		FROM [dbo].[tblOpcUaMonitoredItem]           
		WHERE MonitoredItemGuid=@MonitoredItemGuid;
	
 
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
						+ 'Procedure Name: gsp_OpcUaMonitoredItemInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
GO
 
