/* {CheckPoint: CREATING TRACKING TABLE for tblReserveLevels } */

/****** Object:  Table [track].[tblReserveLevels]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblReserveLevels]
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
	[PK_ReserveLevelGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblReserveLevels_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblReserveLevels_PK_ReserveLevelGuid] ON [track].[tblReserveLevels]
(
    [PK_ReserveLevelGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblReserveLevels_InsertedRowVersion] ON [track].[tblReserveLevels]
(
    [InsertedRowVersion] ASC,
    [PK_ReserveLevelGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblReserveLevels_UpdatedRowVersion] ON [track].[tblReserveLevels]
(
    [UpdatedRowVersion] ASC,
    [PK_ReserveLevelGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblReserveLevels_DeletedRowVersion] ON [track].[tblReserveLevels]
(
    [DeletedRowVersion] ASC,
    [PK_ReserveLevelGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblReserveLevels_PK_ReserveLevelGuid_Sync] ON [track].[tblReserveLevels]
(
	[PK_ReserveLevelGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblReserveLevels_DeletedRowVersionUpdate_ForSync
   ON track.tblReserveLevels
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
        FROM track.tblReserveLevels t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END