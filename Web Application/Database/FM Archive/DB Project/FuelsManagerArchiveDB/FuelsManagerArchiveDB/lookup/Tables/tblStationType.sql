/*

	DROP TABLE [lookup].[tblStationType]

*/
CREATE TABLE [lookup].[tblStationType] (
    [StationTypeIndex] INT                NOT NULL,
    [StationTypeCode]  NVARCHAR (100)     NOT NULL,
    [StationTypeName]  NVARCHAR (100)     NULL,
    [StationTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblStationType] PRIMARY KEY NONCLUSTERED ([StationTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblStationType_StationTypeGuid]
    ON [lookup].[tblStationType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblStationType_ClusterIdx]
    ON [lookup].[tblStationType]([_ClusterIdx] ASC);