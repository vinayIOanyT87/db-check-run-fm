/* {CheckPoint: CREATING TRACKING TABLE for tblAirplaneTankLocation } */

/****** Object:  Table [track].[tblAirplaneTankLocation]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAirplaneTankLocation]
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
	[PK_TankLocationIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAirplaneTankLocation_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAirplaneTankLocation_PK_TankLocationIndex] ON [track].[tblAirplaneTankLocation]
(
    [PK_TankLocationIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAirplaneTankLocation_InsertedRowVersion] ON [track].[tblAirplaneTankLocation]
(
    [InsertedRowVersion] ASC,
    [PK_TankLocationIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAirplaneTankLocation_UpdatedRowVersion] ON [track].[tblAirplaneTankLocation]
(
    [UpdatedRowVersion] ASC,
    [PK_TankLocationIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAirplaneTankLocation_DeletedRowVersion] ON [track].[tblAirplaneTankLocation]
(
    [DeletedRowVersion] ASC,
    [PK_TankLocationIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAirplaneTankLocation_PK_TankLocationIndex_Sync] ON [track].[tblAirplaneTankLocation]
(
	[PK_TankLocationIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAirplaneTankLocation_DeletedRowVersionUpdate_ForSync
   ON track.tblAirplaneTankLocation
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
        FROM track.tblAirplaneTankLocation t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END