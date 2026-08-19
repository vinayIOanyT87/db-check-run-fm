/*

	DROP TABLE [dbo].[tblExportResults]

*/
CREATE TABLE [dbo].[tblExportResults] (
    [InterfaceName]               NVARCHAR (150)     NULL,
    [TransVersion]                BIGINT             NULL,
    [FailedCount]                 INT                NULL,
    [SuccessCount]                INT                NULL,
    [TransDateTime]               DATETIMEOFFSET (7) NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) NULL,
    [CreatedBy]                   [dbo].[udtUserID]  NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  NULL,
    [BatchID]                     NVARCHAR (64)      NULL,
    [ExportResultGuid]            UNIQUEIDENTIFIER   NOT NULL,    
    [SiteGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [LookupExportResultTypeIndex] INT                NULL,
    [ArchiveFileName]             NVARCHAR (150)     NULL,
	[InventoryDateKey]			  INT                NOT NULL,
	[ArchiveDate]				  DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]				  BIGINT		     NULL,
	[_RowVersion]                 ROWVERSION         NOT NULL,
	[_ClusterIdx]				  BIGINT			 NOT NULL IDENTITY,
    CONSTRAINT [PK_tblExportResults_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [ExportResultGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblExportResults_ClusterIdx] 
	ON [dbo].[tblExportResults]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResults_CreatedDate]
    ON [dbo].[tblExportResults]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResults_LookupExportResultTypeIndex]
    ON [dbo].[tblExportResults]([LookupExportResultTypeIndex] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResults_SiteGuid]
    ON [dbo].[tblExportResults]([SiteGuid] ASC, [ExportResultGuid] ASC)
    INCLUDE([InterfaceName], [TransVersion], [FailedCount], [SuccessCount], [TransDateTime], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [BatchID], [ArchiveFileName], [_RowVersion], [LookupExportResultTypeIndex])
	ON [AnnualPS]([InventoryDateKey]);
GO
