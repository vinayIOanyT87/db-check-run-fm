/* {CheckPoint: CREATING TRACKING TABLE for tblListViewStandardType } */

/****** Object:  Table [track].[tblListViewStandardType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblListViewStandardType]
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
	[PK_ListViewStandardTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblListViewStandardType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewStandardType_PK_ListViewStandardTypeIndex] ON [track].[tblListViewStandardType]
(
    [PK_ListViewStandardTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewStandardType_InsertedRowVersion] ON [track].[tblListViewStandardType]
(
    [InsertedRowVersion] ASC,
    [PK_ListViewStandardTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewStandardType_UpdatedRowVersion] ON [track].[tblListViewStandardType]
(
    [UpdatedRowVersion] ASC,
    [PK_ListViewStandardTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewStandardType_DeletedRowVersion] ON [track].[tblListViewStandardType]
(
    [DeletedRowVersion] ASC,
    [PK_ListViewStandardTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblListViewStandardType_PK_ListViewStandardTypeIndex_Sync] ON [track].[tblListViewStandardType]
(
	[PK_ListViewStandardTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblListViewStandardType_DeletedRowVersionUpdate_ForSync
   ON track.tblListViewStandardType
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
        FROM track.tblListViewStandardType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END