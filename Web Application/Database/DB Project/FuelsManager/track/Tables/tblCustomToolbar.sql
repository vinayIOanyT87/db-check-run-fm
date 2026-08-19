/* {CheckPoint: CREATING TRACKING TABLE for tblCustomToolbar } */

/****** Object:  Table [track].[tblCustomToolbar]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCustomToolbar]
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
	[PK_CustomToolbarGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCustomToolbar_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbar_PK_CustomToolbarGuid] ON [track].[tblCustomToolbar]
(
    [PK_CustomToolbarGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbar_InsertedRowVersion] ON [track].[tblCustomToolbar]
(
    [InsertedRowVersion] ASC,
    [PK_CustomToolbarGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbar_UpdatedRowVersion] ON [track].[tblCustomToolbar]
(
    [UpdatedRowVersion] ASC,
    [PK_CustomToolbarGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbar_DeletedRowVersion] ON [track].[tblCustomToolbar]
(
    [DeletedRowVersion] ASC,
    [PK_CustomToolbarGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbar_PK_CustomToolbarGuid_Sync] ON [track].[tblCustomToolbar]
(
	[PK_CustomToolbarGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCustomToolbar_DeletedRowVersionUpdate_ForSync
   ON track.tblCustomToolbar
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
        FROM track.tblCustomToolbar t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END