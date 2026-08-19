/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyPersonnelAssignedToCompany } */

/****** Object:  Table [track].[tblCompanyPersonnelAssignedToCompany]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyPersonnelAssignedToCompany]
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
	[PK_CompanyPersonnelAssignedToCompanyGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyPersonnelAssignedToCompany_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelAssignedToCompany_PK_CompanyPersonnelAssignedToCompanyGuid] ON [track].[tblCompanyPersonnelAssignedToCompany]
(
    [PK_CompanyPersonnelAssignedToCompanyGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelAssignedToCompany_InsertedRowVersion] ON [track].[tblCompanyPersonnelAssignedToCompany]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyPersonnelAssignedToCompanyGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelAssignedToCompany_UpdatedRowVersion] ON [track].[tblCompanyPersonnelAssignedToCompany]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyPersonnelAssignedToCompanyGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelAssignedToCompany_DeletedRowVersion] ON [track].[tblCompanyPersonnelAssignedToCompany]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyPersonnelAssignedToCompanyGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelAssignedToCompany_PK_CompanyPersonnelAssignedToCompanyGuid_Sync] ON [track].[tblCompanyPersonnelAssignedToCompany]
(
	[PK_CompanyPersonnelAssignedToCompanyGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyPersonnelAssignedToCompany_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyPersonnelAssignedToCompany
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
        FROM track.tblCompanyPersonnelAssignedToCompany t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END