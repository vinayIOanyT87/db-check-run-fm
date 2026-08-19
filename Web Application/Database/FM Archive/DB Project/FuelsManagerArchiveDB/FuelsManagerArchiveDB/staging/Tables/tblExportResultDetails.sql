/*

	DROP TABLE [staging].[tblExportResultDetails]

*/
CREATE TABLE [staging].[tblExportResultDetails] (
    [RecordID]					NVARCHAR (64)		NULL,
    [Fail]						BIT					NULL,
    [TransVersion]				BIGINT				NULL,
    [CreatedDate]				DATETIMEOFFSET (7)	NULL,
    [CreatedBy]					[dbo].[udtUserID]	NULL,
    [UpdatedDate]				DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]					[dbo].[udtUserID]	NULL,
    [Error]						NVARCHAR (250)		NULL,
    [ExportResultDetailGuid]	UNIQUEIDENTIFIER	NULL,    
    [ExportResultGuid]			UNIQUEIDENTIFIER	NULL,
    [InterfaceData01]			NVARCHAR (100)		NULL,
    [InterfaceData02]			NVARCHAR (100)		NULL,
    [InterfaceData03]			NVARCHAR (100)		NULL,
    [InterfaceData04]			NVARCHAR (100)		NULL,
    [InterfaceData05]			NVARCHAR (100)		NULL,
    [InterfaceData06]			NVARCHAR (100)		NULL,
    [InterfaceData07]			NVARCHAR (100)		NULL,
    [InterfaceData08]			NVARCHAR (100)		NULL,
	[InventoryDateKey]			INT                 NULL,
	[TransactionGuid]			UNIQUEIDENTIFIER	NULL,
	[ArchiveDate]               DATETIMEOFFSET (7)  NULL,
	[ETLProcessKey]			    BIGINT			    NULL,
	[SourceClusterIdx]			BIGINT				NULL,
	[SourceRowVersion]			BIGINT				NULL,	
	[IgnoreRecord]				BIT					NOT NULL,
	[IsProcessed]				BIT					NOT NULL,
	[_RowVersion]				ROWVERSION			NOT NULL,
	[SKey]						INT					IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblExportResultDetails_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblExportResultDetails] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblExportResultDetails] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO