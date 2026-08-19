/*

	DROP TABLE [lookup].[tblCustomToolbarType]

*/
CREATE TABLE [lookup].[tblCustomToolbarType] (
    [CustomToolbarTypeIndex] INT                NOT NULL,
    [CustomToolbarTypeCode]  NVARCHAR (100)     NOT NULL,
    [CustomToolbarTypeName]  NVARCHAR (100)     NULL,
    [CustomToolbarTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCustomToolbarType] PRIMARY KEY NONCLUSTERED ([CustomToolbarTypeIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblCustomToolbarType_CustomToolbarTypeGuid]
    ON [lookup].[tblCustomToolbarType]([CustomToolbarTypeGuid] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCustomToolbarType_ClusterIdx]
    ON [lookup].[tblCustomToolbarType]([_ClusterIdx] ASC);