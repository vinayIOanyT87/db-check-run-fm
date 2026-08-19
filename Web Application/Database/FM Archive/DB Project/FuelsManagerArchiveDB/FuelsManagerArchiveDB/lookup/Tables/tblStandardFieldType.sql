/*

	DROP TABLE [lookup].[tblStandardFieldType] 

*/
CREATE TABLE [lookup].[tblStandardFieldType] (
    [StandardFieldTypeIndex] INT                NOT NULL,
    [StandardFieldTypeCode]  NVARCHAR (100)     NOT NULL,
    [StandardFieldTypeName]  NVARCHAR (100)     NULL,
    [StandardFieldTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblStandardFieldType] PRIMARY KEY NONCLUSTERED ([StandardFieldTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblStandardFieldType_StandardFieldTypeGuid]
    ON [lookup].[tblStandardFieldType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblStandardFieldType_ClusterIdx]
    ON [lookup].[tblStandardFieldType]([_ClusterIdx] ASC);