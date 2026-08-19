/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyCompanyToCompanyGroup } */

/****** Object:  Table [track].[tblCompanyCompanyToCompanyGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyCompanyToCompanyGroup]
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
	[PK_CompanyCompanyToCompanyGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyCompanyToCompanyGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToCompanyGroup_PK_CompanyCompanyToCompanyGroupGuid] ON [track].[tblCompanyCompanyToCompanyGroup]
(
    [PK_CompanyCompanyToCompanyGroupGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToCompanyGroup_InsertedRowVersion] ON [track].[tblCompanyCompanyToCompanyGroup]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyCompanyToCompanyGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToCompanyGroup_UpdatedRowVersion] ON [track].[tblCompanyCompanyToCompanyGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyCompanyToCompanyGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToCompanyGroup_DeletedRowVersion] ON [track].[tblCompanyCompanyToCompanyGroup]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyCompanyToCompanyGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToCompanyGroup_PK_CompanyCompanyToCompanyGroupGuid_Sync] ON [track].[tblCompanyCompanyToCompanyGroup]
(
	[PK_CompanyCompanyToCompanyGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyCompanyToCompanyGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyCompanyToCompanyGroup
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
        FROM track.tblCompanyCompanyToCompanyGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END