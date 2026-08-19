/* {CheckPoint: CREATING TRACKING TABLE for tblGlobalSpecificChangesQueue } */

/****** Object:  Table [track].[tblGlobalSpecificChangesQueue]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblGlobalSpecificChangesQueue]
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
	[PK_GSQueueGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblGlobalSpecificChangesQueue_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGlobalSpecificChangesQueue_PK_GSQueueGuid] ON [track].[tblGlobalSpecificChangesQueue]
(
    [PK_GSQueueGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGlobalSpecificChangesQueue_InsertedRowVersion] ON [track].[tblGlobalSpecificChangesQueue]
(
    [InsertedRowVersion] ASC,
    [PK_GSQueueGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGlobalSpecificChangesQueue_UpdatedRowVersion] ON [track].[tblGlobalSpecificChangesQueue]
(
    [UpdatedRowVersion] ASC,
    [PK_GSQueueGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGlobalSpecificChangesQueue_DeletedRowVersion] ON [track].[tblGlobalSpecificChangesQueue]
(
    [DeletedRowVersion] ASC,
    [PK_GSQueueGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGlobalSpecificChangesQueue_PK_GSQueueGuid_Sync] ON [track].[tblGlobalSpecificChangesQueue]
(
	[PK_GSQueueGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblGlobalSpecificChangesQueue_DeletedRowVersionUpdate_ForSync
   ON track.tblGlobalSpecificChangesQueue
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
        FROM track.tblGlobalSpecificChangesQueue t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END