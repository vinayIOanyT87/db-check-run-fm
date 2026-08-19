/*

	DROP TABLE [lookup].[tblUserDataType]

*/
CREATE TABLE [lookup].[tblUserDataType] (
    [UserDataTypeIndex] INT                NOT NULL,
    [UserDataTypeCode]  NVARCHAR (100)     NOT NULL,
    [UserDataTypeName]  NVARCHAR (100)     NULL,
    [UserDataTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblUserDataType] PRIMARY KEY NONCLUSTERED ([UserDataTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblUserDataType_UserDataTypeGuid]
    ON [lookup].[tblUserDataType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblUserDataType_ClusterIdx]
    ON [lookup].[tblUserDataType]([_ClusterIdx] ASC);