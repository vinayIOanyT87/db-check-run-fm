CREATE TABLE [dbo].[tblOpcUaSubscription]
(
   [SessionGuid]									UNIQUEIDENTIFIER  NULL,
	[TimeStamp]										DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaSubscription_TimeStamp] DEFAULT (sysdatetimeoffset()) NOT NULL,
   [PublishingInterval]							FLOAT NOT NULL,
   [MaxLifetimeCount]							INT NOT NULL,
   [MaxKeepAliveCount]							INT NOT NULL,
   [MaxNotificationsPerPublish]				INT NOT NULL,
   [PublishingEnabled]							BIT NOT NULL,
   [Priority]										TINYINT NOT NULL,
   [PublishTimerExpiry]							BIGINT NOT NULL,
   [KeepAliveCounter]							INT NOT NULL,
   [LifetimeCounter]								INT NOT NULL,
   [WaitingForPublish]							BIT NOT NULL,
   [LastSentMessage]								INT NOT NULL,
   [SequenceNumber]								BIGINT NOT NULL,
   [MaxMessageCount]								INT NOT NULL,
   [RefreshInProgress]							BIT NOT NULL,
   [Expired]										BIT NOT NULL,
   [ModifyCount]									INT NOT NULL,
   [EnableCount]									INT NOT NULL,
   [DisableCount]									INT NOT NULL,
   [RepublishRequestCount]						INT NOT NULL,
   [RepublishMessageRequestCount]			INT NOT NULL,
   [RepublishMessageCount]						INT NOT NULL,
   [TransferRequestCount]						INT NOT NULL,
   [TransferredToAltClientCount]				INT NOT NULL,
   [TransferredToSameClientCount]			INT NOT NULL,
   [PublishRequestCount]						INT NOT NULL,
   [DataChangeNotificationsCount]			INT NOT NULL,
   [EventNotificationsCount]					INT NOT NULL,
   [NotificationsCount]							INT NOT NULL,
   [LatePublishRequestCount]					INT NOT NULL,
   [CurrentKeepAliveCount]						INT NOT NULL,
   [CurrentLifetimeCount]						INT NOT NULL,
   [UnacknowledgedMessageCount]				INT NOT NULL,
   [DiscardedMessageCount]						INT NOT NULL,
   [MonitoringQueueOverflowCount]			INT NOT NULL,
	[NextSequenceNumber]							INT NOT NULL,
   [EventQueueOverFlowCount]					INT NOT NULL,
	[SerializedSentMessages]					VARBINARY(MAX) NOT NULL,
	[CreatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaSubscription_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy]										[dbo].[udtUserID] CONSTRAINT [DF_tblOpcUaSubscription_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaSubscription_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy]										[dbo].[udtUserID] CONSTRAINT [DF_tblOpcUaSubscription_UpdatedBy] DEFAULT ('') NOT NULL,
   [SubscriptionGuid]							UNIQUEIDENTIFIER   CONSTRAINT [DF_tblOpcUaSubscription_GUID] DEFAULT (newid()) NOT NULL,
	[_RowVersion]									ROWVERSION NOT NULL,
	[_ClusterIdx]									BIGINT NOT NULL IDENTITY,
   CONSTRAINT [PK_tblOpcUaSubscription_SubscriptionGuid] PRIMARY KEY NONCLUSTERED ([SubscriptionGuid] ASC),
   CONSTRAINT [FK_tblOpcUaSubscription_SessionGuid] FOREIGN KEY ([SessionGuid]) REFERENCES [dbo].[tblOpcUaSession] ([SessionGuid]),
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblOpcUaSubscription_ClusterIdx] 
	ON [dbo].[tblOpcUaSubscription]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblOpcUaSubscription_SessionGuid]
    ON [dbo].[tblOpcUaSubscription]([SessionGuid] ASC);
GO
