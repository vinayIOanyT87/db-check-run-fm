/*

	DROP TABLE [lookup].[tblAssetTrackingMessageState]

*/
CREATE TABLE [lookup].[tblAssetTrackingMessageState]
(
	[AssetTrackingMessageStateIndex] INT                NOT NULL,
	[AssetTrackingMessageStateCode]  NVARCHAR (100)     NOT NULL,
    [AssetTrackingMessageStateName]  NVARCHAR (100)     NULL,
    [AssetTrackingMessageStateGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
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