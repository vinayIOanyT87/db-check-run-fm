CREATE TABLE [dbo].[tblExportTransportModeMapping] (
    [ExportTransportModeMappingGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExportTransportModeMapping_GUID] DEFAULT (newid()) NOT NULL,
    [FMATransportMode]               NVARCHAR (100)     NOT NULL,
    [FuelPlusTransportMode]          NVARCHAR (100)     CONSTRAINT [DF_tblExportTransportModeMapping_FuelPlusTransportMode] DEFAULT ('INVALID') NOT NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportTransportModeMapping_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblExportTransportModeMapping_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportTransportModeMapping_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblExportTransportModeMapping_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblExportTransportModeMapping_FMATransportMode] PRIMARY KEY NONCLUSTERED ([FMATransportMode] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExportTransportModeMapping_ClusterIdx]
    ON [dbo].[tblExportTransportModeMapping]([_ClusterIdx] ASC);

