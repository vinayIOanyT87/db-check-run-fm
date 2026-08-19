/* {CheckPoint: CREATING TRACKING TABLE for tblMobileDevice } */

/****** Object:  Table [track].[tblMobileDevice]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMobileDevice]
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
    [PK_MobileDeviceGuid] [UniqueIdentifier] NOT NULL,
    [FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
    CONSTRAINT [PK_track_tblMobileDevice_ChangeIndex] PRIMARY KEY CLUSTERED 
    (
        [ChangeIndex] ASC
    )
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDevice_PK_MobileDeviceGuid] ON [track].[tblMobileDevice]
(
    [PK_MobileDeviceGuid],
	[ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDevice_InsertedRowVersion] ON [track].[tblMobileDevice]
(
    [InsertedRowVersion] ASC,
    [PK_MobileDeviceGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDevice_UpdatedRowVersion] ON [track].[tblMobileDevice]
(
    [UpdatedRowVersion] ASC,
    [PK_MobileDeviceGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDevice_DeletedRowVersion] ON [track].[tblMobileDevice]
(
    [DeletedRowVersion] ASC,
    [PK_MobileDeviceGuid],
    [DeletedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDevice_PK_MobileDeviceGuid_Sync] ON [track].[tblMobileDevice]
(
    [PK_MobileDeviceGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO

CREATE TRIGGER track.trg_insupd_tblMobileDevice_DeletedRowVersionUpdate_ForSync
   ON track.tblMobileDevice
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
			FROM track.tblMobileDevice t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END