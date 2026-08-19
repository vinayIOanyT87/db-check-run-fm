/* {CheckPoint: CREATING TRACKING TABLE for tblPointGroupColumns } */

/****** Object:  Table [track].[tblPointGroupColumns]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointGroupColumns]
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
	[PK_PointGroupColumnsGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointGroupColumns_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupColumns_PK_PointGroupColumnsGuid] ON [track].[tblPointGroupColumns]
(
    [PK_PointGroupColumnsGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupColumns_InsertedRowVersion] ON [track].[tblPointGroupColumns]
(
    [InsertedRowVersion] ASC,
    [PK_PointGroupColumnsGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupColumns_UpdatedRowVersion] ON [track].[tblPointGroupColumns]
(
    [UpdatedRowVersion] ASC,
    [PK_PointGroupColumnsGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupColumns_DeletedRowVersion] ON [track].[tblPointGroupColumns]
(
    [DeletedRowVersion] ASC,
    [PK_PointGroupColumnsGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointGroupColumns_PK_PointGroupColumnsGuid_Sync] ON [track].[tblPointGroupColumns]
(
	[PK_PointGroupColumnsGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointGroupColumns_DeletedRowVersionUpdate_ForSync
   ON track.tblPointGroupColumns
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
        FROM track.tblPointGroupColumns t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END