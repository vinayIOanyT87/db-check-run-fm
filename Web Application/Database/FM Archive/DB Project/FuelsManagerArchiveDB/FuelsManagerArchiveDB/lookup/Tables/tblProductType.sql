/*

	DROP TABLE [lookup].[tblProductType]

*/
CREATE TABLE [lookup].[tblProductType] (
    [ProductTypeIndex] INT                NOT NULL,
    [ProductTypeCode]  NVARCHAR (100)     NOT NULL,
    [ProductTypeName]  NVARCHAR (100)     NULL,
    [ProductTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblProductType] PRIMARY KEY NONCLUSTERED ([ProductTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblProductType_ProductTypeGuid]
    ON [lookup].[tblProductType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblProductType_ClusterIdx]
    ON [lookup].[tblProductType]([_ClusterIdx] ASC);