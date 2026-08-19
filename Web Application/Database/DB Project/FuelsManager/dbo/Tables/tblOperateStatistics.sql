CREATE TABLE [dbo].[tblOperateStatistics]
(
    [WindowName] NVARCHAR (50) NULL,
	 [OperateActiveStartTime] DATETIMEOFFSET(7) NULL,
    [OperateActiveStopTime] DATETIMEOFFSET(7) NULL,
    [AvgMinuteTimeAlarmNotifications] INT NULL DEFAULT 0,
    [MaxMinuteTimeAlarmNotifications] INT NULL DEFAULT 0,
    [AvgSessionTimeAlarmNotifications]  INT NULL DEFAULT 0,
    [MaxSessionTimeAlarmNotifications]  INT NULL DEFAULT 0,
    [AvgMinuteTimeAlarmRefresh] INT NULL DEFAULT 0,
    [MaxMinuteTimeAlarmRefresh] INT NULL DEFAULT 0,
    [AvgSessionTimeAlarmRefresh]  INT NULL DEFAULT 0,
    [MaxSessionTimeAlarmRefresh]  INT NULL DEFAULT 0,
    [AvgMinuteTimeUpdateValues] INT NULL DEFAULT 0,
    [MaxMinuteTimeUpdateValues] INT NULL DEFAULT 0,
    [AvgSessionTimeUpdateValues]  INT NULL DEFAULT 0,
    [MaxSessionTimeUpdateValues]  INT NULL DEFAULT 0,
    [AvgMinuteTimeDynamicPointGroup] INT NULL DEFAULT 0,
    [MaxMinuteTimeDynamicPointGroup] INT NULL DEFAULT 0,
    [AvgSessionTimeDynamicPointGroup]  INT NULL DEFAULT 0,
    [MaxSessionTimeDynamicPointGroup]  INT NULL DEFAULT 0,
    [CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblOperateStatistics_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblOperateStatistics_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblOperateStatistics_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblOperateStatistics_UpdatedBy] DEFAULT ('') NOT NULL,
    [SessionGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblOperateStatistics_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion] ROWVERSION NOT NULL,
	 [_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL,
	 CONSTRAINT [FK_tblOperateStatistics_SessionGuid] FOREIGN KEY ([SessionGuid]) REFERENCES [dbo].[tblSessions] ([SessionGuid])
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblOperateStatistics_SessionGuid_WindowName] 
    ON [dbo].[tblOperateStatistics]([SessionGuid] ASC, [WindowName] ASC)
    WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblOperateStatistics_ClusterIdx]
    ON [dbo].[tblOperateStatistics]([_ClusterIdx] ASC);
GO

