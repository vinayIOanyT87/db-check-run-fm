/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToFootNoteShipper } */

/****** Object:  Table [track].[tblApplicationStringToFootNoteShipper]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblApplicationStringToFootNoteShipper]
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
	[PK_ApplicationStringToFootNoteShipperGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToFootNoteShipper_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipper_PK_ApplicationStringToFootNoteShipperGuid] ON [track].[tblApplicationStringToFootNoteShipper]
(
    [PK_ApplicationStringToFootNoteShipperGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipper_InsertedRowVersion] ON [track].[tblApplicationStringToFootNoteShipper]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteShipperGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipper_UpdatedRowVersion] ON [track].[tblApplicationStringToFootNoteShipper]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteShipperGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipper_DeletedRowVersion] ON [track].[tblApplicationStringToFootNoteShipper]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteShipperGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipper_PK_ApplicationStringToFootNoteShipperGuid_Sync] ON [track].[tblApplicationStringToFootNoteShipper]
(
	[PK_ApplicationStringToFootNoteShipperGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToFootNoteShipper_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToFootNoteShipper
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
        FROM track.tblApplicationStringToFootNoteShipper t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END