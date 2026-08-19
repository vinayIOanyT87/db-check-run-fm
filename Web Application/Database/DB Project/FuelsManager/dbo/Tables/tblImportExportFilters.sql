CREATE TABLE [dbo].[tblImportExportFilters] (
    [Site]                   NVARCHAR (30)    CONSTRAINT [DF_tblImportExportFilters_Site] DEFAULT ('') NOT NULL,
    [ConfigurationID]        INT              CONSTRAINT [DF_tblImportExportFilters_ConfigurationID] DEFAULT ((0)) NOT NULL,
    [Role]                   NVARCHAR (30)    CONSTRAINT [DF_tblImportExportFilters_Role] DEFAULT ('') NOT NULL,
    [CompanyID]              NVARCHAR (50)    NULL,
    [ImportExportFilterGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblImportExportFilters_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]            ROWVERSION       NOT NULL,
    [_ClusterIdx]            BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblImportExportFilters_GUID] PRIMARY KEY NONCLUSTERED ([ImportExportFilterGuid] ASC)
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_tblImportExportFilters_ImportExportFilterGuid]
    ON [dbo].[tblImportExportFilters]([ImportExportFilterGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblImportExportFilters_ClusterIdx]
    ON [dbo].[tblImportExportFilters]([_ClusterIdx] ASC);

