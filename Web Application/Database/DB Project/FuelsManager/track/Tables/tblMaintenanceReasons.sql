/* {CheckPoint: CREATING TRACKING TABLE for tblMaintenanceReasons } */

/****** Object:  Table [track].[tblMaintenanceReasons]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMaintenanceReasons]
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
	[PK_MaintenanceReasonGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMaintenanceReasons_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMaintenanceReasons_PK_MaintenanceReasonGuid] ON [track].[tblMaintenanceReasons]
(
    [PK_MaintenanceReasonGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMaintenanceReasons_InsertedRowVersion] ON [track].[tblMaintenanceReasons]
(
    [InsertedRowVersion] ASC,
    [PK_MaintenanceReasonGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMaintenanceReasons_UpdatedRowVersion] ON [track].[tblMaintenanceReasons]
(
    [UpdatedRowVersion] ASC,
    [PK_MaintenanceReasonGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMaintenanceReasons_DeletedRowVersion] ON [track].[tblMaintenanceReasons]
(
    [DeletedRowVersion] ASC,
    [PK_MaintenanceReasonGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMaintenanceReasons_PK_MaintenanceReasonGuid_Sync] ON [track].[tblMaintenanceReasons]
(
	[PK_MaintenanceReasonGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMaintenanceReasons_DeletedRowVersionUpdate_ForSync
   ON track.tblMaintenanceReasons
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
        FROM track.tblMaintenanceReasons t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END