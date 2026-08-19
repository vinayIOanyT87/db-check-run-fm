/* {CheckPoint: CREATING TRACKING TABLE for tblTankMaintenanceLog } */

/****** Object:  Table [track].[tblTankMaintenanceLog]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTankMaintenanceLog]
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
	[PK_TankMaintenanceLogGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTankMaintenanceLog_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankMaintenanceLog_PK_TankMaintenanceLogGuid] ON [track].[tblTankMaintenanceLog]
(
    [PK_TankMaintenanceLogGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankMaintenanceLog_InsertedRowVersion] ON [track].[tblTankMaintenanceLog]
(
    [InsertedRowVersion] ASC,
    [PK_TankMaintenanceLogGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankMaintenanceLog_UpdatedRowVersion] ON [track].[tblTankMaintenanceLog]
(
    [UpdatedRowVersion] ASC,
    [PK_TankMaintenanceLogGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankMaintenanceLog_DeletedRowVersion] ON [track].[tblTankMaintenanceLog]
(
    [DeletedRowVersion] ASC,
    [PK_TankMaintenanceLogGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTankMaintenanceLog_PK_TankMaintenanceLogGuid_Sync] ON [track].[tblTankMaintenanceLog]
(
	[PK_TankMaintenanceLogGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTankMaintenanceLog_DeletedRowVersionUpdate_ForSync
   ON track.tblTankMaintenanceLog
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
        FROM track.tblTankMaintenanceLog t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END