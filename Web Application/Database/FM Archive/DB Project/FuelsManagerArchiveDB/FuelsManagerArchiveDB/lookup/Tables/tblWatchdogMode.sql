/*

	DROP TABLE [lookup].[tblWatchdogMode]

*/
CREATE TABLE [lookup].[tblWatchdogMode] (
    [WatchdogModeIndex] TINYINT            NOT NULL,
    [WatchdogModeCode]  NVARCHAR (100)     NOT NULL,
    [WatchdogModeName]  NVARCHAR (100)     NULL,
    [WatchdogModeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblWatchdogMode] PRIMARY KEY NONCLUSTERED ([WatchdogModeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblWatchdogMode_WatchdogModeGuid]
    ON [lookup].[tblWatchdogMode]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblWatchdogMode_ClusterIdx]
    ON [lookup].[tblWatchdogMode]([_ClusterIdx] ASC);