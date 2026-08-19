CREATE TABLE [lookup].[tblAssetTrackingMessageState]
(
	[AssetTrackingMessageStateIndex] INT                NOT NULL,
	[AssetTrackingMessageStateCode]  NVARCHAR (100)     NOT NULL,
    [AssetTrackingMessageStateName]  NVARCHAR (100)     NULL,
    [AssetTrackingMessageStateGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblAssetTrackingMessageState_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblAssetTrackingMessageState_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblAssetTrackingMessageState_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblAssetTrackingMessageState_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblAssetTrackingMessageState_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblAssetTrackingMessageState] PRIMARY KEY NONCLUSTERED ([AssetTrackingMessageStateIndex] ASC)
);

GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblAssetTrackingMessageState_AssetTrackingMessageStateGuid]
    ON [lookup].[tblAssetTrackingMessageState]([CreatedDate] ASC);

	GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAssetTrackingMessageState_ClusterIdx]
    ON [lookup].[tblAssetTrackingMessageState]([_ClusterIdx] ASC);