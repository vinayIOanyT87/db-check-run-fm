/*

	DROP TABLE [lookup].[tblCompanyCrossReferenceType]

*/

CREATE TABLE [lookup].[tblCompanyCrossReferenceType] (
    [CompanyCrossReferenceTypeIndex] INT                NOT NULL,
    [ReferenceTypeName]              NVARCHAR (100)     NOT NULL,
    [CompanyCrossReferenceTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) NULL,
    [CreatedBy]                      [dbo].[udtUserID]  NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCompanyCrossReferenceType] PRIMARY KEY NONCLUSTERED ([CompanyCrossReferenceTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCompanyCrossReferenceType_ClusterIdx]
    ON [lookup].[tblCompanyCrossReferenceType]([_ClusterIdx] ASC);