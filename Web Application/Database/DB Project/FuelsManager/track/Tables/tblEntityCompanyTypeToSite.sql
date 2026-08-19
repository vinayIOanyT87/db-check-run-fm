/* {CheckPoint: CREATING TRACKING TABLE for tblEntityCompanyTypeToSite } */

/****** Object:  Table [track].[tblEntityCompanyTypeToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityCompanyTypeToSite]
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
	[PK_CompanyTypeToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityCompanyTypeToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityCompanyTypeToSite_PK_CompanyTypeToSiteGuid] ON [track].[tblEntityCompanyTypeToSite]
(
    [PK_CompanyTypeToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityCompanyTypeToSite_InsertedRowVersion] ON [track].[tblEntityCompanyTypeToSite]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyTypeToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityCompanyTypeToSite_UpdatedRowVersion] ON [track].[tblEntityCompanyTypeToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyTypeToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityCompanyTypeToSite_DeletedRowVersion] ON [track].[tblEntityCompanyTypeToSite]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyTypeToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityCompanyTypeToSite_PK_CompanyTypeToSiteGuid_Sync] ON [track].[tblEntityCompanyTypeToSite]
(
	[PK_CompanyTypeToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityCompanyTypeToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityCompanyTypeToSite
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
        FROM track.tblEntityCompanyTypeToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END