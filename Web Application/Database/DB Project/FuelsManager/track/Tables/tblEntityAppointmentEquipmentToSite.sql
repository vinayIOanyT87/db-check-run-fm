/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAppointmentEquipmentToSite } */

/****** Object:  Table [track].[tblEntityAppointmentEquipmentToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAppointmentEquipmentToSite]
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
	[PK_AppointmentEquipmentToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAppointmentEquipmentToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentEquipmentToSite_PK_AppointmentEquipmentToSiteGuid] ON [track].[tblEntityAppointmentEquipmentToSite]
(
    [PK_AppointmentEquipmentToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentEquipmentToSite_InsertedRowVersion] ON [track].[tblEntityAppointmentEquipmentToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AppointmentEquipmentToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentEquipmentToSite_UpdatedRowVersion] ON [track].[tblEntityAppointmentEquipmentToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AppointmentEquipmentToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentEquipmentToSite_DeletedRowVersion] ON [track].[tblEntityAppointmentEquipmentToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AppointmentEquipmentToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentEquipmentToSite_PK_AppointmentEquipmentToSiteGuid_Sync] ON [track].[tblEntityAppointmentEquipmentToSite]
(
	[PK_AppointmentEquipmentToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAppointmentEquipmentToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAppointmentEquipmentToSite
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
        FROM track.tblEntityAppointmentEquipmentToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END