/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonLicenseToStation } */

/****** Object:  Table [track].[tblQualificationPersonLicenseToStation]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblQualificationPersonLicenseToStation]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblQualificationPersonLicenseToStation]
(
	ChangeIndex [bigint] NOT NULL IDENTITY(1,1),
	InsertedDate [datetimeoffset](7) NOT NULL,
	InsertedContext [varbinary](128) NULL,
	InsertedRowVersion [varbinary](8) NOT NULL,
	UpdatedDate [datetimeoffset](7) NULL,
	UpdatedContext [varbinary](128) NULL,
	UpdatedRowVersion [varbinary](8) NULL,
	DeletedDate [datetimeoffset](7) NULL,
	DeletedContext [varbinary](128) NULL,
	DeletedRowVersion [varbinary](8) NULL,
	CurrentSiteGuid [uniqueidentifier] NULL,
	PreviousSiteGuid [uniqueidentifier] NULL,
    PK_QualificationPersonLicenseToStationGuid [UniqueIdentifier] NOT NULL,
    FK_ParentPK uniqueidentifier NULL,
	CONSTRAINT [PK_track_tblQualificationPersonLicenseToStation_ChangeIndex] PRIMARY KEY CLUSTERED 
	(
		[ChangeIndex] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
--END


GO
/****** Object:  Index [IX_track_tblQualificationPersonLicenseToStation_InsertContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblQualificationPersonLicenseToStation]') AND name = N'IX_track_tblQualificationPersonLicenseToStation_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToStation_InsertContext] ON [track].[tblQualificationPersonLicenseToStation]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonLicenseToStationGuid] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblQualificationPersonLicenseToStation_UpdateContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblQualificationPersonLicenseToStation]') AND name = N'IX_track_tblQualificationPersonLicenseToStation_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToStation_UpdateContext] ON [track].[tblQualificationPersonLicenseToStation]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonLicenseToStationGuid] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblQualificationPersonLicenseToStation_DeleteContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblQualificationPersonLicenseToStation]') AND name = N'IX_track_tblQualificationPersonLicenseToStation_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonLicenseToStation_DeleteContext] ON [track].[tblQualificationPersonLicenseToStation]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonLicenseToStationGuid] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE NONCLUSTERED INDEX [IX_tblQualificationPersonLicenseToStation_PK_QualificationPersonLicenseToStationGuid]
    ON [track].[tblQualificationPersonLicenseToStation]([PK_QualificationPersonLicenseToStationGuid] ASC);


GO
