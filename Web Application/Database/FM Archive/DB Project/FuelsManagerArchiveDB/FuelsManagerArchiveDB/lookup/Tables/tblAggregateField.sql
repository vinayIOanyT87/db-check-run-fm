/*

	DROP TABLE [lookup].[tblAggregateField]

*/
CREATE TABLE [lookup].[tblAggregateField] (
    [AggregateFieldIndex] INT                NOT NULL,
    [AggregateFieldCode]  NVARCHAR (100)     NOT NULL,
    [AggregateFieldName]  NVARCHAR (100)     NULL,
    [AggregateFieldGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]         DATETIMEOFFSET (7) NULL,
    [CreatedBy]           [dbo].[udtUserID]  NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) NULL,
    [UpdatedBy]           [dbo].[udtUserID]  NULL,
    [_RowVersion]         ROWVERSION         NOT NULL,
    [_ClusterIdx]         BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblAggregateField] PRIMARY KEY NONCLUSTERED ([AggregateFieldIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAggregateField_ClusterIdx]
    ON [lookup].[tblAggregateField]([_ClusterIdx] ASC);