/* {CheckPoint: CREATING TRACKING TABLE for tblTransactionAliases } */

/****** Object:  Table [track].[tblTransactionAliases]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTransactionAliases]
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
	[PK_TransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTransactionAliases_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliases_PK_TransactionAliasGuid] ON [track].[tblTransactionAliases]
(
    [PK_TransactionAliasGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliases_InsertedRowVersion] ON [track].[tblTransactionAliases]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionAliasGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliases_UpdatedRowVersion] ON [track].[tblTransactionAliases]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionAliasGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliases_DeletedRowVersion] ON [track].[tblTransactionAliases]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionAliasGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliases_PK_TransactionAliasGuid_Sync] ON [track].[tblTransactionAliases]
(
	[PK_TransactionAliasGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTransactionAliases_DeletedRowVersionUpdate_ForSync
   ON track.tblTransactionAliases
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
        FROM track.tblTransactionAliases t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END