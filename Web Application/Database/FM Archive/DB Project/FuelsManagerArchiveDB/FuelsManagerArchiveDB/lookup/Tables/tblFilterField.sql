/*

	DROP TABLE [lookup].[tblFilterField]

*/
CREATE TABLE [lookup].[tblFilterField] (
    [FilterFieldIndex] INT               NOT NULL,
    [FilterFieldCode]  NVARCHAR (100)    NOT NULL,
    [FilterFieldName]  NVARCHAR (100)    NULL,
    [FilterFieldGuid]  UNIQUEIDENTIFIER  NOT NULL,
    [CreatedDate]      DATETIME          NULL,
    [CreatedBy]        [dbo].[udtUserID] NULL,
    [UpdatedDate]      DATETIME          NULL,
    [UpdatedBy]        [dbo].[udtUserID] NULL,
    [_RowVersion]      ROWVERSION        NOT NULL,
    [_ClusterIdx]      BIGINT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblFilterField] PRIMARY KEY NONCLUSTERED ([FilterFieldIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblFilterField_FilterFieldGuid]
    ON [lookup].[tblFilterField]([FilterFieldGuid] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblFilterField_ClusterIdx]
    ON [lookup].[tblFilterField]([_ClusterIdx] ASC);