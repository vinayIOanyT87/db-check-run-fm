/* {CheckPoint: CREATING TRACKING TABLE for tblAssociatedTransactionAliases } */

/****** Object:  Table [track].[tblAssociatedTransactionAliases]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAssociatedTransactionAliases]
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
	[PK_AssociatedTransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAssociatedTransactionAliases_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAssociatedTransactionAliases_PK_AssociatedTransactionAliasGuid] ON [track].[tblAssociatedTransactionAliases]
(
    [PK_AssociatedTransactionAliasGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAssociatedTransactionAliases_InsertedRowVersion] ON [track].[tblAssociatedTransactionAliases]
(
    [InsertedRowVersion] ASC,
    [PK_AssociatedTransactionAliasGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAssociatedTransactionAliases_UpdatedRowVersion] ON [track].[tblAssociatedTransactionAliases]
(
    [UpdatedRowVersion] ASC,
    [PK_AssociatedTransactionAliasGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAssociatedTransactionAliases_DeletedRowVersion] ON [track].[tblAssociatedTransactionAliases]
(
    [DeletedRowVersion] ASC,
    [PK_AssociatedTransactionAliasGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAssociatedTransactionAliases_PK_AssociatedTransactionAliasGuid_Sync] ON [track].[tblAssociatedTransactionAliases]
(
	[PK_AssociatedTransactionAliasGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAssociatedTransactionAliases_DeletedRowVersionUpdate_ForSync
   ON track.tblAssociatedTransactionAliases
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
        FROM track.tblAssociatedTransactionAliases t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END