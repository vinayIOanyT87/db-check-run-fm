/*

	DROP TABLE [staging].[tblExportResults]

*/
CREATE TABLE [staging].[tblExportResults] (
    [InterfaceName]					NVARCHAR (150)		NULL,
    [TransVersion]					BIGINT				NULL,
    [FailedCount]					INT					NULL,
    [SuccessCount]					INT					NULL,
    [TransDateTime]					DATETIMEOFFSET (7)	NULL,
    [CreatedDate]					DATETIMEOFFSET (7)	NULL,
    [CreatedBy]						[dbo].[udtUserID]	NULL,
    [UpdatedDate]					DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]						[dbo].[udtUserID]	NULL,
    [BatchID]						NVARCHAR (64)		NULL,
    [ExportResultGuid]				UNIQUEIDENTIFIER	NULL,    
    [SiteGuid]						UNIQUEIDENTIFIER	NULL,
    [LookupExportResultTypeIndex]	INT					NULL,
    [ArchiveFileName]				NVARCHAR (150)		NULL,
	[TransactionGuid]				UNIQUEIDENTIFIER	NULL,
	[InventoryDateKey]				INT                 NULL,
	[ArchiveDate]					DATETIMEOFFSET (7)  NULL,
	[ETLProcessKey]					BIGINT			    NULL,
	[SourceClusterIdx]				BIGINT				NULL,
	[SourceRowVersion]				BIGINT				NULL,
	[IgnoreRecord]					BIT					NOT NULL,
	[IsProcessed]					BIT					NOT NULL,
	[_RowVersion]					ROWVERSION			NOT NULL,
	[SKey]							INT				IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblExportResults_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblExportResults] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblExportResults] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO