/* {CheckPoint: CREATING TRACKING TABLE for tblAlarmTestTemplate } */

/****** Object:  Table [track].[tblAlarmTestTemplate]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAlarmTestTemplate]
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
	[PK_AlarmTestTemplateGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAlarmTestTemplate_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTestTemplate_PK_AlarmTestTemplateGuid] ON [track].[tblAlarmTestTemplate]
(
    [PK_AlarmTestTemplateGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTestTemplate_InsertedRowVersion] ON [track].[tblAlarmTestTemplate]
(
    [InsertedRowVersion] ASC,
    [PK_AlarmTestTemplateGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTestTemplate_UpdatedRowVersion] ON [track].[tblAlarmTestTemplate]
(
    [UpdatedRowVersion] ASC,
    [PK_AlarmTestTemplateGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTestTemplate_DeletedRowVersion] ON [track].[tblAlarmTestTemplate]
(
    [DeletedRowVersion] ASC,
    [PK_AlarmTestTemplateGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTestTemplate_PK_AlarmTestTemplateGuid_Sync] ON [track].[tblAlarmTestTemplate]
(
	[PK_AlarmTestTemplateGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAlarmTestTemplate_DeletedRowVersionUpdate_ForSync
   ON track.tblAlarmTestTemplate
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
        FROM track.tblAlarmTestTemplate t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END