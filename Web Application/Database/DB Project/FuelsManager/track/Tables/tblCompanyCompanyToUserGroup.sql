/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyCompanyToUserGroup } */

/****** Object:  Table [track].[tblCompanyCompanyToUserGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyCompanyToUserGroup]
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
	[PK_CompanyCompanyToUserGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyCompanyToUserGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToUserGroup_PK_CompanyCompanyToUserGroupGuid] ON [track].[tblCompanyCompanyToUserGroup]
(
    [PK_CompanyCompanyToUserGroupGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToUserGroup_InsertedRowVersion] ON [track].[tblCompanyCompanyToUserGroup]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyCompanyToUserGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToUserGroup_UpdatedRowVersion] ON [track].[tblCompanyCompanyToUserGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyCompanyToUserGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToUserGroup_DeletedRowVersion] ON [track].[tblCompanyCompanyToUserGroup]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyCompanyToUserGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyCompanyToUserGroup_PK_CompanyCompanyToUserGroupGuid_Sync] ON [track].[tblCompanyCompanyToUserGroup]
(
	[PK_CompanyCompanyToUserGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyCompanyToUserGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyCompanyToUserGroup
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
        FROM track.tblCompanyCompanyToUserGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END