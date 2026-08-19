/* {CheckPoint: CREATING TRACKING TABLE for tblGroupToLedgerView } */

/****** Object:  Table [track].[tblGroupToLedgerView]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblGroupToLedgerView]
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
	[PK_GroupToLedgerViewGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblGroupToLedgerView_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToLedgerView_PK_GroupToLedgerViewGuid] ON [track].[tblGroupToLedgerView]
(
    [PK_GroupToLedgerViewGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToLedgerView_InsertedRowVersion] ON [track].[tblGroupToLedgerView]
(
    [InsertedRowVersion] ASC,
    [PK_GroupToLedgerViewGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToLedgerView_UpdatedRowVersion] ON [track].[tblGroupToLedgerView]
(
    [UpdatedRowVersion] ASC,
    [PK_GroupToLedgerViewGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToLedgerView_DeletedRowVersion] ON [track].[tblGroupToLedgerView]
(
    [DeletedRowVersion] ASC,
    [PK_GroupToLedgerViewGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGroupToLedgerView_PK_GroupToLedgerViewGuid_Sync] ON [track].[tblGroupToLedgerView]
(
	[PK_GroupToLedgerViewGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblGroupToLedgerView_DeletedRowVersionUpdate_ForSync
   ON track.tblGroupToLedgerView
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
        FROM track.tblGroupToLedgerView t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END