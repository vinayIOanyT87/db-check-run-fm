/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAppointmentPersonnelToSite } */

/****** Object:  Table [track].[tblEntityAppointmentPersonnelToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAppointmentPersonnelToSite]
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
	[PK_AppointmentPersonnelToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAppointmentPersonnelToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentPersonnelToSite_PK_AppointmentPersonnelToSiteGuid] ON [track].[tblEntityAppointmentPersonnelToSite]
(
    [PK_AppointmentPersonnelToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentPersonnelToSite_InsertedRowVersion] ON [track].[tblEntityAppointmentPersonnelToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AppointmentPersonnelToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentPersonnelToSite_UpdatedRowVersion] ON [track].[tblEntityAppointmentPersonnelToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AppointmentPersonnelToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentPersonnelToSite_DeletedRowVersion] ON [track].[tblEntityAppointmentPersonnelToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AppointmentPersonnelToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAppointmentPersonnelToSite_PK_AppointmentPersonnelToSiteGuid_Sync] ON [track].[tblEntityAppointmentPersonnelToSite]
(
	[PK_AppointmentPersonnelToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAppointmentPersonnelToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAppointmentPersonnelToSite
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
        FROM track.tblEntityAppointmentPersonnelToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END