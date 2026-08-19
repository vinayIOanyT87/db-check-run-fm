/* {CheckPoint: CREATING TRACKING TABLE for tblEntityModuleToSite } */

/****** Object:  Table [track].[tblEntityModuleToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityModuleToSite]
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
	[PK_ModuleToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityModuleToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityModuleToSite_PK_ModuleToSiteGuid] ON [track].[tblEntityModuleToSite]
(
    [PK_ModuleToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityModuleToSite_InsertedRowVersion] ON [track].[tblEntityModuleToSite]
(
    [InsertedRowVersion] ASC,
    [PK_ModuleToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityModuleToSite_UpdatedRowVersion] ON [track].[tblEntityModuleToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_ModuleToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityModuleToSite_DeletedRowVersion] ON [track].[tblEntityModuleToSite]
(
    [DeletedRowVersion] ASC,
    [PK_ModuleToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityModuleToSite_PK_ModuleToSiteGuid_Sync] ON [track].[tblEntityModuleToSite]
(
	[PK_ModuleToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityModuleToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityModuleToSite
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
        FROM track.tblEntityModuleToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END