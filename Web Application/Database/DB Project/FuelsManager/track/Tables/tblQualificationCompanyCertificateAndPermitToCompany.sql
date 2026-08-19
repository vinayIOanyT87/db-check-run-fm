/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationCompanyCertificateAndPermitToCompany } */

/****** Object:  Table [track].[tblQualificationCompanyCertificateAndPermitToCompany]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationCompanyCertificateAndPermitToCompany]
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
	[PK_QualificationCompanyCertificateAndPermitToCompanyGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationCompanyCertificateAndPermitToCompany_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationCompanyCertificateAndPermitToCompany_PK_QualificationCompanyCertificateAndPermitToCompanyGuid] ON [track].[tblQualificationCompanyCertificateAndPermitToCompany]
(
    [PK_QualificationCompanyCertificateAndPermitToCompanyGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationCompanyCertificateAndPermitToCompany_InsertedRowVersion] ON [track].[tblQualificationCompanyCertificateAndPermitToCompany]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationCompanyCertificateAndPermitToCompanyGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationCompanyCertificateAndPermitToCompany_UpdatedRowVersion] ON [track].[tblQualificationCompanyCertificateAndPermitToCompany]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationCompanyCertificateAndPermitToCompanyGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationCompanyCertificateAndPermitToCompany_DeletedRowVersion] ON [track].[tblQualificationCompanyCertificateAndPermitToCompany]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationCompanyCertificateAndPermitToCompanyGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationCompanyCertificateAndPermitToCompany_PK_QualificationCompanyCertificateAndPermitToCompanyGuid_Sync] ON [track].[tblQualificationCompanyCertificateAndPermitToCompany]
(
	[PK_QualificationCompanyCertificateAndPermitToCompanyGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationCompanyCertificateAndPermitToCompany_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationCompanyCertificateAndPermitToCompany
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
        FROM track.tblQualificationCompanyCertificateAndPermitToCompany t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END