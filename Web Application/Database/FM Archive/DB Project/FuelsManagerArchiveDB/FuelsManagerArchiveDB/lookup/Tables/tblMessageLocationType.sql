/*

	DROP TABLE [lookup].[tblMessageLocationType]

*/
CREATE TABLE [lookup].[tblMessageLocationType] (
    [MessageLocationTypeIndex] INT                NOT NULL,
    [MessageLocationTypeCode]  NVARCHAR (100)     NOT NULL,
    [MessageLocationTypeName]  NVARCHAR (100)     NULL,
    [MessageLocationTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMessageLocationType] PRIMARY KEY NONCLUSTERED ([MessageLocationTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblMessageLocationType_MessageLocationTypeGuid]
    ON [lookup].[tblMessageLocationType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMessageLocationType_ClusterIdx]
    ON [lookup].[tblMessageLocationType]([_ClusterIdx] ASC);