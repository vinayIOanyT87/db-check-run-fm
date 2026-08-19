/*

	DROP TABLE [lookup].[tblNumberGroupSizesType]

*/
CREATE TABLE [lookup].[tblNumberGroupSizesType] (
    [NumberGroupSizesTypeIndex] INT                NOT NULL,
    [NumberGroupSizesTypeCode]  NVARCHAR (100)     NOT NULL,
    [NumberGroupSizesTypeName]  NVARCHAR (100)     NULL,
    [NumberGroupSizesTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NULL,
    [CreatedBy]                 [dbo].[udtUserID]  NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  NULL,
    [_RowVersion]               ROWVERSION         NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblNumberGroupSizesType] PRIMARY KEY NONCLUSTERED ([NumberGroupSizesTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblNumberGroupSizesType_NumberGroupSizesTypeGuid]
    ON [lookup].[tblNumberGroupSizesType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblNumberGroupSizesType_ClusterIdx]
    ON [lookup].[tblNumberGroupSizesType]([_ClusterIdx] ASC);