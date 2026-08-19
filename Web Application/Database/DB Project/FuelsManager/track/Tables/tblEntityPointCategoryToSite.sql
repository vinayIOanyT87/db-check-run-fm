/* {CheckPoint: CREATING TRACKING TABLE for tblEntityPointCategoryToSite } */

/****** Object:  Table [track].[tblEntityPointCategoryToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityPointCategoryToSite]
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
	[PK_PointCategoryToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityPointCategoryToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointCategoryToSite_PK_PointCategoryToSiteGuid] ON [track].[tblEntityPointCategoryToSite]
(
    [PK_PointCategoryToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointCategoryToSite_InsertedRowVersion] ON [track].[tblEntityPointCategoryToSite]
(
    [InsertedRowVersion] ASC,
    [PK_PointCategoryToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointCategoryToSite_UpdatedRowVersion] ON [track].[tblEntityPointCategoryToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_PointCategoryToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointCategoryToSite_DeletedRowVersion] ON [track].[tblEntityPointCategoryToSite]
(
    [DeletedRowVersion] ASC,
    [PK_PointCategoryToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointCategoryToSite_PK_PointCategoryToSiteGuid_Sync] ON [track].[tblEntityPointCategoryToSite]
(
	[PK_PointCategoryToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityPointCategoryToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityPointCategoryToSite
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
        FROM track.tblEntityPointCategoryToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END