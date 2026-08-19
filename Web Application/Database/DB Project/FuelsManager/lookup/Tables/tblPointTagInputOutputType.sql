CREATE TABLE [lookup].[tblPointTagInputOutputType]
(
    [PointTagInputOutputTypeIndex] INT                NOT NULL,
    [PointTagInputOutputTypeCode]  NVARCHAR (100)     NOT NULL,
    [PointTagInputOutputTypeName]  NVARCHAR (100)     NULL,
    [PointTagInputOutputTypeGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblPointTagInputOutputType_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblPointTagInputOutputType_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]         [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblPointTagInputOutputType_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblPointTagInputOutputType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblPointTagInputOutputType_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
	[_ClusterIdx]		BIGINT			   NOT NULL IDENTITY,
    CONSTRAINT [PK_lookup_tblPointTagInputOutputType] PRIMARY KEY NONCLUSTERED ([PointTagInputOutputTypeIndex] ASC)
)

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPointTagInputOutputType_ClusterIdx] 
	ON [lookup].[tblPointTagInputOutputType]([_ClusterIdx]);


GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblPointTagInputOutputType_PointTagInputOutputTypeGuid]
    ON [lookup].[tblPointTagInputOutputType]([CreatedDate] ASC);


GO

