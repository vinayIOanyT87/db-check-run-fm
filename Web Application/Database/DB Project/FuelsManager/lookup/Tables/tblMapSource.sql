CREATE TABLE [lookup].[tblMapSource]
(
	[MapSourceIndex] INT                NOT NULL,
    [MapSourceCode]  NVARCHAR (100)     NOT NULL,
    [MapSourceName]  NVARCHAR (100)     NULL,
    [MapSourceGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblMapSource_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblMapSource_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblMapSource_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblMapSource_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblMapSource_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMapSource] PRIMARY KEY NONCLUSTERED ([MapSourceIndex] ASC)
);

GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblMapSource_MapSourceGuid]
    ON [lookup].[tblMapSource]([CreatedDate] ASC);

	GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMapSource_ClusterIdx]
    ON [lookup].[tblMapSource]([_ClusterIdx] ASC);
