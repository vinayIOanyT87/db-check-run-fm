/* {CheckPoint: CREATING TRACKING TABLE for tblListViewFieldType } */

/****** Object:  Table [track].[tblListViewFieldType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblListViewFieldType]
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
	[PK_ListViewFieldTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblListViewFieldType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewFieldType_PK_ListViewFieldTypeIndex] ON [track].[tblListViewFieldType]
(
    [PK_ListViewFieldTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewFieldType_InsertedRowVersion] ON [track].[tblListViewFieldType]
(
    [InsertedRowVersion] ASC,
    [PK_ListViewFieldTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewFieldType_UpdatedRowVersion] ON [track].[tblListViewFieldType]
(
    [UpdatedRowVersion] ASC,
    [PK_ListViewFieldTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblListViewFieldType_DeletedRowVersion] ON [track].[tblListViewFieldType]
(
    [DeletedRowVersion] ASC,
    [PK_ListViewFieldTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblListViewFieldType_PK_ListViewFieldTypeIndex_Sync] ON [track].[tblListViewFieldType]
(
	[PK_ListViewFieldTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblListViewFieldType_DeletedRowVersionUpdate_ForSync
   ON track.tblListViewFieldType
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
        FROM track.tblListViewFieldType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END