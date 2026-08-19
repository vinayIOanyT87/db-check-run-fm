/* {CheckPoint: CREATING TRACKING TABLE for tblPointGroupRows } */

/****** Object:  Table [track].[tblPointGroupRows]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointGroupRows]
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
	[PK_PointGroupRowsGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointGroupRows_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupRows_PK_PointGroupRowsGuid] ON [track].[tblPointGroupRows]
(
    [PK_PointGroupRowsGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupRows_InsertedRowVersion] ON [track].[tblPointGroupRows]
(
    [InsertedRowVersion] ASC,
    [PK_PointGroupRowsGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupRows_UpdatedRowVersion] ON [track].[tblPointGroupRows]
(
    [UpdatedRowVersion] ASC,
    [PK_PointGroupRowsGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupRows_DeletedRowVersion] ON [track].[tblPointGroupRows]
(
    [DeletedRowVersion] ASC,
    [PK_PointGroupRowsGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupRows_PK_PointGroupRowsGuid_Sync] ON [track].[tblPointGroupRows]
(
	[PK_PointGroupRowsGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointGroupRows_DeletedRowVersionUpdate_ForSync
   ON track.tblPointGroupRows
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
        FROM track.tblPointGroupRows t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END