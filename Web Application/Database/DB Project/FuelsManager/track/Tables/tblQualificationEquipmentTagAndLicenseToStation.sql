/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationEquipmentTagAndLicenseToStation } */

/****** Object:  Table [track].[tblQualificationEquipmentTagAndLicenseToStation]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationEquipmentTagAndLicenseToStation]
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
	[PK_QualificationEquipmentTagAndLicenseToStationGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationEquipmentTagAndLicenseToStation_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToStation_PK_QualificationEquipmentTagAndLicenseToStationGuid] ON [track].[tblQualificationEquipmentTagAndLicenseToStation]
(
    [PK_QualificationEquipmentTagAndLicenseToStationGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToStation_InsertedRowVersion] ON [track].[tblQualificationEquipmentTagAndLicenseToStation]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationEquipmentTagAndLicenseToStationGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToStation_UpdatedRowVersion] ON [track].[tblQualificationEquipmentTagAndLicenseToStation]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationEquipmentTagAndLicenseToStationGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToStation_DeletedRowVersion] ON [track].[tblQualificationEquipmentTagAndLicenseToStation]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationEquipmentTagAndLicenseToStationGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToStation_PK_QualificationEquipmentTagAndLicenseToStationGuid_Sync] ON [track].[tblQualificationEquipmentTagAndLicenseToStation]
(
	[PK_QualificationEquipmentTagAndLicenseToStationGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationEquipmentTagAndLicenseToStation_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationEquipmentTagAndLicenseToStation
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
        FROM track.tblQualificationEquipmentTagAndLicenseToStation t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END