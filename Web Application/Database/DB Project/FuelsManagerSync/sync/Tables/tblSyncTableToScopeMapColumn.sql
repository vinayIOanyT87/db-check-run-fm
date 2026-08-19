CREATE TABLE [sync].[tblSyncTableToScopeMapColumn] (
    [SyncTableToScopeMapColumnGuid] UNIQUEIDENTIFIER   NOT NULL,
    [SyncTableToScopeMapGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [ColumnName]                    NVARCHAR (512)     NOT NULL,
    [ColumnIndex]                   INT                NOT NULL,
    [ColumnType]                    NVARCHAR (256)     NOT NULL,
    [ColumnSize]                    INT                NULL,
    [ColumnPrecision]               INT                NULL,
    [ColumnScale]                   INT                NULL,
    [IsNullableFlag]                BIT                CONSTRAINT [DF_tblSyncTableToScopeMapColumn_IsNullableFlag] DEFAULT ((0)) NOT NULL,
    [IsPrimaryKeyMemberFlag]        BIT                CONSTRAINT [DF_tblSyncTableToScopeMapColumn_IsPrimaryKeyMemberFlag] DEFAULT ((0)) NOT NULL,
    [IsIdentityColumnFlag]          BIT                CONSTRAINT [DF_tblSyncTableToScopeMapColumn_IsIdentityColumnFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTableToScopeMapColumn_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                     [dbo].[udtUserID]  NULL,
    [UpdatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTableToScopeMapColumn_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                     [dbo].[udtUserID]  NULL,
    [_RowVersion]                   ROWVERSION         NOT NULL,
    [_ClusterIdx]                   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncTableToScopeMapColumn] PRIMARY KEY NONCLUSTERED ([SyncTableToScopeMapColumnGuid] ASC),
    CONSTRAINT [FK_tblSyncTableToScopeMapColumn_tblSyncTableToScopeMap] FOREIGN KEY ([SyncTableToScopeMapGuid]) REFERENCES [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid])
);






GO



GO
CREATE NONCLUSTERED INDEX [IX_tblSyncTableToScopeMapColumn_CreatedDate]
    ON [sync].[tblSyncTableToScopeMapColumn]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncTableToScopeMapColumn_ClusterIdx]
    ON [sync].[tblSyncTableToScopeMapColumn]([_ClusterIdx] ASC);

