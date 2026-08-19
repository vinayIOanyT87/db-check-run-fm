/*

	DROP TABLE [lookup].[tblVariantType]

*/
CREATE TABLE [lookup].[tblVariantType] (
    [VariantTypeIndex] INT               NOT NULL,
    [CodeType]         NVARCHAR (100)    NOT NULL,
    [DatabaseType]     NVARCHAR (100)    NOT NULL,
    [VariantTypeGuid]  UNIQUEIDENTIFIER  NOT NULL,
    [CreatedDate]      DATETIME          NULL,
    [CreatedBy]        [dbo].[udtUserID] NULL,
    [UpdatedDate]      DATETIME          NULL,
    [UpdatedBy]        [dbo].[udtUserID] NULL,
    [_RowVersion]      ROWVERSION        NOT NULL,
    [_ClusterIdx]      BIGINT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblVariantType] PRIMARY KEY NONCLUSTERED ([VariantTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblVariantType_ClusterIdx]
    ON [lookup].[tblVariantType]([_ClusterIdx] ASC);