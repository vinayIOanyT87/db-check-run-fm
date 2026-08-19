/*

	DROP TABLE [lookup].[tblMajorCorrectionType]

*/
CREATE TABLE [lookup].[tblMajorCorrectionType] (
    [MajorCorrectionTypeIndex] INT                NOT NULL,
    [MajorCorrectionTypeCode]  NVARCHAR (100)     NOT NULL,
    [MajorCorrectionTypeName]  NVARCHAR (100)     NULL,
    [MajorCorrectionTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMajorCorrectionType] PRIMARY KEY NONCLUSTERED ([MajorCorrectionTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblMajorCorrectionType_MajorCorrectionTypeGuid]
    ON [lookup].[tblMajorCorrectionType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMajorCorrectionType_ClusterIdx]
    ON [lookup].[tblMajorCorrectionType]([_ClusterIdx] ASC);