/*

	DROP TABLE [dbo].[tblAlarmAndEventLog]

*/
CREATE TABLE [dbo].[tblAlarmAndEventLog] (
    [SequenceNumber]       BIGINT             NOT NULL,
    [Source]               NVARCHAR (120)     NULL,
    [Alarm]                BIT                NULL,
    [ID]                   NVARCHAR (120)     NULL,
    [AssociatedData]       NVARCHAR (MAX)     NULL,
    [CategoryID]           NVARCHAR (50)      NULL,
    [PriorityID]           NVARCHAR (50)      NULL,
    [Acknowledged]         BIT                NULL,
    [CreatedDate]          DATETIMEOFFSET (7) NULL,
    [CreatedBy]            [dbo].[udtUserID]  NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) NULL,
    [UpdatedBy]            [dbo].[udtUserID]  NULL,
    [AlarmAndEventLogGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [SiteGuid]             UNIQUEIDENTIFIER   NOT NULL,
    [SourceNode]		   NVARCHAR (256)     NULL,	
	[UpdatedDateKey]	   INT				  NOT NULL,
	[ArchiveDate]		   DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]		   BIGINT			  NULL,
	[_RowVersion]          ROWVERSION         NOT NULL,
	[_ClusterIdx]		   BIGINT             IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblAlarmAndEventLog_GUID] PRIMARY KEY NONCLUSTERED ([UpdatedDateKey] ASC, [AlarmAndEventLogGuid] ASC) ON [AnnualPS]([UpdatedDateKey])
) ON [AnnualPS]([UpdatedDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEventLog_SiteGuid_CreatedDate]
    ON [dbo].[tblAlarmAndEventLog]([SiteGuid] ASC, [CreatedDate] ASC)
	ON [AnnualPS]([UpdatedDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEventLog_SequenceNumber]
    ON [dbo].[tblAlarmAndEventLog]([SequenceNumber] ASC)
	ON [AnnualPS]([UpdatedDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblAlarmAndEventLog_ClusterIdx]
    ON [dbo].[tblAlarmAndEventLog]([UpdatedDateKey] ASC, [_ClusterIdx] ASC)
	ON [AnnualPS]([UpdatedDateKey]);
GO