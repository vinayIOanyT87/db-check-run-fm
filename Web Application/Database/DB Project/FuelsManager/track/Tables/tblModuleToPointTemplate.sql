/* {CheckPoint: CREATING TRACKING TABLE for tblModuleToPointTemplate } */

/****** Object:  Table [track].[tblModuleToPointTemplate]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblModuleToPointTemplate]
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
	[PK_ModuleToPointTemplateGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblModuleToPointTemplate_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblModuleToPointTemplate_PK_ModuleToPointTemplateGuid] ON [track].[tblModuleToPointTemplate]
(
    [PK_ModuleToPointTemplateGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblModuleToPointTemplate_InsertedRowVersion] ON [track].[tblModuleToPointTemplate]
(
    [InsertedRowVersion] ASC,
    [PK_ModuleToPointTemplateGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblModuleToPointTemplate_UpdatedRowVersion] ON [track].[tblModuleToPointTemplate]
(
    [UpdatedRowVersion] ASC,
    [PK_ModuleToPointTemplateGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblModuleToPointTemplate_DeletedRowVersion] ON [track].[tblModuleToPointTemplate]
(
    [DeletedRowVersion] ASC,
    [PK_ModuleToPointTemplateGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblModuleToPointTemplate_PK_ModuleToPointTemplateGuid_Sync] ON [track].[tblModuleToPointTemplate]
(
	[PK_ModuleToPointTemplateGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblModuleToPointTemplate_DeletedRowVersionUpdate_ForSync
   ON track.tblModuleToPointTemplate
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
        FROM track.tblModuleToPointTemplate t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END