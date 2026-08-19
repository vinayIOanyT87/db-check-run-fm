/*

	DROP TABLE [dbo].[tblAlarmAndEvents]

*/
CREATE TABLE [dbo].[tblAlarmAndEvents] (
    [Source]            NVARCHAR (120)     NULL,
    [Alarm]             BIT                NULL,
    [ID]                NVARCHAR (120)     NULL,
    [CategoryIndex]     INT                NULL,
    [PriorityIndex]     INT                NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [Enabled]           BIT                NULL,
    [AlarmAndEventGuid] UNIQUEIDENTIFIER   NOT NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [SiteGuid]          UNIQUEIDENTIFIER   NOT NULL,
    [CategoryGuid]      UNIQUEIDENTIFIER   NULL,
    [PriorityGuid]      UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAlarmAndEvents_GUID] PRIMARY KEY NONCLUSTERED ([AlarmAndEventGuid] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEvents_CreatedDate]
    ON [dbo].[tblAlarmAndEvents]([CreatedDate] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEvents_Source_ID]
    ON [dbo].[tblAlarmAndEvents]([Source] ASC, [ID] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblAlarmAndEvents_ID_SiteGuid]
    ON [dbo].[tblAlarmAndEvents]([ID] ASC, [SiteGuid] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAlarmAndEvents_ClusterIdx]
    ON [dbo].[tblAlarmAndEvents]([_ClusterIdx] ASC);