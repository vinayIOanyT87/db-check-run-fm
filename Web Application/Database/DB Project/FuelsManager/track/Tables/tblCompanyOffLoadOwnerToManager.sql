/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyOffLoadOwnerToManager } */

/****** Object:  Table [track].[tblCompanyOffLoadOwnerToManager]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyOffLoadOwnerToManager]
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
	[PK_CompanyOffLoadOwnerToManagerGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyOffLoadOwnerToManager_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyOffLoadOwnerToManager_PK_CompanyOffLoadOwnerToManagerGuid] ON [track].[tblCompanyOffLoadOwnerToManager]
(
    [PK_CompanyOffLoadOwnerToManagerGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyOffLoadOwnerToManager_InsertedRowVersion] ON [track].[tblCompanyOffLoadOwnerToManager]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyOffLoadOwnerToManagerGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyOffLoadOwnerToManager_UpdatedRowVersion] ON [track].[tblCompanyOffLoadOwnerToManager]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyOffLoadOwnerToManagerGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyOffLoadOwnerToManager_DeletedRowVersion] ON [track].[tblCompanyOffLoadOwnerToManager]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyOffLoadOwnerToManagerGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyOffLoadOwnerToManager_PK_CompanyOffLoadOwnerToManagerGuid_Sync] ON [track].[tblCompanyOffLoadOwnerToManager]
(
	[PK_CompanyOffLoadOwnerToManagerGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyOffLoadOwnerToManager_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyOffLoadOwnerToManager
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
        FROM track.tblCompanyOffLoadOwnerToManager t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END