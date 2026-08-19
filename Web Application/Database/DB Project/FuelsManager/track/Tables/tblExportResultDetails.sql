/* {CheckPoint: CREATING TRACKING TABLE for tblExportResultDetails } */

/****** Object:  Table [track].[tblExportResultDetails]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblExportResultDetails]
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
	[PK_ExportResultDetailGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblExportResultDetails_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblExportResultDetails_PK_ExportResultDetailGuid] ON [track].[tblExportResultDetails]
(
    [PK_ExportResultDetailGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblExportResultDetails_InsertedRowVersion] ON [track].[tblExportResultDetails]
(
    [InsertedRowVersion] ASC,
    [PK_ExportResultDetailGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblExportResultDetails_UpdatedRowVersion] ON [track].[tblExportResultDetails]
(
    [UpdatedRowVersion] ASC,
    [PK_ExportResultDetailGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblExportResultDetails_DeletedRowVersion] ON [track].[tblExportResultDetails]
(
    [DeletedRowVersion] ASC,
    [PK_ExportResultDetailGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblExportResultDetails_PK_ExportResultDetailGuid_Sync] ON [track].[tblExportResultDetails]
(
	[PK_ExportResultDetailGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblExportResultDetails_DeletedRowVersionUpdate_ForSync
   ON track.tblExportResultDetails
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
        FROM track.tblExportResultDetails t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END