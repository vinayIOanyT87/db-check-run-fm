/* {CheckPoint: CREATING TRACKING TABLE for tblEntityPersonnelQualificationToSite } */

/****** Object:  Table [track].[tblEntityPersonnelQualificationToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityPersonnelQualificationToSite]
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
	[PK_PersonnelQualificationToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityPersonnelQualificationToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelQualificationToSite_PK_PersonnelQualificationToSiteGuid] ON [track].[tblEntityPersonnelQualificationToSite]
(
    [PK_PersonnelQualificationToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelQualificationToSite_InsertedRowVersion] ON [track].[tblEntityPersonnelQualificationToSite]
(
    [InsertedRowVersion] ASC,
    [PK_PersonnelQualificationToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelQualificationToSite_UpdatedRowVersion] ON [track].[tblEntityPersonnelQualificationToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_PersonnelQualificationToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelQualificationToSite_DeletedRowVersion] ON [track].[tblEntityPersonnelQualificationToSite]
(
    [DeletedRowVersion] ASC,
    [PK_PersonnelQualificationToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityPersonnelQualificationToSite_PK_PersonnelQualificationToSiteGuid_Sync] ON [track].[tblEntityPersonnelQualificationToSite]
(
	[PK_PersonnelQualificationToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityPersonnelQualificationToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityPersonnelQualificationToSite
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
        FROM track.tblEntityPersonnelQualificationToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END