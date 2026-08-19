/* {CheckPoint: CREATING TRACKING TABLE for tblAlarmPriorities } */

/****** Object:  Table [track].[tblAlarmPriorities]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAlarmPriorities]
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
	[PK_AlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAlarmPriorities_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorities_PK_AlarmPriorityGuid] ON [track].[tblAlarmPriorities]
(
    [PK_AlarmPriorityGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorities_InsertedRowVersion] ON [track].[tblAlarmPriorities]
(
    [InsertedRowVersion] ASC,
    [PK_AlarmPriorityGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorities_UpdatedRowVersion] ON [track].[tblAlarmPriorities]
(
    [UpdatedRowVersion] ASC,
    [PK_AlarmPriorityGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorities_DeletedRowVersion] ON [track].[tblAlarmPriorities]
(
    [DeletedRowVersion] ASC,
    [PK_AlarmPriorityGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorities_PK_AlarmPriorityGuid_Sync] ON [track].[tblAlarmPriorities]
(
	[PK_AlarmPriorityGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAlarmPriorities_DeletedRowVersionUpdate_ForSync
   ON track.tblAlarmPriorities
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
        FROM track.tblAlarmPriorities t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END