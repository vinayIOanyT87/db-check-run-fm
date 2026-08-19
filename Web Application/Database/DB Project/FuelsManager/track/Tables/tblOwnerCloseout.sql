/* {CheckPoint: CREATING TRACKING TABLE for tblOwnerCloseout } */

/****** Object:  Table [track].[tblOwnerCloseout]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblOwnerCloseout]
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
	[PK_OwnerCloseoutGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblOwnerCloseout_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerCloseout_PK_OwnerCloseoutGuid] ON [track].[tblOwnerCloseout]
(
    [PK_OwnerCloseoutGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerCloseout_InsertedRowVersion] ON [track].[tblOwnerCloseout]
(
    [InsertedRowVersion] ASC,
    [PK_OwnerCloseoutGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerCloseout_UpdatedRowVersion] ON [track].[tblOwnerCloseout]
(
    [UpdatedRowVersion] ASC,
    [PK_OwnerCloseoutGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerCloseout_DeletedRowVersion] ON [track].[tblOwnerCloseout]
(
    [DeletedRowVersion] ASC,
    [PK_OwnerCloseoutGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblOwnerCloseout_PK_OwnerCloseoutGuid_Sync] ON [track].[tblOwnerCloseout]
(
	[PK_OwnerCloseoutGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblOwnerCloseout_DeletedRowVersionUpdate_ForSync
   ON track.tblOwnerCloseout
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
        FROM track.tblOwnerCloseout t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END