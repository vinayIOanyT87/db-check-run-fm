/* {CheckPoint: CREATING TRACKING TABLE for tblWatchdogMode } */

/****** Object:  Table [track].[tblWatchdogMode]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblWatchdogMode]
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
	[PK_WatchdogModeIndex] [tinyint] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblWatchdogMode_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWatchdogMode_PK_WatchdogModeIndex] ON [track].[tblWatchdogMode]
(
    [PK_WatchdogModeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWatchdogMode_InsertedRowVersion] ON [track].[tblWatchdogMode]
(
    [InsertedRowVersion] ASC,
    [PK_WatchdogModeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWatchdogMode_UpdatedRowVersion] ON [track].[tblWatchdogMode]
(
    [UpdatedRowVersion] ASC,
    [PK_WatchdogModeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWatchdogMode_DeletedRowVersion] ON [track].[tblWatchdogMode]
(
    [DeletedRowVersion] ASC,
    [PK_WatchdogModeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblWatchdogMode_PK_WatchdogModeIndex_Sync] ON [track].[tblWatchdogMode]
(
	[PK_WatchdogModeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblWatchdogMode_DeletedRowVersionUpdate_ForSync
   ON track.tblWatchdogMode
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
        FROM track.tblWatchdogMode t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END