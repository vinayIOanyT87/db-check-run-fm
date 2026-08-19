/* {CheckPoint: CREATING TRACKING TABLE for tblLedgerAggregateColumnToTransactionAlias } */

/****** Object:  Table [track].[tblLedgerAggregateColumnToTransactionAlias]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblLedgerAggregateColumnToTransactionAlias]
( 
	[ChangeIndex] [bigint] IDENTITY(1,1) NOT NULL,
	[InsertedDate] [datetimeoffset](7) NOT NULL,
	[InsertedContext] [varbinary](128) NULL,
	[InsertedRowVersion] [varbinary](8) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[UpdatedContext] [varbinary](128) NULL,
	[UpdatedRowVersion] [varbinary](8) NULL,
	[DeletedDate] [datetimeoffset](7) NULL,
	[DeletedContext] [varbinary](128) NULL,
	[DeletedRowVersion] [varbinary](8) NULL,
	[CurrentSiteGuid] [uniqueidentifier] NULL,
	[PreviousSiteGuid] [uniqueidentifier] NULL,
	[PK_LedgerAggregateColumnToTransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblLedgerAggregateColumnToTransactionAlias_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblLedgerAggregateColumnToTransactionAlias_PK_LedgerAggregateColumnToTransactionAliasGuid] ON [track].[tblLedgerAggregateColumnToTransactionAlias]
(
    [PK_LedgerAggregateColumnToTransactionAliasGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblLedgerAggregateColumnToTransactionAlias_InsertedRowVersion] ON [track].[tblLedgerAggregateColumnToTransactionAlias]
(
    [InsertedRowVersion] ASC,
    [PK_LedgerAggregateColumnToTransactionAliasGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblLedgerAggregateColumnToTransactionAlias_UpdatedRowVersion] ON [track].[tblLedgerAggregateColumnToTransactionAlias]
(
    [UpdatedRowVersion] ASC,
    [PK_LedgerAggregateColumnToTransactionAliasGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblLedgerAggregateColumnToTransactionAlias_DeletedRowVersion] ON [track].[tblLedgerAggregateColumnToTransactionAlias]
(
    [DeletedRowVersion] ASC,
    [PK_LedgerAggregateColumnToTransactionAliasGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblLedgerAggregateColumnToTransactionAlias_PK_LedgerAggregateColumnToTransactionAliasGuid_Sync] ON [track].[tblLedgerAggregateColumnToTransactionAlias]
(
	[PK_LedgerAggregateColumnToTransactionAliasGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblLedgerAggregateColumnToTransactionAlias_DeletedRowVersionUpdate_ForSync
   ON track.tblLedgerAggregateColumnToTransactionAlias
   AFTER UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    IF ( UPDATE( DeletedDate ) )
    BEGIN
        UPDATE t
            SET DeletedRowVersion = convert(varbinary(8), i._RowVersion)
        FROM track.tblLedgerAggregateColumnToTransactionAlias t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END