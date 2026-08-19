CREATE TABLE [dbo].[tblImportExportConfig] (
    [Site]                   NVARCHAR (50)    CONSTRAINT [DF_tblImportExportConfig_Site] DEFAULT ('') NOT NULL,
    [ImportExportName]       NVARCHAR (50)    CONSTRAINT [DF_tblImportExportConfig_ImportExportName] DEFAULT ('') NOT NULL,
    [PluginType]             NVARCHAR (50)    CONSTRAINT [DF_tblImportExportConfig_PluginType] DEFAULT ('') NOT NULL,
    [ConfigName]             NVARCHAR (50)    CONSTRAINT [DF_tblImportExportConfig_ConfigName] DEFAULT ('') NOT NULL,
    [LastExported]           NVARCHAR (50)    CONSTRAINT [DF_tblImportExportConfig_LastExported] DEFAULT ('') NOT NULL,
    [ImportExportConfigGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblImportExportConfig_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]            ROWVERSION       NOT NULL,
    [_ClusterIdx]            BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblImportExportConfig_GUID] PRIMARY KEY NONCLUSTERED ([ImportExportConfigGuid] ASC)
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_tblImportExportConfig_ImportExportConfigGuid]
    ON [dbo].[tblImportExportConfig]([ImportExportConfigGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblImportExportConfig_ClusterIdx]
    ON [dbo].[tblImportExportConfig]([_ClusterIdx] ASC);

