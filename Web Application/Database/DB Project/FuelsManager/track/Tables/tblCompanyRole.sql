/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyRole } */

/****** Object:  Table [track].[tblCompanyRole]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyRole]
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
	[PK_CompanyRoleIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyRole_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyRole_PK_CompanyRoleIndex] ON [track].[tblCompanyRole]
(
    [PK_CompanyRoleIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyRole_InsertedRowVersion] ON [track].[tblCompanyRole]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyRoleIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyRole_UpdatedRowVersion] ON [track].[tblCompanyRole]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyRoleIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyRole_DeletedRowVersion] ON [track].[tblCompanyRole]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyRoleIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyRole_PK_CompanyRoleIndex_Sync] ON [track].[tblCompanyRole]
(
	[PK_CompanyRoleIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyRole_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyRole
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
        FROM track.tblCompanyRole t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END