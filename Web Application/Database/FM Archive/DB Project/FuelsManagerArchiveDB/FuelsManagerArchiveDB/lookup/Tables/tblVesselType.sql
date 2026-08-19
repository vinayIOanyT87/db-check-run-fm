/*

	DROP TABLE [lookup].[tblVesselType]

*/
CREATE TABLE [lookup].[tblVesselType] (
    [VesselTypeIndex] INT                NOT NULL,
    [VesselTypeCode]  NVARCHAR (100)     NOT NULL,
    [VesselTypeName]  NVARCHAR (100)     NULL,
    [VesselTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (7) NULL,
    [CreatedBy]       [dbo].[udtUserID]  NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) NULL,
    [UpdatedBy]       [dbo].[udtUserID]  NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblVesselType] PRIMARY KEY NONCLUSTERED ([VesselTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblVesselType_VesselTypeGuid]
    ON [lookup].[tblVesselType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblVesselType_ClusterIdx]
    ON [lookup].[tblVesselType]([_ClusterIdx] ASC);