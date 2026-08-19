/* {CheckPoint: CREATING TRACKING TABLE for tblPointTemplateTagAlarmStatus } */

/****** Object:  Table [track].[tblPointTemplateTagAlarmStatus]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointTemplateTagAlarmStatus]
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
	[PK_PointTemplateTagAlarmStatusGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointTemplateTagAlarmStatus_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateTagAlarmStatus_PK_PointTemplateTagAlarmStatusGuid] ON [track].[tblPointTemplateTagAlarmStatus]
(
    [PK_PointTemplateTagAlarmStatusGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateTagAlarmStatus_InsertedRowVersion] ON [track].[tblPointTemplateTagAlarmStatus]
(
    [InsertedRowVersion] ASC,
    [PK_PointTemplateTagAlarmStatusGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateTagAlarmStatus_UpdatedRowVersion] ON [track].[tblPointTemplateTagAlarmStatus]
(
    [UpdatedRowVersion] ASC,
    [PK_PointTemplateTagAlarmStatusGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateTagAlarmStatus_DeletedRowVersion] ON [track].[tblPointTemplateTagAlarmStatus]
(
    [DeletedRowVersion] ASC,
    [PK_PointTemplateTagAlarmStatusGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateTagAlarmStatus_PK_PointTemplateTagAlarmStatusGuid_Sync] ON [track].[tblPointTemplateTagAlarmStatus]
(
	[PK_PointTemplateTagAlarmStatusGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointTemplateTagAlarmStatus_DeletedRowVersionUpdate_ForSync
   ON track.tblPointTemplateTagAlarmStatus
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
        FROM track.tblPointTemplateTagAlarmStatus t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END