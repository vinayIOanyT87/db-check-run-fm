/*

	DROP TABLE [lookup].[tblProcessVariableType]

*/
CREATE TABLE [lookup].[tblProcessVariableType] (
    [ProcessVariableTypeIndex] INT                NOT NULL,
    [ProcessVariableTypeCode]  NVARCHAR (100)     NOT NULL,
    [ProcessVariableTypeName]  NVARCHAR (100)     NULL,
    [ProcessVariableTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblProcessVariableType] PRIMARY KEY NONCLUSTERED ([ProcessVariableTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblProcessVariableType_ProcessVariableTypeGuid]
    ON [lookup].[tblProcessVariableType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblProcessVariableType_ClusterIdx]
    ON [lookup].[tblProcessVariableType]([_ClusterIdx] ASC);