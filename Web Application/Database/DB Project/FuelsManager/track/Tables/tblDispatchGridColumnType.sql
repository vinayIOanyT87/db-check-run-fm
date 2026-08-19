/* {CheckPoint: CREATING TRACKING TABLE for tblDispatchGridColumnType } */

/****** Object:  Table [track].[tblDispatchGridColumnType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblDispatchGridColumnType]
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
	[PK_DispatchGridColumnTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblDispatchGridColumnType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDispatchGridColumnType_PK_DispatchGridColumnTypeIndex] ON [track].[tblDispatchGridColumnType]
(
    [PK_DispatchGridColumnTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDispatchGridColumnType_InsertedRowVersion] ON [track].[tblDispatchGridColumnType]
(
    [InsertedRowVersion] ASC,
    [PK_DispatchGridColumnTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDispatchGridColumnType_UpdatedRowVersion] ON [track].[tblDispatchGridColumnType]
(
    [UpdatedRowVersion] ASC,
    [PK_DispatchGridColumnTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDispatchGridColumnType_DeletedRowVersion] ON [track].[tblDispatchGridColumnType]
(
    [DeletedRowVersion] ASC,
    [PK_DispatchGridColumnTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblDispatchGridColumnType_PK_DispatchGridColumnTypeIndex_Sync] ON [track].[tblDispatchGridColumnType]
(
	[PK_DispatchGridColumnTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblDispatchGridColumnType_DeletedRowVersionUpdate_ForSync
   ON track.tblDispatchGridColumnType
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
        FROM track.tblDispatchGridColumnType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END