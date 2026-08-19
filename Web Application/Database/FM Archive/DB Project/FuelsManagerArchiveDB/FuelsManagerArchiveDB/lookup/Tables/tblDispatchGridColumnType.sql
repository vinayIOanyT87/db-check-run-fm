/*

	DROP TABLE [lookup].[tblDispatchGridColumnType]

*/
CREATE TABLE [lookup].[tblDispatchGridColumnType] (
    [DispatchGridColumnTypeIndex] INT                NOT NULL,
    [LookupDispatchGridTypeIndex] INT                NOT NULL,
    [DispatchGridColumnTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) NULL,
    [CreatedBy]                   [dbo].[udtUserID]  NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [ID]                          NVARCHAR (100)     NULL,
    [DisplayName]                 NVARCHAR (100)     NULL,
    [DataField]                   NVARCHAR (100)     NULL,
    [Width]                       INT                NULL,
    [DefaultColumnOrder]          INT                NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblDispatchGridColumnType] PRIMARY KEY NONCLUSTERED ([DispatchGridColumnTypeIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblDispatchGridColumnType_DispatchGridColumnTypeGuid]
    ON [lookup].[tblDispatchGridColumnType]([DispatchGridColumnTypeGuid] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblDispatchGridColumnType_LookupDispatchGridTypeIndex_ID]
    ON [lookup].[tblDispatchGridColumnType]([LookupDispatchGridTypeIndex] ASC, [ID] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDispatchGridColumnType_ClusterIdx]
    ON [lookup].[tblDispatchGridColumnType]([_ClusterIdx] ASC);