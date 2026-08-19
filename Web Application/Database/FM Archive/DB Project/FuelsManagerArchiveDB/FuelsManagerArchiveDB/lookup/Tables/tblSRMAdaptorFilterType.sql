/*

	DROP TABLE [lookup].[tblSRMAdaptorFilterType]

*/
CREATE TABLE [lookup].[tblSRMAdaptorFilterType] (
    [SRMAdaptorFilterTypeGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblSRMAdaptorFilterType_SRMAdaptorFilterTypeGuid] DEFAULT (newid()) NOT NULL,
    [SRMAdaptorFilterType]     NVARCHAR (100)     NOT NULL,
    [SRMAdaptorFilterTypeCode] TINYINT            NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblSRMAdaptorFilterType] PRIMARY KEY NONCLUSTERED ([SRMAdaptorFilterTypeGuid] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_lookup_tblSRMAdaptorFilterType_SRMAdaptorFilterTypeCode]
    ON [lookup].[tblSRMAdaptorFilterType]([SRMAdaptorFilterTypeCode] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSRMAdaptorFilterType_ClusterIdx]
    ON [lookup].[tblSRMAdaptorFilterType]([_ClusterIdx] ASC);