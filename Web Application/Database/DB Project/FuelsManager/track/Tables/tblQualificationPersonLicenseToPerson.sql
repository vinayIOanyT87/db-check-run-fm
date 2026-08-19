/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonLicenseToPerson } */

/****** Object:  Table [track].[tblQualificationPersonLicenseToPerson]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationPersonLicenseToPerson]
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
	[PK_QualificationPersonLicenseToPersonGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationPersonLicenseToPerson_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToPerson_PK_QualificationPersonLicenseToPersonGuid] ON [track].[tblQualificationPersonLicenseToPerson]
(
    [PK_QualificationPersonLicenseToPersonGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToPerson_InsertedRowVersion] ON [track].[tblQualificationPersonLicenseToPerson]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonLicenseToPersonGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToPerson_UpdatedRowVersion] ON [track].[tblQualificationPersonLicenseToPerson]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonLicenseToPersonGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToPerson_DeletedRowVersion] ON [track].[tblQualificationPersonLicenseToPerson]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonLicenseToPersonGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToPerson_PK_QualificationPersonLicenseToPersonGuid_Sync] ON [track].[tblQualificationPersonLicenseToPerson]
(
	[PK_QualificationPersonLicenseToPersonGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationPersonLicenseToPerson_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationPersonLicenseToPerson
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
        FROM track.tblQualificationPersonLicenseToPerson t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END