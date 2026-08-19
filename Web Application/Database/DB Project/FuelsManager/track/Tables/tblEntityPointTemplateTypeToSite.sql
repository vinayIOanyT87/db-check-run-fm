/* {CheckPoint: CREATING TRACKING TABLE for tblEntityPointTemplateTypeToSite } */

/****** Object:  Table [track].[tblEntityPointTemplateTypeToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityPointTemplateTypeToSite]
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
	[PK_PointTemplateTypeToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityPointTemplateTypeToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointTemplateTypeToSite_PK_PointTemplateTypeToSiteGuid] ON [track].[tblEntityPointTemplateTypeToSite]
(
    [PK_PointTemplateTypeToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointTemplateTypeToSite_InsertedRowVersion] ON [track].[tblEntityPointTemplateTypeToSite]
(
    [InsertedRowVersion] ASC,
    [PK_PointTemplateTypeToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointTemplateTypeToSite_UpdatedRowVersion] ON [track].[tblEntityPointTemplateTypeToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_PointTemplateTypeToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointTemplateTypeToSite_DeletedRowVersion] ON [track].[tblEntityPointTemplateTypeToSite]
(
    [DeletedRowVersion] ASC,
    [PK_PointTemplateTypeToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityPointTemplateTypeToSite_PK_PointTemplateTypeToSiteGuid_Sync] ON [track].[tblEntityPointTemplateTypeToSite]
(
	[PK_PointTemplateTypeToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityPointTemplateTypeToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityPointTemplateTypeToSite
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
        FROM track.tblEntityPointTemplateTypeToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END