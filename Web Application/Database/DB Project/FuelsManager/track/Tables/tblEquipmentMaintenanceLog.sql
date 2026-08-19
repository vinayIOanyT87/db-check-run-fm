/* {CheckPoint: CREATING TRACKING TABLE for tblEquipmentMaintenanceLog } */

/****** Object:  Table [track].[tblEquipmentMaintenanceLog]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEquipmentMaintenanceLog]
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
	[PK_EquipmentMaintenanceLogGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEquipmentMaintenanceLog_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentMaintenanceLog_PK_EquipmentMaintenanceLogGuid] ON [track].[tblEquipmentMaintenanceLog]
(
    [PK_EquipmentMaintenanceLogGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentMaintenanceLog_InsertedRowVersion] ON [track].[tblEquipmentMaintenanceLog]
(
    [InsertedRowVersion] ASC,
    [PK_EquipmentMaintenanceLogGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentMaintenanceLog_UpdatedRowVersion] ON [track].[tblEquipmentMaintenanceLog]
(
    [UpdatedRowVersion] ASC,
    [PK_EquipmentMaintenanceLogGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentMaintenanceLog_DeletedRowVersion] ON [track].[tblEquipmentMaintenanceLog]
(
    [DeletedRowVersion] ASC,
    [PK_EquipmentMaintenanceLogGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentMaintenanceLog_PK_EquipmentMaintenanceLogGuid_Sync] ON [track].[tblEquipmentMaintenanceLog]
(
	[PK_EquipmentMaintenanceLogGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEquipmentMaintenanceLog_DeletedRowVersionUpdate_ForSync
   ON track.tblEquipmentMaintenanceLog
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
        FROM track.tblEquipmentMaintenanceLog t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END