/*

	DROP TABLE [lookup].[tblDispatchGridType]

*/
CREATE TABLE [lookup].[tblDispatchGridType] (
    [DispatchGridTypeIndex] INT                NOT NULL,
    [DispatchGridTypeCode]  NVARCHAR (100)     NOT NULL,
    [DispatchGridTypeName]  NVARCHAR (100)     NULL,
    [DispatchGridTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblDispatchGridType] PRIMARY KEY NONCLUSTERED ([DispatchGridTypeIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblDispatchGridType_DispatchGridTypeGuid]
    ON [lookup].[tblDispatchGridType]([DispatchGridTypeGuid] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDispatchGridType_ClusterIdx]
    ON [lookup].[tblDispatchGridType]([_ClusterIdx] ASC);