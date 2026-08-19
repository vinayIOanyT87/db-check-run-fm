/*

	DROP TABLE [lookup].[tblServiceType]

*/
CREATE TABLE [lookup].[tblServiceType] (
    [ServiceTypeIndex] INT                NOT NULL,
    [ServiceTypeCode]  NVARCHAR (100)     NOT NULL,
    [ServiceTypeName]  NVARCHAR (100)     NULL,
    [ServiceTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblServiceType] PRIMARY KEY NONCLUSTERED ([ServiceTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblServiceType_ServiceTypeGuid]
    ON [lookup].[tblServiceType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblServiceType_ClusterIdx]
    ON [lookup].[tblServiceType]([_ClusterIdx] ASC);