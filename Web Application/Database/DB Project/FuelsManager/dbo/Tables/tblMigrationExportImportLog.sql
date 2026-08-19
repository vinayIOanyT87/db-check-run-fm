CREATE TABLE [dbo].[tblMigrationExportImportLog] (
    [MigrationExportImportLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblMigrationExportImportLog_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]                     UNIQUEIDENTIFIER   NOT NULL,
    [ActivityID]                   NVARCHAR (30)      NOT NULL,
    [ActivityDescription]          NVARCHAR (256)     NOT NULL,
    [ActivityStatus]               NVARCHAR (100)     NOT NULL,
    [PerformedBy]                  NVARCHAR (100)     CONSTRAINT [DF_tblMigrationExportImportLog_PerformedBy] DEFAULT (suser_sname()) NOT NULL,
    [ClientIPAddress]              NVARCHAR (50)      NULL,
    [CreatedDate]                  DATETIMEOFFSET (7) CONSTRAINT [DF_tblMigrationExportImportLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                    [dbo].[udtUserID]  CONSTRAINT [DF_tblMigrationExportImportLog_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                  DATETIMEOFFSET (7) CONSTRAINT [DF_tblMigrationExportImportLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                    [dbo].[udtUserID]  CONSTRAINT [DF_tblMigrationExportImportLog_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                  ROWVERSION         NOT NULL,
    [_ClusterIdx]                  BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblMigrationExportImportLog_GUID] PRIMARY KEY NONCLUSTERED ([MigrationExportImportLogGuid] ASC),
    CONSTRAINT [FK_tblMigrationExportImportLog_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblMigrationExportImportLog_CreatedDate]
    ON [dbo].[tblMigrationExportImportLog]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblMigrationExportImportLog_ActivityID]
    ON [dbo].[tblMigrationExportImportLog]([ActivityID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblMigrationExportImportLog_SiteGuid]
    ON [dbo].[tblMigrationExportImportLog]([SiteGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMigrationExportImportLog_ClusterIdx]
    ON [dbo].[tblMigrationExportImportLog]([_ClusterIdx] ASC);

