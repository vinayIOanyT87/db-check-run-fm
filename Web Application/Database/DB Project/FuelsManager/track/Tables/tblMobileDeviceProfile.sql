/* {CheckPoint: CREATING TRACKING TABLE for tblMobileDeviceProfile } */

/****** Object:  Table [track].[tblMobileDeviceProfile]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMobileDeviceProfile]
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
	[PK_MobileDeviceProfileGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMobileDeviceProfile_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfile_PK_MobileDeviceProfileGuid] ON [track].[tblMobileDeviceProfile]
(
    [PK_MobileDeviceProfileGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfile_InsertedRowVersion] ON [track].[tblMobileDeviceProfile]
(
    [InsertedRowVersion] ASC,
    [PK_MobileDeviceProfileGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfile_UpdatedRowVersion] ON [track].[tblMobileDeviceProfile]
(
    [UpdatedRowVersion] ASC,
    [PK_MobileDeviceProfileGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfile_DeletedRowVersion] ON [track].[tblMobileDeviceProfile]
(
    [DeletedRowVersion] ASC,
    [PK_MobileDeviceProfileGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfile_PK_MobileDeviceProfileGuid_Sync] ON [track].[tblMobileDeviceProfile]
(
	[PK_MobileDeviceProfileGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMobileDeviceProfile_DeletedRowVersionUpdate_ForSync
   ON track.tblMobileDeviceProfile
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
        FROM track.tblMobileDeviceProfile t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END