/* {CheckPoint: CREATING TRACKING TABLE for tblEntityMaintenanceReasonToSite } */

/****** Object:  Table [track].[tblEntityMaintenanceReasonToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityMaintenanceReasonToSite]
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
	[PK_MaintenanceReasonToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityMaintenanceReasonToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMaintenanceReasonToSite_PK_MaintenanceReasonToSiteGuid] ON [track].[tblEntityMaintenanceReasonToSite]
(
    [PK_MaintenanceReasonToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMaintenanceReasonToSite_InsertedRowVersion] ON [track].[tblEntityMaintenanceReasonToSite]
(
    [InsertedRowVersion] ASC,
    [PK_MaintenanceReasonToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMaintenanceReasonToSite_UpdatedRowVersion] ON [track].[tblEntityMaintenanceReasonToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_MaintenanceReasonToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityMaintenanceReasonToSite_DeletedRowVersion] ON [track].[tblEntityMaintenanceReasonToSite]
(
    [DeletedRowVersion] ASC,
    [PK_MaintenanceReasonToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityMaintenanceReasonToSite_PK_MaintenanceReasonToSiteGuid_Sync] ON [track].[tblEntityMaintenanceReasonToSite]
(
	[PK_MaintenanceReasonToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityMaintenanceReasonToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityMaintenanceReasonToSite
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
        FROM track.tblEntityMaintenanceReasonToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END