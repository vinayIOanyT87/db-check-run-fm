/*

	DROP TABLE [dbo].[tblExportResultDetails]

*/
CREATE TABLE [dbo].[tblExportResultDetails] (
    [RecordID]               NVARCHAR (64)      NULL,
    [Fail]                   BIT                NULL,
    [TransVersion]           BIGINT             NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [Error]                  NVARCHAR (250)     NULL,
    [ExportResultDetailGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [ExportResultGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [InterfaceData01]        NVARCHAR (100)     NULL,
    [InterfaceData02]        NVARCHAR (100)     NULL,
    [InterfaceData03]        NVARCHAR (100)     NULL,
    [InterfaceData04]        NVARCHAR (100)     NULL,
    [InterfaceData05]        NVARCHAR (100)     NULL,
    [InterfaceData06]        NVARCHAR (100)     NULL,
    [InterfaceData07]        NVARCHAR (100)     NULL,
    [InterfaceData08]        NVARCHAR (100)     NULL,
	[InventoryDateKey]		 INT				NOT NULL,
	[ArchiveDate]            DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]		     BIGINT			    NULL,
	[_RowVersion]            ROWVERSION         NOT NULL,
	[_ClusterIdx]			 BIGINT		NOT NULL IDENTITY,
    CONSTRAINT [PK_tblExportResultDetails_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [ExportResultDetailGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblExportResultDetails_ClusterIdx] 
	ON [dbo].[tblExportResultDetails]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_CreatedDate]
    ON [dbo].[tblExportResultDetails]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_ExportResultGuid]
    ON [dbo].[tblExportResultDetails]([ExportResultGuid] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_GuidRowVersion]
    ON [dbo].[tblExportResultDetails]([ExportResultDetailGuid] ASC, [_RowVersion] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_RecordID_TransVersion]
    ON [dbo].[tblExportResultDetails]([RecordID] ASC, [TransVersion] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO
