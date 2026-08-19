/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableStationOutputPermissive } */

/****** Object:  Table [track].[tblProcessVariableStationOutputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableStationOutputPermissive]
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
	[PK_ProcessVariableStationGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableStationOutputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableStationOutputPermissive_PK_ProcessVariableStationGuid] ON [track].[tblProcessVariableStationOutputPermissive]
(
    [PK_ProcessVariableStationGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableStationOutputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableStationOutputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableStationGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableStationOutputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableStationOutputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableStationGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableStationOutputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableStationOutputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableStationGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableStationOutputPermissive_PK_ProcessVariableStationGuid_Sync] ON [track].[tblProcessVariableStationOutputPermissive]
(
	[PK_ProcessVariableStationGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableStationOutputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableStationOutputPermissive
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
        FROM track.tblProcessVariableStationOutputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END