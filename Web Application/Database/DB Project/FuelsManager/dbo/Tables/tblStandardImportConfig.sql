CREATE TABLE [dbo].[tblStandardImportConfig] (
    [Name]                     NVARCHAR (50)    CONSTRAINT [DF_tblStandardImportConfig_Name] DEFAULT ('') NOT NULL,
    [KeyName]                  NVARCHAR (50)    CONSTRAINT [DF_tblStandardImportConfig_KeyName] DEFAULT ('') NOT NULL,
    [KeyValue]                 NVARCHAR (50)    CONSTRAINT [DF_tblStandardImportConfig_KeyValue] DEFAULT ('') NOT NULL,
    [StandardImportConfigGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblStandardImportConfig_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]              ROWVERSION       NOT NULL,
    [_ClusterIdx]              BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblStandardImportConfig_GUID] PRIMARY KEY NONCLUSTERED ([StandardImportConfigGuid] ASC)
);


GO



GO
CREATE NONCLUSTERED INDEX [IX_tblStandardImportConfig_StandardImportConfigGuid]
    ON [dbo].[tblStandardImportConfig]([StandardImportConfigGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblStandardImportConfig_ClusterIdx]
    ON [dbo].[tblStandardImportConfig]([_ClusterIdx] ASC);

