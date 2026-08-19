/*

	DROP TABLE [lookup].[tblQualificationType]

*/
CREATE TABLE [lookup].[tblQualificationType] (
    [QualificationTypeIndex] INT                NOT NULL,
    [QualificationTypeCode]  NVARCHAR (100)     NOT NULL,
    [QualificationTypeName]  NVARCHAR (100)     NULL,
    [QualificationTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblQualificationType] PRIMARY KEY NONCLUSTERED ([QualificationTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblQualificationType_QualificationTypeGuid]
    ON [lookup].[tblQualificationType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblQualificationType_ClusterIdx]
    ON [lookup].[tblQualificationType]([_ClusterIdx] ASC);