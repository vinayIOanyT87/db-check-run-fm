/* {CheckPoint: CREATING TRACKING TABLE for tblStationInterfaceType } */

/****** Object:  Table [track].[tblStationInterfaceType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblStationInterfaceType]
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
	[PK_StationInterfaceTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblStationInterfaceType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStationInterfaceType_PK_StationInterfaceTypeIndex] ON [track].[tblStationInterfaceType]
(
    [PK_StationInterfaceTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStationInterfaceType_InsertedRowVersion] ON [track].[tblStationInterfaceType]
(
    [InsertedRowVersion] ASC,
    [PK_StationInterfaceTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStationInterfaceType_UpdatedRowVersion] ON [track].[tblStationInterfaceType]
(
    [UpdatedRowVersion] ASC,
    [PK_StationInterfaceTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStationInterfaceType_DeletedRowVersion] ON [track].[tblStationInterfaceType]
(
    [DeletedRowVersion] ASC,
    [PK_StationInterfaceTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblStationInterfaceType_PK_StationInterfaceTypeIndex_Sync] ON [track].[tblStationInterfaceType]
(
	[PK_StationInterfaceTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblStationInterfaceType_DeletedRowVersionUpdate_ForSync
   ON track.tblStationInterfaceType
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
        FROM track.tblStationInterfaceType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END