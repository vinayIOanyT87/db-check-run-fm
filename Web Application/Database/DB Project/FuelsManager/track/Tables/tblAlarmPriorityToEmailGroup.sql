/* {CheckPoint: CREATING TRACKING TABLE for tblAlarmPriorityToEmailGroup } */

/****** Object:  Table [track].[tblAlarmPriorityToEmailGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAlarmPriorityToEmailGroup]
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
	[PK_AlarmPriorityEmailGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAlarmPriorityToEmailGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorityToEmailGroup_PK_AlarmPriorityEmailGroupGuid] ON [track].[tblAlarmPriorityToEmailGroup]
(
    [PK_AlarmPriorityEmailGroupGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorityToEmailGroup_InsertedRowVersion] ON [track].[tblAlarmPriorityToEmailGroup]
(
    [InsertedRowVersion] ASC,
    [PK_AlarmPriorityEmailGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorityToEmailGroup_UpdatedRowVersion] ON [track].[tblAlarmPriorityToEmailGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_AlarmPriorityEmailGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorityToEmailGroup_DeletedRowVersion] ON [track].[tblAlarmPriorityToEmailGroup]
(
    [DeletedRowVersion] ASC,
    [PK_AlarmPriorityEmailGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAlarmPriorityToEmailGroup_PK_AlarmPriorityEmailGroupGuid_Sync] ON [track].[tblAlarmPriorityToEmailGroup]
(
	[PK_AlarmPriorityEmailGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAlarmPriorityToEmailGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblAlarmPriorityToEmailGroup
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
        FROM track.tblAlarmPriorityToEmailGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END