	
CREATE PROCEDURE [dbo].[gsp_OpcUaSubscriptionInsertByPK]
(
		@SubscriptionGuid uniqueidentifier=NULL OUTPUT
	,	@SessionGuid uniqueidentifier=NULL
	,	@TimeStamp datetimeoffset=NULL
	,	@PublishingInterval float=NULL
	,	@MaxLifetimeCount int=NULL
	,	@MaxKeepAliveCount int=NULL
	,	@MaxNotificationsPerPublish int=NULL
	,	@PublishingEnabled bit=NULL
	,	@Priority tinyint=NULL
	,	@PublishTimerExpiry bigint=NULL
	,	@KeepAliveCounter int=NULL
	,	@LifetimeCounter int=NULL
	,	@WaitingForPublish bit=NULL
	,	@LastSentMessage int=NULL
	,	@SequenceNumber bigint=NULL
	,	@MaxMessageCount int=NULL
	,	@RefreshInProgress bit=NULL
	,	@Expired bit=NULL
	,	@ModifyCount int=NULL
	,	@EnableCount int=NULL
	,	@DisableCount int=NULL
	,	@RepublishRequestCount int=NULL
	,	@RepublishMessageRequestCount int=NULL
	,	@RepublishMessageCount int=NULL
	,	@TransferRequestCount int=NULL
	,	@TransferredToAltClientCount int=NULL
	,	@TransferredToSameClientCount int=NULL
	,	@PublishRequestCount int=NULL
	,	@DataChangeNotificationsCount int=NULL
	,	@EventNotificationsCount int=NULL
	,	@NotificationsCount int=NULL
	,	@LatePublishRequestCount int=NULL
	,	@CurrentKeepAliveCount int=NULL
	,	@CurrentLifetimeCount int=NULL
	,	@UnacknowledgedMessageCount int=NULL
	,	@DiscardedMessageCount int=NULL
	,	@MonitoringQueueOverflowCount int=NULL
	,	@NextSequenceNumber int=NULL
	,	@EventQueueOverFlowCount int=NULL
	,	@SerializedSentMessages varbinary(max) = NULL
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
	-- Stored procedure: [dbo].[gsp_OpcUaSubscriptionInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2015-11-05 09:19:17.2562759 -05:00
	-- Purpose: Insert into table [dbo].[tblOpcUaSubscription]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SubscriptionGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblOpcUaSubscription] 
		(
			[SubscriptionGuid]
		,	[SessionGuid]
		,	[TimeStamp]
		,	[PublishingInterval]
		,	[MaxLifetimeCount]
		,	[MaxKeepAliveCount]
		,	[MaxNotificationsPerPublish]
		,	[PublishingEnabled]
		,	[Priority]
		,	[PublishTimerExpiry]
		,	[KeepAliveCounter]
		,	[LifetimeCounter]
		,	[WaitingForPublish]
		,	[LastSentMessage]
		,	[SequenceNumber]
		,	[MaxMessageCount]
		,	[RefreshInProgress]
		,	[Expired]
		,	[ModifyCount]
		,	[EnableCount]
		,	[DisableCount]
		,	[RepublishRequestCount]
		,	[RepublishMessageRequestCount]
		,	[RepublishMessageCount]
		,	[TransferRequestCount]
		,	[TransferredToAltClientCount]
		,	[TransferredToSameClientCount]
		,	[PublishRequestCount]
		,	[DataChangeNotificationsCount]
		,	[EventNotificationsCount]
		,	[NotificationsCount]
		,	[LatePublishRequestCount]
		,	[CurrentKeepAliveCount]
		,	[CurrentLifetimeCount]
		,	[UnacknowledgedMessageCount]
		,	[DiscardedMessageCount]
		,	[MonitoringQueueOverflowCount]
		,	[NextSequenceNumber]
		,	[EventQueueOverFlowCount]
		,	[SerializedSentMessages]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@SubscriptionGuid
		,	@SessionGuid
		,	@TimeStamp
		,	@PublishingInterval
		,	@MaxLifetimeCount
		,	@MaxKeepAliveCount
		,	@MaxNotificationsPerPublish
		,	@PublishingEnabled
		,	@Priority
		,	@PublishTimerExpiry
		,	@KeepAliveCounter
		,	@LifetimeCounter
		,	@WaitingForPublish
		,	@LastSentMessage
		,	@SequenceNumber
		,	@MaxMessageCount
		,	@RefreshInProgress
		,	@Expired
		,	@ModifyCount
		,	@EnableCount
		,	@DisableCount
		,	@RepublishRequestCount
		,	@RepublishMessageRequestCount
		,	@RepublishMessageCount
		,	@TransferRequestCount
		,	@TransferredToAltClientCount
		,	@TransferredToSameClientCount
		,	@PublishRequestCount
		,	@DataChangeNotificationsCount
		,	@EventNotificationsCount
		,	@NotificationsCount
		,	@LatePublishRequestCount
		,	@CurrentKeepAliveCount
		,	@CurrentLifetimeCount
		,	@UnacknowledgedMessageCount
		,	@DiscardedMessageCount
		,	@MonitoringQueueOverflowCount
		,	@NextSequenceNumber
		,	@EventQueueOverFlowCount
		,	@SerializedSentMessages
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion = _RowVersion,@_ClusterIdx = _ClusterIdx        
		FROM [dbo].[tblOpcUaSubscription]           
		WHERE SubscriptionGuid=@SubscriptionGuid;
	
 
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
						+ 'Procedure Name: gsp_OpcUaSubscriptionInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
GO
 
