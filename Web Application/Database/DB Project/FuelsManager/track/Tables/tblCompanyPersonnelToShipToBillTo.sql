/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyPersonnelToShipToBillTo } */

/****** Object:  Table [track].[tblCompanyPersonnelToShipToBillTo]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyPersonnelToShipToBillTo]
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
	[PK_CompanyPersonnelToShipToBillToGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyPersonnelToShipToBillTo_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToShipToBillTo_PK_CompanyPersonnelToShipToBillToGuid] ON [track].[tblCompanyPersonnelToShipToBillTo]
(
    [PK_CompanyPersonnelToShipToBillToGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToShipToBillTo_InsertedRowVersion] ON [track].[tblCompanyPersonnelToShipToBillTo]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyPersonnelToShipToBillToGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToShipToBillTo_UpdatedRowVersion] ON [track].[tblCompanyPersonnelToShipToBillTo]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyPersonnelToShipToBillToGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToShipToBillTo_DeletedRowVersion] ON [track].[tblCompanyPersonnelToShipToBillTo]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyPersonnelToShipToBillToGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyPersonnelToShipToBillTo_PK_CompanyPersonnelToShipToBillToGuid_Sync] ON [track].[tblCompanyPersonnelToShipToBillTo]
(
	[PK_CompanyPersonnelToShipToBillToGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyPersonnelToShipToBillTo_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyPersonnelToShipToBillTo
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
        FROM track.tblCompanyPersonnelToShipToBillTo t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END