CREATE TABLE [dbo].[tblOpcUaMonitoredItem]
(
   [SubscriptionId]								BIGINT NOT NULL,
	[ClientHandle]									BIGINT NOT NULL,
	[PointValueGuid]								UNIQUEIDENTIFIER  NOT NULL,
	[PointValueType]								INT NOT NULL,
	[PointValuePropertyID]						NVARCHAR(30),
	[AttributeId]									INT NOT NULL,
	[LastValue]										VARBINARY(MAX)	NULL,
	[LastServerTimeStamp]						DATETIMEOFFSET (7)  CONSTRAINT [DF_tblOpcUaMonitoredItem_LastServerTimeStamp] DEFAULT (getdate()) NULL,
	[LastSourceTimeStamp]						DATETIMEOFFSET (7)  CONSTRAINT [DF_tblOpcUaMonitoredItem_LastSourceTimeStamp] DEFAULT (getdate()) NULL,
	[LastStatus]									BIGINT NULL,
	[ReadyToPublish]								BIT CONSTRAINT [DF_tblOpcUaMonitoredItem_ReadyToPublish] DEFAULT (0) NOT NULL,
	[CreatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaMonitoredItem_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy]										[dbo].[udtUserID] CONSTRAINT [DF_tblOpcUaMonitoredItem_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaMonitoredItem_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy]										[dbo].[udtUserID] CONSTRAINT [DF_tblOpcUaMonitoredItem_UpdatedBy] DEFAULT ('') NOT NULL,
   [MonitoredItemGuid]							UNIQUEIDENTIFIER   CONSTRAINT [DF_tblOpcUaMonitoredItem_MonitoredItemGuid] DEFAULT (newid()) NOT NULL,
	[_RowVersion]									ROWVERSION NOT NULL,
	[_ClusterIdx]									BIGINT NOT NULL IDENTITY,
   CONSTRAINT [PK_tblOpcUaMonitoredItem_MonitoredItemGuid] PRIMARY KEY NONCLUSTERED ([MonitoredItemGuid] ASC),
   CONSTRAINT [FK_tblOpcUaMonitoredItem_SubscriptionId] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[tblOpcUaSubscription] ([_ClusterIdx]),
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblOpcUaMonitoredItem_ClusterIdx] 
	ON [dbo].[tblOpcUaMonitoredItem]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblOpcUaMonitoredItem_SubscriptionId]
    ON [dbo].[tblOpcUaMonitoredItem]([SubscriptionId] ASC);
GO
