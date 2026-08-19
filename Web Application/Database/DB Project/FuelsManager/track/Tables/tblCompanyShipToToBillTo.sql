/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyShipToToBillTo } */

/****** Object:  Table [track].[tblCompanyShipToToBillTo]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyShipToToBillTo]
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
	[PK_CompanyShipToToBillToGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyShipToToBillTo_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyShipToToBillTo_PK_CompanyShipToToBillToGuid] ON [track].[tblCompanyShipToToBillTo]
(
    [PK_CompanyShipToToBillToGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyShipToToBillTo_InsertedRowVersion] ON [track].[tblCompanyShipToToBillTo]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyShipToToBillToGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyShipToToBillTo_UpdatedRowVersion] ON [track].[tblCompanyShipToToBillTo]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyShipToToBillToGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyShipToToBillTo_DeletedRowVersion] ON [track].[tblCompanyShipToToBillTo]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyShipToToBillToGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyShipToToBillTo_PK_CompanyShipToToBillToGuid_Sync] ON [track].[tblCompanyShipToToBillTo]
(
	[PK_CompanyShipToToBillToGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyShipToToBillTo_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyShipToToBillTo
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
        FROM track.tblCompanyShipToToBillTo t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END