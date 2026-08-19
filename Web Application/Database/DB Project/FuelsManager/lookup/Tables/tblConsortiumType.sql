CREATE TABLE [lookup].[tblConsortiumType]
(    
    [ConsortiumTypeIndex] INT                     NOT NULL,
    [ConsortiumTypeCode]  NVARCHAR (100)          NOT NULL,
    [ConsortiumTypeName]  NVARCHAR (100)          NULL,
    [ConsortiumTypeIndexGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblConsortiumType_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblConsortiumType_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblConsortiumType_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblConsortiumType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblConsortiumType_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblConsortiumType] PRIMARY KEY NONCLUSTERED ([ConsortiumTypeIndex] ASC)
)

GO