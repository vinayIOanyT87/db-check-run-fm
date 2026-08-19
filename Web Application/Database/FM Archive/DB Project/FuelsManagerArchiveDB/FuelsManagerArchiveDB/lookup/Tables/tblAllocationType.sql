/*

	DROP TABLE [lookup].[tblAllocationType]

*/
CREATE TABLE [lookup].[tblAllocationType] (
    [AllocationTypeIndex] INT                NOT NULL,
    [AllocationTypeCode]  NVARCHAR (100)     NOT NULL,
    [AllocationTypeName]  NVARCHAR (100)     NULL,
    [AllocationTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]         DATETIMEOFFSET (7) NULL,
    [CreatedBy]           [dbo].[udtUserID]  NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) NULL,
    [UpdatedBy]           [dbo].[udtUserID]  NULL,
    [_RowVersion]         ROWVERSION         NOT NULL,
    [_ClusterIdx]         BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblAllocationType] PRIMARY KEY NONCLUSTERED ([AllocationTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAllocationType_ClusterIdx]
    ON [lookup].[tblAllocationType]([_ClusterIdx] ASC);