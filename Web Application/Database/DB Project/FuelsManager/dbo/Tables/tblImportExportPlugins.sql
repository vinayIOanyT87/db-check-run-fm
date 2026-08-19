CREATE TABLE [dbo].[tblImportExportPlugins] (
    [PluginType]             NVARCHAR (50)    CONSTRAINT [DF_tblImportExportPlugins_PluginType] DEFAULT ('') NOT NULL,
    [ConfigURL]              NVARCHAR (250)   CONSTRAINT [DF_tblImportExportPlugins_ConfigURL] DEFAULT ('') NOT NULL,
    [RunURL]                 NVARCHAR (250)   CONSTRAINT [DF_tblImportExportPlugins_RunURL] DEFAULT ('') NOT NULL,
    [Import]                 BIT              CONSTRAINT [DF_tblImportExportPlugins_Import] DEFAULT ((0)) NOT NULL,
    [Export]                 BIT              CONSTRAINT [DF_tblImportExportPlugins_Export] DEFAULT ((0)) NOT NULL,
    [ImportExportPluginGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblImportExportPlugins_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]            ROWVERSION       NOT NULL,
    [_ClusterIdx]            BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblImportExportPlugins_GUID] PRIMARY KEY NONCLUSTERED ([ImportExportPluginGuid] ASC)
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_tblImportExportPlugins_ImportExportPluginGuid]
    ON [dbo].[tblImportExportPlugins]([ImportExportPluginGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblImportExportPlugins_ClusterIdx]
    ON [dbo].[tblImportExportPlugins]([_ClusterIdx] ASC);

