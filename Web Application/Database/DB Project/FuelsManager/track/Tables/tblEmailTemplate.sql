CREATE TABLE [track].[tblEmailTemplate]
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
	[PK_EmailTemplateGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEmailTemplate_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplate_PK_EmailTemplateGuid] ON [track].[tblEmailTemplate]
(
	[PK_EmailTemplateGuid] ASC
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplate_InsertedRowVersion] ON [track].[tblEmailTemplate]
(
	[InsertedRowVersion] ASC,
	[PK_EmailTemplateGuid],
	[InsertedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplate_UpdatedRowVersion] ON [track].[tblEmailTemplate]
(
	[UpdatedRowVersion] ASC,
	[PK_EmailTemplateGuid],
	[UpdatedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblEmailTemplate_DeletedRowVersion] ON [track].[tblEmailTemplate]
(
	[DeletedRowVersion] ASC,
	[PK_EmailTemplateGuid],
	[DeletedContext]
)
GO
 
CREATE TRIGGER track.trg_insupd_tblEmailTemplate_DeletedRowVersionUpdate_ForSync
   ON track.tblEmailTemplate
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
        FROM track.tblEmailTemplate t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END
GO
 