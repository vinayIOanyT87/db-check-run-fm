/*

	DROP TABLE [lookup].[tblAssetTrackingPayloadType]

*/
CREATE TABLE [lookup].[tblAssetTrackingPayloadType]
(
	[AssetTrackingPayloadTypeIndex] INT                NOT NULL,
	[AssetTrackingPayloadTypeCode]  NVARCHAR (100)     NOT NULL,
    [AssetTrackingPayloadTypeName]  NVARCHAR (100)     NULL,
    [AssetTrackingPayloadTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblAssetTrackingPayloadType] PRIMARY KEY NONCLUSTERED ([AssetTrackingPayloadTypeIndex] ASC)
)
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblAssetTrackingPayloadType_AssetTrackingPayloadTypeGuid]
    ON [lookup].[tblAssetTrackingPayloadType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAssetTrackingPayloadType_ClusterIdx]
    ON [lookup].[tblAssetTrackingPayloadType]([_ClusterIdx] ASC);