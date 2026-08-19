/* {CheckPoint: CREATING TRACKING TABLE for tblMobileDeviceProfileAnalogInput } */

/****** Object:  Table [track].[tblMobileDeviceProfileAnalogInput]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMobileDeviceProfileAnalogInput]
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
	[PK_MobileDeviceProfileAnalogInputGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMobileDeviceProfileAnalogInput_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfileAnalogInput_PK_MobileDeviceProfileAnalogInputGuid] ON [track].[tblMobileDeviceProfileAnalogInput]
(
    [PK_MobileDeviceProfileAnalogInputGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfileAnalogInput_InsertedRowVersion] ON [track].[tblMobileDeviceProfileAnalogInput]
(
    [InsertedRowVersion] ASC,
    [PK_MobileDeviceProfileAnalogInputGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfileAnalogInput_UpdatedRowVersion] ON [track].[tblMobileDeviceProfileAnalogInput]
(
    [UpdatedRowVersion] ASC,
    [PK_MobileDeviceProfileAnalogInputGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfileAnalogInput_DeletedRowVersion] ON [track].[tblMobileDeviceProfileAnalogInput]
(
    [DeletedRowVersion] ASC,
    [PK_MobileDeviceProfileAnalogInputGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMobileDeviceProfileAnalogInput_PK_MobileDeviceProfileAnalogInputGuid_Sync] ON [track].[tblMobileDeviceProfileAnalogInput]
(
	[PK_MobileDeviceProfileAnalogInputGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMobileDeviceProfileAnalogInput_DeletedRowVersionUpdate_ForSync
   ON track.tblMobileDeviceProfileAnalogInput
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
        FROM track.tblMobileDeviceProfileAnalogInput t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END