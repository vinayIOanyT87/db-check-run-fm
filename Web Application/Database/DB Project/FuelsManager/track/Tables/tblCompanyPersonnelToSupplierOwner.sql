/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyPersonnelToSupplierOwner } */

/****** Object:  Table [track].[tblCompanyPersonnelToSupplierOwner]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyPersonnelToSupplierOwner]
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
	[PK_CompanyPersonnelToSupplierOwnerGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyPersonnelToSupplierOwner_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToSupplierOwner_PK_CompanyPersonnelToSupplierOwnerGuid] ON [track].[tblCompanyPersonnelToSupplierOwner]
(
    [PK_CompanyPersonnelToSupplierOwnerGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToSupplierOwner_InsertedRowVersion] ON [track].[tblCompanyPersonnelToSupplierOwner]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyPersonnelToSupplierOwnerGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToSupplierOwner_UpdatedRowVersion] ON [track].[tblCompanyPersonnelToSupplierOwner]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyPersonnelToSupplierOwnerGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToSupplierOwner_DeletedRowVersion] ON [track].[tblCompanyPersonnelToSupplierOwner]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyPersonnelToSupplierOwnerGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToSupplierOwner_PK_CompanyPersonnelToSupplierOwnerGuid_Sync] ON [track].[tblCompanyPersonnelToSupplierOwner]
(
	[PK_CompanyPersonnelToSupplierOwnerGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyPersonnelToSupplierOwner_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyPersonnelToSupplierOwner
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
        FROM track.tblCompanyPersonnelToSupplierOwner t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END