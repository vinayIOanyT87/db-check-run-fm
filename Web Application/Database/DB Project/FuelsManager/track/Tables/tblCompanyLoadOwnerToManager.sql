/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyLoadOwnerToManager } */

/****** Object:  Table [track].[tblCompanyLoadOwnerToManager]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyLoadOwnerToManager]
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
	[PK_CompanyLoadOwnerToManagerGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyLoadOwnerToManager_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyLoadOwnerToManager_PK_CompanyLoadOwnerToManagerGuid] ON [track].[tblCompanyLoadOwnerToManager]
(
    [PK_CompanyLoadOwnerToManagerGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyLoadOwnerToManager_InsertedRowVersion] ON [track].[tblCompanyLoadOwnerToManager]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyLoadOwnerToManagerGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyLoadOwnerToManager_UpdatedRowVersion] ON [track].[tblCompanyLoadOwnerToManager]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyLoadOwnerToManagerGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyLoadOwnerToManager_DeletedRowVersion] ON [track].[tblCompanyLoadOwnerToManager]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyLoadOwnerToManagerGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyLoadOwnerToManager_PK_CompanyLoadOwnerToManagerGuid_Sync] ON [track].[tblCompanyLoadOwnerToManager]
(
	[PK_CompanyLoadOwnerToManagerGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyLoadOwnerToManager_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyLoadOwnerToManager
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
        FROM track.tblCompanyLoadOwnerToManager t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END