CREATE TABLE [lookup].[tblExternalStationSessionType]
(
	[ExternalStationSessionTypeIndex] INT NOT NULL , 
    [ExternalStationSessionTypeCode] NVARCHAR(100) NOT NULL, 
    [ExternalStationSessionTypeName] NVARCHAR(100) NOT NULL, 
    [ExternalStationSessionTypeGuid] UNIQUEIDENTIFIER NOT NULL, 
    [LongDescription] NVARCHAR(1024) NULL, 
    [CreatedBy] [dbo].[udtUserID] NOT NULL, 
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET NOT NULL, 
    [_RowVersion] TIMESTAMP NOT NULL, 
    [_ClusterIdx] BIGINT IDENTITY(1,1) NOT NULL,
    CONSTRAINT [PK_tblExternalStationSessionType] PRIMARY KEY NONCLUSTERED ([ExternalStationSessionTypeIndex])
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblExternalStationSessionType__ClusterIdx] ON [lookup].[tblExternalStationSessionType] ([_ClusterIdx])
GO

CREATE UNIQUE INDEX [IX_tblExternalStationSessionType_ExternalStationSessionTypeGuid] ON [lookup].[tblExternalStationSessionType] (ExternalStationSessionTypeGuid)
