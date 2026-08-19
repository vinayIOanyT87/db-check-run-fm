/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAlarmAndEventToSite } */

/****** Object:  Table [track].[tblEntityAlarmAndEventToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAlarmAndEventToSite]
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
	[PK_AlarmAndEventToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAlarmAndEventToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmAndEventToSite_PK_AlarmAndEventToSiteGuid] ON [track].[tblEntityAlarmAndEventToSite]
(
    [PK_AlarmAndEventToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmAndEventToSite_InsertedRowVersion] ON [track].[tblEntityAlarmAndEventToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AlarmAndEventToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmAndEventToSite_UpdatedRowVersion] ON [track].[tblEntityAlarmAndEventToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AlarmAndEventToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmAndEventToSite_DeletedRowVersion] ON [track].[tblEntityAlarmAndEventToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AlarmAndEventToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmAndEventToSite_PK_AlarmAndEventToSiteGuid_Sync] ON [track].[tblEntityAlarmAndEventToSite]
(
	[PK_AlarmAndEventToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAlarmAndEventToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAlarmAndEventToSite
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
        FROM track.tblEntityAlarmAndEventToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END