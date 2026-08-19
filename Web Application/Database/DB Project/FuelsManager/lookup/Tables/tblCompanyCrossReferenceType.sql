CREATE TABLE [lookup].[tblCompanyCrossReferenceType] (
    [CompanyCrossReferenceTypeIndex] INT                NOT NULL,
    [ReferenceTypeName]              NVARCHAR (100)     NOT NULL,
    [CompanyCrossReferenceTypeGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblCompanyCrossReferenceType_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblCompanyCrossReferenceType_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblCompanyCrossReferenceType_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblCompanyCrossReferenceType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblCompanyCrossReferenceType_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCompanyCrossReferenceType] PRIMARY KEY NONCLUSTERED ([CompanyCrossReferenceTypeIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCompanyCrossReferenceType_ClusterIdx]
    ON [lookup].[tblCompanyCrossReferenceType]([_ClusterIdx] ASC);

