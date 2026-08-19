/* {CheckPoint: CREATING TRACKING TABLE for tblEntityPersonnelTrainingToSite } */

/****** Object:  Table [track].[tblEntityPersonnelTrainingToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityPersonnelTrainingToSite]
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
	[PK_PersonnelTrainingToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityPersonnelTrainingToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelTrainingToSite_PK_PersonnelTrainingToSiteGuid] ON [track].[tblEntityPersonnelTrainingToSite]
(
    [PK_PersonnelTrainingToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelTrainingToSite_InsertedRowVersion] ON [track].[tblEntityPersonnelTrainingToSite]
(
    [InsertedRowVersion] ASC,
    [PK_PersonnelTrainingToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelTrainingToSite_UpdatedRowVersion] ON [track].[tblEntityPersonnelTrainingToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_PersonnelTrainingToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelTrainingToSite_DeletedRowVersion] ON [track].[tblEntityPersonnelTrainingToSite]
(
    [DeletedRowVersion] ASC,
    [PK_PersonnelTrainingToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelTrainingToSite_PK_PersonnelTrainingToSiteGuid_Sync] ON [track].[tblEntityPersonnelTrainingToSite]
(
	[PK_PersonnelTrainingToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityPersonnelTrainingToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityPersonnelTrainingToSite
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
        FROM track.tblEntityPersonnelTrainingToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END