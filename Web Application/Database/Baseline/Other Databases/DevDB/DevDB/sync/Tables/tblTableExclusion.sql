CREATE TABLE [sync].[tblTableExclusion] (
    [TableExclusionGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTableExclusion_TableExclusionGuid] DEFAULT (newid()) NOT NULL,
    [TableName]          NVARCHAR (256)     NULL,
    [CreatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblTableExclusion_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]          NVARCHAR (100)     NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblTableExclusion_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]          NVARCHAR (100)     NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblTableExclusion] PRIMARY KEY NONCLUSTERED ([TableExclusionGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTableExclusion_CreatedDate]
    ON [sync].[tblTableExclusion]([CreatedDate] ASC);

