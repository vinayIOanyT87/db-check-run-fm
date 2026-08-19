/* {CheckPoint: CREATING TRACKING TABLE for tblTanks } */

/****** Object:  Table [track].[tblTanks]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTanks]
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
	[PK_TankGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTanks_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTanks_PK_TankGuid] ON [track].[tblTanks]
(
    [PK_TankGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTanks_InsertedRowVersion] ON [track].[tblTanks]
(
    [InsertedRowVersion] ASC,
    [PK_TankGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTanks_UpdatedRowVersion] ON [track].[tblTanks]
(
    [UpdatedRowVersion] ASC,
    [PK_TankGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTanks_DeletedRowVersion] ON [track].[tblTanks]
(
    [DeletedRowVersion] ASC,
    [PK_TankGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTanks_PK_TankGuid_Sync] ON [track].[tblTanks]
(
	[PK_TankGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTanks_DeletedRowVersionUpdate_ForSync
   ON track.tblTanks
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
        FROM track.tblTanks t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END