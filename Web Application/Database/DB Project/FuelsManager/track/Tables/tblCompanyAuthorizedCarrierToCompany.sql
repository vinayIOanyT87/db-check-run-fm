/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyAuthorizedCarrierToCompany } */

/****** Object:  Table [track].[tblCompanyAuthorizedCarrierToCompany]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyAuthorizedCarrierToCompany]
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
	[PK_CompanyAuthorizedCarrierToCompanyGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyAuthorizedCarrierToCompany_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyAuthorizedCarrierToCompany_PK_CompanyAuthorizedCarrierToCompanyGuid] ON [track].[tblCompanyAuthorizedCarrierToCompany]
(
    [PK_CompanyAuthorizedCarrierToCompanyGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyAuthorizedCarrierToCompany_InsertedRowVersion] ON [track].[tblCompanyAuthorizedCarrierToCompany]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyAuthorizedCarrierToCompanyGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyAuthorizedCarrierToCompany_UpdatedRowVersion] ON [track].[tblCompanyAuthorizedCarrierToCompany]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyAuthorizedCarrierToCompanyGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyAuthorizedCarrierToCompany_DeletedRowVersion] ON [track].[tblCompanyAuthorizedCarrierToCompany]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyAuthorizedCarrierToCompanyGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyAuthorizedCarrierToCompany_PK_CompanyAuthorizedCarrierToCompanyGuid_Sync] ON [track].[tblCompanyAuthorizedCarrierToCompany]
(
	[PK_CompanyAuthorizedCarrierToCompanyGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyAuthorizedCarrierToCompany_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyAuthorizedCarrierToCompany
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
        FROM track.tblCompanyAuthorizedCarrierToCompany t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END