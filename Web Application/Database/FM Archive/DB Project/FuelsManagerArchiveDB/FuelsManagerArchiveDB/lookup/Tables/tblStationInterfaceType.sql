/*

	DROP TABLE [lookup].[tblStationInterfaceType]

*/
CREATE TABLE [lookup].[tblStationInterfaceType] (
    [StationInterfaceTypeIndex] INT                NOT NULL,
    [StationInterfaceTypeCode]  NVARCHAR (100)     NOT NULL,
    [StationInterfaceTypeName]  NVARCHAR (100)     NULL,
    [StationInterfaceTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NULL,
    [CreatedBy]                 [dbo].[udtUserID]  NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  NULL,
    [_RowVersion]               ROWVERSION         NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblStationInterfaceType] PRIMARY KEY NONCLUSTERED ([StationInterfaceTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblStationInterfaceType_StationInterfaceTypeGuid]
    ON [lookup].[tblStationInterfaceType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblStationInterfaceType_ClusterIdx]
    ON [lookup].[tblStationInterfaceType]([_ClusterIdx] ASC);