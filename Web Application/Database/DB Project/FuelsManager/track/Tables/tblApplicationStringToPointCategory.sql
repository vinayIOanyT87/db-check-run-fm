/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToPointCategory } */

/****** Object:  Table [track].[tblApplicationStringToPointCategory]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblApplicationStringToPointCategory]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblApplicationStringToPointCategory]
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
	[PK_ApplicationStringToPointCategoryGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToPointCategory_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToPointCategory_PK_ApplicationStringToPointCategoryGuid] ON [track].[tblApplicationStringToPointCategory]
(
    [PK_ApplicationStringToPointCategoryGuid],
	 [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToPointCategory_InsertedRowVersion] ON [track].[tblApplicationStringToPointCategory]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToPointCategoryGuid] ASC,
    [InsertedContext] ASC
)


GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToPointCategory_UpdatedRowVersion] ON [track].[tblApplicationStringToPointCategory]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToPointCategoryGuid] ASC,
    [UpdatedContext] ASC
)


GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToPointCategory_DeletedRowVersion] ON [track].[tblApplicationStringToPointCategory]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToPointCategoryGuid] ASC,
    [DeletedContext] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToPointCategory_PK_ApplicationStringToPointCategoryGuid_Sync] ON [track].[tblApplicationStringToPointCategory]
(
	[PK_ApplicationStringToPointCategoryGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToPointCategory_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToPointCategory
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
        FROM track.tblApplicationStringToPointCategory t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END