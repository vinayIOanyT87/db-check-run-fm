/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAlarmPriorityToSite } */

/****** Object:  Table [track].[tblEntityAlarmPriorityToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAlarmPriorityToSite]
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
	[PK_AlarmPriorityToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAlarmPriorityToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmPriorityToSite_PK_AlarmPriorityToSiteGuid] ON [track].[tblEntityAlarmPriorityToSite]
(
    [PK_AlarmPriorityToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmPriorityToSite_InsertedRowVersion] ON [track].[tblEntityAlarmPriorityToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AlarmPriorityToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmPriorityToSite_UpdatedRowVersion] ON [track].[tblEntityAlarmPriorityToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AlarmPriorityToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmPriorityToSite_DeletedRowVersion] ON [track].[tblEntityAlarmPriorityToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AlarmPriorityToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAlarmPriorityToSite_PK_AlarmPriorityToSiteGuid_Sync] ON [track].[tblEntityAlarmPriorityToSite]
(
	[PK_AlarmPriorityToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAlarmPriorityToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAlarmPriorityToSite
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
        FROM track.tblEntityAlarmPriorityToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END