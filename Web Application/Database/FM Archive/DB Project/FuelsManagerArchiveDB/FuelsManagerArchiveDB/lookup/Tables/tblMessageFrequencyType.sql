/*

	DROP TABLE [lookup].[tblMessageFrequencyType]

*/
/*

	DROP TABLE [lookup].[tblMessageFrequencyType]

*/
CREATE TABLE [lookup].[tblMessageFrequencyType] (
    [MessageFrequencyTypeIndex] INT                NOT NULL,
    [MessageFrequencyTypeCode]  NVARCHAR (100)     NOT NULL,
    [MessageFrequencyTypeName]  NVARCHAR (100)     NULL,
    [MessageFrequencyTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NULL,
    [CreatedBy]                 [dbo].[udtUserID]  NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  NULL,
    [_RowVersion]               ROWVERSION         NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMessageFrequencyType] PRIMARY KEY NONCLUSTERED ([MessageFrequencyTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblMessageFrequencyType_MessageFrequencyTypeGuid]
    ON [lookup].[tblMessageFrequencyType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMessageFrequencyType_ClusterIdx]
    ON [lookup].[tblMessageFrequencyType]([_ClusterIdx] ASC);