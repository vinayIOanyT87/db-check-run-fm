CREATE TABLE [sync].[tblSchemaChangeHistory] (
    [SchemaChangeHistoryGuid] UNIQUEIDENTIFIER   NOT NULL,
    [Version]                 NVARCHAR (80)      NOT NULL,
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSchemaChangeHistory_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblSchemaChangeHistory_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSchemaChangeHistory_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblSchemaChangeHistory_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSchemaChangeHistory] PRIMARY KEY NONCLUSTERED ([SchemaChangeHistoryGuid] ASC)
);






GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSchemaChangeHistory_Version]
    ON [sync].[tblSchemaChangeHistory]([Version] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_tblSchemaChangeHistory_CreatedDate]
    ON [sync].[tblSchemaChangeHistory]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSchemaChangeHistory_ClusterIdx]
    ON [sync].[tblSchemaChangeHistory]([_ClusterIdx] ASC);

