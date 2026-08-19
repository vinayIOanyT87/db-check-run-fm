/*

	DROP TABLE [lookup].[tblAssetTrackingDeviceType]

*/

CREATE TABLE [lookup].[tblAssetTrackingDeviceType]
(
	[AssetTrackingDeviceTypeIndex] INT                NOT NULL,
	[AssetTrackingDeviceTypeCode]  NVARCHAR (100)     NOT NULL,
    [AssetTrackingDeviceTypeName]  NVARCHAR (100)     NULL,
    [AssetTrackingDeviceTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblAssetTrackingDeviceType] PRIMARY KEY NONCLUSTERED ([AssetTrackingDeviceTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblAssetTrackingDeviceType_AssetTrackingDeviceTypeGuid]
    ON [lookup].[tblAssetTrackingDeviceType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAssetTrackingDeviceType_ClusterIdx]
    ON [lookup].[tblAssetTrackingDeviceType]([_ClusterIdx] ASC);