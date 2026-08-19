/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyBillToToShipper } */

/****** Object:  Table [track].[tblCompanyBillToToShipper]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyBillToToShipper]
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
	[PK_CompanyBillToToShipperGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyBillToToShipper_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyBillToToShipper_PK_CompanyBillToToShipperGuid] ON [track].[tblCompanyBillToToShipper]
(
    [PK_CompanyBillToToShipperGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyBillToToShipper_InsertedRowVersion] ON [track].[tblCompanyBillToToShipper]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyBillToToShipperGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyBillToToShipper_UpdatedRowVersion] ON [track].[tblCompanyBillToToShipper]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyBillToToShipperGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyBillToToShipper_DeletedRowVersion] ON [track].[tblCompanyBillToToShipper]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyBillToToShipperGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyBillToToShipper_PK_CompanyBillToToShipperGuid_Sync] ON [track].[tblCompanyBillToToShipper]
(
	[PK_CompanyBillToToShipperGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyBillToToShipper_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyBillToToShipper
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
        FROM track.tblCompanyBillToToShipper t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END