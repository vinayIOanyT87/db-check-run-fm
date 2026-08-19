CREATE TABLE [dbo].[tblAlarmAndEventLog] (
    [SequenceNumber]       BIGINT             IDENTITY (1, 1) NOT NULL,
    [Source]               NVARCHAR (120)     CONSTRAINT [DF_tblAlarmAndEventLog_Source] DEFAULT ('') NOT NULL,
    [Alarm]                BIT                CONSTRAINT [DF_tblAlarmAndEventLog_Alarm] DEFAULT ((0)) NOT NULL,
    [ID]                   NVARCHAR (120)     CONSTRAINT [DF_tblAlarmAndEventLog_ID] DEFAULT ('') NOT NULL,
    [AssociatedData]       NVARCHAR (MAX)     CONSTRAINT [DF_tblAlarmAndEventLog_AssociatedData] DEFAULT ('') NOT NULL,
    [CategoryID]           NVARCHAR (50)      CONSTRAINT [DF_tblAlarmAndEventLog_CategoryID] DEFAULT ('') NOT NULL,
    [PriorityID]           NVARCHAR (50)      CONSTRAINT [DF_tblAlarmAndEventLog_PriorityID] DEFAULT ('') NOT NULL,
    [Acknowledged]         BIT                CONSTRAINT [DF_tblAlarmAndEventLog_Acknowledged] DEFAULT ((0)) NOT NULL,
    [CreatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAlarmAndEventLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblAlarmAndEventLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAlarmAndEventLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblAlarmAndEventLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [AlarmAndEventLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblAlarmAndEventLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [SiteGuid]             UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblAlarmAndEventLog_GUID] PRIMARY KEY NONCLUSTERED ([AlarmAndEventLogGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblAlarmAndEventLog_CreatedDate]
    ON [dbo].[tblAlarmAndEventLog]([CreatedDate] ASC);

GO
