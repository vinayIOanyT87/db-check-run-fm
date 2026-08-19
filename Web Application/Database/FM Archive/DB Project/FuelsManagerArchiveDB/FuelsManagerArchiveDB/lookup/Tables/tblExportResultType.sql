/*

	DROP TABLE [lookup].[tblExportResultType]

*/
CREATE TABLE [lookup].[tblExportResultType] (
    [ExportResultTypeIndex] INT                NOT NULL,
    [ExportResultTypeCode]  NVARCHAR (100)     NOT NULL,
    [ExportResultTypeName]  NVARCHAR (100)     NULL,
    [ExportResultTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblExportResultType] PRIMARY KEY NONCLUSTERED ([ExportResultTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblExportResultType_ExportResultTypeGuid]
    ON [lookup].[tblExportResultType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExportResultType_ClusterIdx]
    ON [lookup].[tblExportResultType]([_ClusterIdx] ASC);