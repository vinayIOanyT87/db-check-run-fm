/* {CheckPoint: CREATING TRACKING TABLE for tblAppointmentTank } */

/****** Object:  Table [track].[tblAppointmentTank]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAppointmentTank]
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
	[PK_AppointmentTankGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAppointmentTank_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAppointmentTank_PK_AppointmentTankGuid] ON [track].[tblAppointmentTank]
(
    [PK_AppointmentTankGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAppointmentTank_InsertedRowVersion] ON [track].[tblAppointmentTank]
(
    [InsertedRowVersion] ASC,
    [PK_AppointmentTankGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAppointmentTank_UpdatedRowVersion] ON [track].[tblAppointmentTank]
(
    [UpdatedRowVersion] ASC,
    [PK_AppointmentTankGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAppointmentTank_DeletedRowVersion] ON [track].[tblAppointmentTank]
(
    [DeletedRowVersion] ASC,
    [PK_AppointmentTankGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAppointmentTank_PK_AppointmentTankGuid_Sync] ON [track].[tblAppointmentTank]
(
	[PK_AppointmentTankGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAppointmentTank_DeletedRowVersionUpdate_ForSync
   ON track.tblAppointmentTank
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
        FROM track.tblAppointmentTank t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END