CREATE TABLE [dbo].[tblExportPaiceTransTracking] (
    [TransID]     NVARCHAR (64)      NOT NULL,
    [TransType]   NVARCHAR (2)       NOT NULL,
    [SentDate]    DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportPaiceTransTracking_SentDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportPaiceTransTracking_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]   [dbo].[udtUserID]  CONSTRAINT [DF_tblExportPaiceTransTracking_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportPaiceTransTracking_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]   [dbo].[udtUserID]  CONSTRAINT [DF_tblExportPaiceTransTracking_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion] ROWVERSION         NOT NULL,
    [_ClusterIdx] BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblExportPaiceTransTracking_GUID] PRIMARY KEY NONCLUSTERED ([TransID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblExportPaiceTransTracking_CreatedDate]
    ON [dbo].[tblExportPaiceTransTracking]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExportPaiceTransTracking_ClusterIdx]
    ON [dbo].[tblExportPaiceTransTracking]([_ClusterIdx] ASC);

