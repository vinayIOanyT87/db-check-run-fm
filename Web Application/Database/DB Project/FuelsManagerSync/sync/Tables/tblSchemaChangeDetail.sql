CREATE TABLE [sync].[tblSchemaChangeDetail] (
    [SchemaChangeDetailGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [SchemaChangeHistoryGuid] UNIQUEIDENTIFIER   NOT NULL,
    [SchemaObjectTypeIndex]   BIGINT             CONSTRAINT [DF_tblSchemaChangeDetail_SchemaObjectTypeIndex] DEFAULT ((1)) NOT NULL,
    [SchemaName]              NVARCHAR (64)      NOT NULL,
    [ObjectName]              NVARCHAR (512)     NOT NULL,
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSchemaChangeDetail_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblSchemaChangeDetail_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSchemaChangeDetail_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblSchemaChangeDetail_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSchemaChangeDetail] PRIMARY KEY NONCLUSTERED ([SchemaChangeDetailGuid] ASC),
    CONSTRAINT [FK_tblSchemaChangeDetail_tblSchemaChangeHistory] FOREIGN KEY ([SchemaChangeHistoryGuid]) REFERENCES [sync].[tblSchemaChangeHistory] ([SchemaChangeHistoryGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblSchemaChangeDetail_CreatedDate]
    ON [sync].[tblSchemaChangeDetail]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSchemaChangeDetail_ClusterIdx]
    ON [sync].[tblSchemaChangeDetail]([_ClusterIdx] ASC);

