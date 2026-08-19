 --Creating Sync Tracking Table for tblEmailTemplateToAlarmAndEvent
CREATE TABLE [track].[tblEmailTemplateToAlarmAndEvent]
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
	[PK_EmailTemplateToAlarmAndEventGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEmailTemplateToAlarmAndEvent_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplateToAlarmAndEvent_PK_EmailTemplateToAlarmAndEventGuid] ON [track].[tblEmailTemplateToAlarmAndEvent]
(
	[PK_EmailTemplateToAlarmAndEventGuid] ASC
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplateToAlarmAndEvent_InsertedRowVersion] ON [track].[tblEmailTemplateToAlarmAndEvent]
(
	[InsertedRowVersion] ASC,
	[PK_EmailTemplateToAlarmAndEventGuid],
	[InsertedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplateToAlarmAndEvent_UpdatedRowVersion] ON [track].[tblEmailTemplateToAlarmAndEvent]
(
	[UpdatedRowVersion] ASC,
	[PK_EmailTemplateToAlarmAndEventGuid],
	[UpdatedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplateToAlarmAndEvent_DeletedRowVersion] ON [track].[tblEmailTemplateToAlarmAndEvent]
(
	[DeletedRowVersion] ASC,
	[PK_EmailTemplateToAlarmAndEventGuid],
	[DeletedContext]
)
GO
 
CREATE TRIGGER track.trg_insupd_tblEmailTemplateToAlarmAndEvent_DeletedRowVersionUpdate_ForSync
   ON track.tblEmailTemplateToAlarmAndEvent
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
        FROM track.tblEmailTemplateToAlarmAndEvent t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END
GO
 