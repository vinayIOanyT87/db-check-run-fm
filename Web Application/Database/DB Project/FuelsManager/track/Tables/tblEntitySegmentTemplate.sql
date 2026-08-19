/* {CheckPoint: CREATING TRACKING TABLE for tblEntitySegmentTemplate } */

/****** Object:  Table [track].[tblEntitySegmentTemplate]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntitySegmentTemplate]
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
	[PK_EntitySegmentTemplateGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntitySegmentTemplate_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntitySegmentTemplate_PK_EntitySegmentTemplateGuid] ON [track].[tblEntitySegmentTemplate]
(
    [PK_EntitySegmentTemplateGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntitySegmentTemplate_InsertedRowVersion] ON [track].[tblEntitySegmentTemplate]
(
    [InsertedRowVersion] ASC,
    [PK_EntitySegmentTemplateGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntitySegmentTemplate_UpdatedRowVersion] ON [track].[tblEntitySegmentTemplate]
(
    [UpdatedRowVersion] ASC,
    [PK_EntitySegmentTemplateGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntitySegmentTemplate_DeletedRowVersion] ON [track].[tblEntitySegmentTemplate]
(
    [DeletedRowVersion] ASC,
    [PK_EntitySegmentTemplateGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntitySegmentTemplate_PK_EntitySegmentTemplateGuid_Sync] ON [track].[tblEntitySegmentTemplate]
(
	[PK_EntitySegmentTemplateGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntitySegmentTemplate_DeletedRowVersionUpdate_ForSync
   ON track.tblEntitySegmentTemplate
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
        FROM track.tblEntitySegmentTemplate t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END