/* {CheckPoint: CREATING TRACKING TABLE for tblTransactionLinks } */

/****** Object:  Table [track].[tblTransactionLinks]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTransactionLinks]
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
	[PK_TransactionLinkGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTransactionLinks_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLinks_PK_TransactionLinkGuid] ON [track].[tblTransactionLinks]
(
    [PK_TransactionLinkGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLinks_InsertedRowVersion] ON [track].[tblTransactionLinks]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionLinkGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLinks_UpdatedRowVersion] ON [track].[tblTransactionLinks]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionLinkGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLinks_DeletedRowVersion] ON [track].[tblTransactionLinks]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionLinkGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLinks_PK_TransactionLinkGuid_Sync] ON [track].[tblTransactionLinks]
(
	[PK_TransactionLinkGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTransactionLinks_DeletedRowVersionUpdate_ForSync
   ON track.tblTransactionLinks
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
        FROM track.tblTransactionLinks t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END