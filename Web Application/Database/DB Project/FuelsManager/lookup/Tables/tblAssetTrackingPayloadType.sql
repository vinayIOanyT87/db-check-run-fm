CREATE TABLE [lookup].[tblAssetTrackingPayloadType]
(
	[AssetTrackingPayloadTypeIndex] INT                NOT NULL,
	[AssetTrackingPayloadTypeCode]  NVARCHAR (100)     NOT NULL,
    [AssetTrackingPayloadTypeName]  NVARCHAR (100)     NULL,
    [AssetTrackingPayloadTypeGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblAssetTrackingPayloadType_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblAssetTrackingPayloadType_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblAssetTrackingPayloadType_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblAssetTrackingPayloadType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblAssetTrackingPayloadType_UpdatedBy] DEFAULT (suser_sname()) NULL,
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
