/* {CheckPoint: CREATING TRACKING TABLE for tblAlarmTemplate } */

/****** Object:  Table [track].[tblAlarmTemplate]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAlarmTemplate]
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
	[PK_AlarmTemplateGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAlarmTemplate_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTemplate_PK_AlarmTemplateGuid] ON [track].[tblAlarmTemplate]
(
    [PK_AlarmTemplateGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTemplate_InsertedRowVersion] ON [track].[tblAlarmTemplate]
(
    [InsertedRowVersion] ASC,
    [PK_AlarmTemplateGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTemplate_UpdatedRowVersion] ON [track].[tblAlarmTemplate]
(
    [UpdatedRowVersion] ASC,
    [PK_AlarmTemplateGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTemplate_DeletedRowVersion] ON [track].[tblAlarmTemplate]
(
    [DeletedRowVersion] ASC,
    [PK_AlarmTemplateGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAlarmTemplate_PK_AlarmTemplateGuid_Sync] ON [track].[tblAlarmTemplate]
(
	[PK_AlarmTemplateGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAlarmTemplate_DeletedRowVersionUpdate_ForSync
   ON track.tblAlarmTemplate
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
        FROM track.tblAlarmTemplate t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END