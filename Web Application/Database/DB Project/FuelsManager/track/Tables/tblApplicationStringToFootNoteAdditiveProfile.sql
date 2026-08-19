/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToFootNoteAdditiveProfile } */

/****** Object:  Table [track].[tblApplicationStringToFootNoteAdditiveProfile]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblApplicationStringToFootNoteAdditiveProfile]
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
	[PK_ApplicationStringToFootNoteAdditiveProfileGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToFootNoteAdditiveProfile_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteAdditiveProfile_PK_ApplicationStringToFootNoteAdditiveProfileGuid] ON [track].[tblApplicationStringToFootNoteAdditiveProfile]
(
    [PK_ApplicationStringToFootNoteAdditiveProfileGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteAdditiveProfile_InsertedRowVersion] ON [track].[tblApplicationStringToFootNoteAdditiveProfile]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteAdditiveProfileGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteAdditiveProfile_UpdatedRowVersion] ON [track].[tblApplicationStringToFootNoteAdditiveProfile]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteAdditiveProfileGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteAdditiveProfile_DeletedRowVersion] ON [track].[tblApplicationStringToFootNoteAdditiveProfile]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToFootNoteAdditiveProfileGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToFootNoteAdditiveProfile_PK_ApplicationStringToFootNoteAdditiveProfileGuid_Sync] ON [track].[tblApplicationStringToFootNoteAdditiveProfile]
(
	[PK_ApplicationStringToFootNoteAdditiveProfileGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToFootNoteAdditiveProfile_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToFootNoteAdditiveProfile
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
        FROM track.tblApplicationStringToFootNoteAdditiveProfile t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END