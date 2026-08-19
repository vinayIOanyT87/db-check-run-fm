/* {CheckPoint: CREATING TRACKING TABLE for tblPointTemplateProperty } */

/****** Object:  Table [track].[tblPointTemplateProperty]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointTemplateProperty]
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
	[PK_PointTemplatePropertyGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointTemplateProperty_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateProperty_PK_PointTemplatePropertyGuid] ON [track].[tblPointTemplateProperty]
(
    [PK_PointTemplatePropertyGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateProperty_InsertedRowVersion] ON [track].[tblPointTemplateProperty]
(
    [InsertedRowVersion] ASC,
    [PK_PointTemplatePropertyGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateProperty_UpdatedRowVersion] ON [track].[tblPointTemplateProperty]
(
    [UpdatedRowVersion] ASC,
    [PK_PointTemplatePropertyGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateProperty_DeletedRowVersion] ON [track].[tblPointTemplateProperty]
(
    [DeletedRowVersion] ASC,
    [PK_PointTemplatePropertyGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointTemplateProperty_PK_PointTemplatePropertyGuid_Sync] ON [track].[tblPointTemplateProperty]
(
	[PK_PointTemplatePropertyGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointTemplateProperty_DeletedRowVersionUpdate_ForSync
   ON track.tblPointTemplateProperty
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
        FROM track.tblPointTemplateProperty t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END