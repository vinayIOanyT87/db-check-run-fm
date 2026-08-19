/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationEquipmentTagAndLicenseToEquipment } */

/****** Object:  Table [track].[tblQualificationEquipmentTagAndLicenseToEquipment]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationEquipmentTagAndLicenseToEquipment]
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
	[PK_QualificationEquipmentTagAndLicenseToEquipmentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationEquipmentTagAndLicenseToEquipment_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToEquipment_PK_QualificationEquipmentTagAndLicenseToEquipmentGuid] ON [track].[tblQualificationEquipmentTagAndLicenseToEquipment]
(
    [PK_QualificationEquipmentTagAndLicenseToEquipmentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToEquipment_InsertedRowVersion] ON [track].[tblQualificationEquipmentTagAndLicenseToEquipment]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationEquipmentTagAndLicenseToEquipmentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToEquipment_UpdatedRowVersion] ON [track].[tblQualificationEquipmentTagAndLicenseToEquipment]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationEquipmentTagAndLicenseToEquipmentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToEquipment_DeletedRowVersion] ON [track].[tblQualificationEquipmentTagAndLicenseToEquipment]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationEquipmentTagAndLicenseToEquipmentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTagAndLicenseToEquipment_PK_QualificationEquipmentTagAndLicenseToEquipmentGuid_Sync] ON [track].[tblQualificationEquipmentTagAndLicenseToEquipment]
(
	[PK_QualificationEquipmentTagAndLicenseToEquipmentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationEquipmentTagAndLicenseToEquipment_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationEquipmentTagAndLicenseToEquipment
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
        FROM track.tblQualificationEquipmentTagAndLicenseToEquipment t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END