/* {CheckPoint: CREATING TRACKING TABLE for tblEntityMobileDeviceProfileToSite } */

/****** Object:  Table [track].[tblEntityMobileDeviceProfileToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityMobileDeviceProfileToSite]
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
	[PK_MobileDeviceProfileToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityMobileDeviceProfileToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMobileDeviceProfileToSite_PK_MobileDeviceProfileToSiteGuid] ON [track].[tblEntityMobileDeviceProfileToSite]
(
    [PK_MobileDeviceProfileToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMobileDeviceProfileToSite_InsertedRowVersion] ON [track].[tblEntityMobileDeviceProfileToSite]
(
    [InsertedRowVersion] ASC,
    [PK_MobileDeviceProfileToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMobileDeviceProfileToSite_UpdatedRowVersion] ON [track].[tblEntityMobileDeviceProfileToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_MobileDeviceProfileToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMobileDeviceProfileToSite_DeletedRowVersion] ON [track].[tblEntityMobileDeviceProfileToSite]
(
    [DeletedRowVersion] ASC,
    [PK_MobileDeviceProfileToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityMobileDeviceProfileToSite_PK_MobileDeviceProfileToSiteGuid_Sync] ON [track].[tblEntityMobileDeviceProfileToSite]
(
	[PK_MobileDeviceProfileToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityMobileDeviceProfileToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityMobileDeviceProfileToSite
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
        FROM track.tblEntityMobileDeviceProfileToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END