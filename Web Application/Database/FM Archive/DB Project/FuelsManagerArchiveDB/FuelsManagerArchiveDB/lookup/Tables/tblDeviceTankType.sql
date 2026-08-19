/*

	DROP TABLE [lookup].[tblDeviceTankType]

*/
CREATE TABLE [lookup].[tblDeviceTankType]
(
	[DeviceTankTypeIndex] INT                NOT NULL,
	[DeviceTankTypeCode]  NVARCHAR (100)     NOT NULL,
    [DeviceTankTypeName]  NVARCHAR (100)     NULL,
    [DeviceTankTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblDeviceTankType] PRIMARY KEY NONCLUSTERED ([DeviceTankTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblDeviceTankType_DeviceTankTypeGuid]
    ON [lookup].[tblDeviceTankType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDeviceTankType_ClusterIdx]
    ON [lookup].[tblDeviceTankType]([_ClusterIdx] ASC);