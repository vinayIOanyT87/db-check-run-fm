/*

	DROP TABLE [lookup].[tblCompanyMapType]

*/

CREATE TABLE [lookup].[tblCompanyMapType] (
    [CompanyMapTypeIndex] INT                NOT NULL,
    [CompanyMapTypeCode]  NVARCHAR (100)     NOT NULL,
    [CompanyMapTypeName]  NVARCHAR (100)     NULL,
    [CompanyMapTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]         DATETIMEOFFSET (7) NULL,
    [CreatedBy]           [dbo].[udtUserID]  NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) NULL,
    [UpdatedBy]           [dbo].[udtUserID]  NULL,
    [_RowVersion]         ROWVERSION         NOT NULL,
    [_ClusterIdx]         BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCompanyMapType] PRIMARY KEY NONCLUSTERED ([CompanyMapTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblCompanyMapType_CompanyMapTypeGuid]
    ON [lookup].[tblCompanyMapType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCompanyMapType_ClusterIdx]
    ON [lookup].[tblCompanyMapType]([_ClusterIdx] ASC);