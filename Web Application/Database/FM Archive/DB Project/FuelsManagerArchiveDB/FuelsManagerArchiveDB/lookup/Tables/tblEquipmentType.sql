/*

	DROP TABLE [lookup].[tblEquipmentType]

*/
/*

	DROP TABLE [lookup].[tblEquipmentType]

*/
CREATE TABLE [lookup].[tblEquipmentType] (
    [EquipmentTypeIndex] INT                NOT NULL,
    [EquipmentTypeCode]  NVARCHAR (100)     NOT NULL,
    [EquipmentTypeName]  NVARCHAR (100)     NULL,
    [EquipmentTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]        DATETIMEOFFSET (7) NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblEquipmentType] PRIMARY KEY NONCLUSTERED ([EquipmentTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblEquipmentType_EquipmentTypeGuid]
    ON [lookup].[tblEquipmentType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEquipmentType_ClusterIdx]
    ON [lookup].[tblEquipmentType]([_ClusterIdx] ASC);