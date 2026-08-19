/*

	DROP  TABLE [lookup].[tblPresetType]

*/
CREATE TABLE [lookup].[tblPresetType] (
    [PresetTypeIndex] INT                NOT NULL,
    [PresetTypeCode]  NVARCHAR (100)     NOT NULL,
    [PresetTypeName]  NVARCHAR (100)     NULL,
    [PresetTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (7) NULL,
    [CreatedBy]       [dbo].[udtUserID]  NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) NULL,
    [UpdatedBy]       [dbo].[udtUserID]  NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblPresetType] PRIMARY KEY NONCLUSTERED ([PresetTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblPresetType_PresetTypeGuid]
    ON [lookup].[tblPresetType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPresetType_ClusterIdx]
    ON [lookup].[tblPresetType]([_ClusterIdx] ASC);