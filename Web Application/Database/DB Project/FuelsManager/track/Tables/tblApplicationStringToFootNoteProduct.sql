/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToFootNoteProduct } */

/****** Object:  Table [track].[tblApplicationStringToFootNoteProduct]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblApplicationStringToFootNoteProduct]
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
	[PK_ApplicationStringToFootNoteProductGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToFootNoteProduct_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteProduct_PK_ApplicationStringToFootNoteProductGuid] ON [track].[tblApplicationStringToFootNoteProduct]
(
    [PK_ApplicationStringToFootNoteProductGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteProduct_InsertedRowVersion] ON [track].[tblApplicationStringToFootNoteProduct]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteProductGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteProduct_UpdatedRowVersion] ON [track].[tblApplicationStringToFootNoteProduct]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteProductGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteProduct_DeletedRowVersion] ON [track].[tblApplicationStringToFootNoteProduct]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteProductGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteProduct_PK_ApplicationStringToFootNoteProductGuid_Sync] ON [track].[tblApplicationStringToFootNoteProduct]
(
	[PK_ApplicationStringToFootNoteProductGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToFootNoteProduct_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToFootNoteProduct
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
        FROM track.tblApplicationStringToFootNoteProduct t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END