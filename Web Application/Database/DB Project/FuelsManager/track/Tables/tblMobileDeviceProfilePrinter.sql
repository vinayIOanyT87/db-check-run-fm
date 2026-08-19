/* {CheckPoint: CREATING TRACKING TABLE for tblMobileDeviceProfilePrinter } */

/****** Object:  Table [track].[tblMobileDeviceProfilePrinter]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMobileDeviceProfilePrinter]
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
	[PK_MobileDeviceProfilePrinterGUID] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMobileDeviceProfilePrinter_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfilePrinter_PK_MobileDeviceProfilePrinterGUID] ON [track].[tblMobileDeviceProfilePrinter]
(
    [PK_MobileDeviceProfilePrinterGUID],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfilePrinter_InsertedRowVersion] ON [track].[tblMobileDeviceProfilePrinter]
(
    [InsertedRowVersion] ASC,
    [PK_MobileDeviceProfilePrinterGUID],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfilePrinter_UpdatedRowVersion] ON [track].[tblMobileDeviceProfilePrinter]
(
    [UpdatedRowVersion] ASC,
    [PK_MobileDeviceProfilePrinterGUID],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfilePrinter_DeletedRowVersion] ON [track].[tblMobileDeviceProfilePrinter]
(
    [DeletedRowVersion] ASC,
    [PK_MobileDeviceProfilePrinterGUID],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfilePrinter_PK_MobileDeviceProfilePrinterGUID_Sync] ON [track].[tblMobileDeviceProfilePrinter]
(
	[PK_MobileDeviceProfilePrinterGUID] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMobileDeviceProfilePrinter_DeletedRowVersionUpdate_ForSync
   ON track.tblMobileDeviceProfilePrinter
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
        FROM track.tblMobileDeviceProfilePrinter t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END