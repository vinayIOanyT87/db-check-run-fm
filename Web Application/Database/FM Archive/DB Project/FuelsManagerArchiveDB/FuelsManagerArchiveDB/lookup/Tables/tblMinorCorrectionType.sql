/*

	DROP TABLE [lookup].[tblMinorCorrectionType]

*/
CREATE TABLE [lookup].[tblMinorCorrectionType] (
    [MinorCorrectionTypeIndex] INT                NOT NULL,
    [MinorCorrectionTypeCode]  NVARCHAR (100)     NOT NULL,
    [MinorCorrectionTypeName]  NVARCHAR (100)     NULL,
    [MinorCorrectionTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMinorCorrectionType] PRIMARY KEY NONCLUSTERED ([MinorCorrectionTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblMinorMinorCorrectionType_MinorCorrectionTypeGuid]
    ON [lookup].[tblMinorCorrectionType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMinorCorrectionType_ClusterIdx]
    ON [lookup].[tblMinorCorrectionType]([_ClusterIdx] ASC);