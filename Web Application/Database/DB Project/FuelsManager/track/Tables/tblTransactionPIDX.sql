/* {CheckPoint: CREATING TRACKING TABLE for tblTransactionPIDX } */

/****** Object:  Table [track].[tblTransactionPIDX]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTransactionPIDX]
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
	[PK_TransactionPIDXGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTransactionPIDX_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionPIDX_PK_TransactionPIDXGuid] ON [track].[tblTransactionPIDX]
(
    [PK_TransactionPIDXGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionPIDX_InsertedRowVersion] ON [track].[tblTransactionPIDX]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionPIDXGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionPIDX_UpdatedRowVersion] ON [track].[tblTransactionPIDX]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionPIDXGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionPIDX_DeletedRowVersion] ON [track].[tblTransactionPIDX]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionPIDXGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTransactionPIDX_PK_TransactionPIDXGuid_Sync] ON [track].[tblTransactionPIDX]
(
	[PK_TransactionPIDXGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTransactionPIDX_DeletedRowVersionUpdate_ForSync
   ON track.tblTransactionPIDX
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
        FROM track.tblTransactionPIDX t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END