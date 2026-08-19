/* {CheckPoint: CREATING TRACKING TABLE for tblEntityTransactionAliasToSite } */

/****** Object:  Table [track].[tblEntityTransactionAliasToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityTransactionAliasToSite]
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
	[PK_TransactionAliasToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityTransactionAliasToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityTransactionAliasToSite_PK_TransactionAliasToSiteGuid] ON [track].[tblEntityTransactionAliasToSite]
(
    [PK_TransactionAliasToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityTransactionAliasToSite_InsertedRowVersion] ON [track].[tblEntityTransactionAliasToSite]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionAliasToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityTransactionAliasToSite_UpdatedRowVersion] ON [track].[tblEntityTransactionAliasToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionAliasToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityTransactionAliasToSite_DeletedRowVersion] ON [track].[tblEntityTransactionAliasToSite]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionAliasToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityTransactionAliasToSite_PK_TransactionAliasToSiteGuid_Sync] ON [track].[tblEntityTransactionAliasToSite]
(
	[PK_TransactionAliasToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityTransactionAliasToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityTransactionAliasToSite
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
        FROM track.tblEntityTransactionAliasToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END