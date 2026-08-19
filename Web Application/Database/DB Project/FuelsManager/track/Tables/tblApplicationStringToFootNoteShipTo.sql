/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToFootNoteShipTo } */

/****** Object:  Table [track].[tblApplicationStringToFootNoteShipTo]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblApplicationStringToFootNoteShipTo]
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
	[PK_ApplicationStringToFootNoteShipToGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToFootNoteShipTo_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipTo_PK_ApplicationStringToFootNoteShipToGuid] ON [track].[tblApplicationStringToFootNoteShipTo]
(
    [PK_ApplicationStringToFootNoteShipToGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipTo_InsertedRowVersion] ON [track].[tblApplicationStringToFootNoteShipTo]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteShipToGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipTo_UpdatedRowVersion] ON [track].[tblApplicationStringToFootNoteShipTo]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteShipToGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipTo_DeletedRowVersion] ON [track].[tblApplicationStringToFootNoteShipTo]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteShipToGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteShipTo_PK_ApplicationStringToFootNoteShipToGuid_Sync] ON [track].[tblApplicationStringToFootNoteShipTo]
(
	[PK_ApplicationStringToFootNoteShipToGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToFootNoteShipTo_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToFootNoteShipTo
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
        FROM track.tblApplicationStringToFootNoteShipTo t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END