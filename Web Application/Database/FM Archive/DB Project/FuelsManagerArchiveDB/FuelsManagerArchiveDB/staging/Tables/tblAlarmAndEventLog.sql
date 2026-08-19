/*

	DROP TABLE [staging].[tblAlarmAndEventLog]

*/
CREATE TABLE [staging].[tblAlarmAndEventLog] (
    [SequenceNumber]       BIGINT             NULL,
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
    [AlarmAndEventLogGuid] UNIQUEIDENTIFIER   NULL,	    
    [SiteGuid]             UNIQUEIDENTIFIER   NULL,
    [SourceNode]		   NVARCHAR (256)     NULL,	
	[SourceRowVersion]	   BIGINT			  NULL,
	[UpdatedDateKey]	   INT				  NULL,
	[ArchiveDate]          DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]		   BIGINT			  NULL,
	[IgnoreRecord]		   BIT				  NOT NULL,
	[IsProcessed]		   BIT				  NOT NULL,
	[_RowVersion]          ROWVERSION         NOT NULL,
	[SKey]				   INT             IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblAlarmAndEventLog_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblAlarmAndEventLog] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblAlarmAndEventLog] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO

