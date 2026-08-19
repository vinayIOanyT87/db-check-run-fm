/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToDotHazardousMessage } */

/****** Object:  Table [track].[tblApplicationStringToDotHazardousMessage]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblApplicationStringToDotHazardousMessage]
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
	[PK_ApplicationStringToDotHazardousMessageGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToDotHazardousMessage_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToDotHazardousMessage_PK_ApplicationStringToDotHazardousMessageGuid] ON [track].[tblApplicationStringToDotHazardousMessage]
(
    [PK_ApplicationStringToDotHazardousMessageGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToDotHazardousMessage_InsertedRowVersion] ON [track].[tblApplicationStringToDotHazardousMessage]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToDotHazardousMessageGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToDotHazardousMessage_UpdatedRowVersion] ON [track].[tblApplicationStringToDotHazardousMessage]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToDotHazardousMessageGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToDotHazardousMessage_DeletedRowVersion] ON [track].[tblApplicationStringToDotHazardousMessage]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToDotHazardousMessageGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToDotHazardousMessage_PK_ApplicationStringToDotHazardousMessageGuid_Sync] ON [track].[tblApplicationStringToDotHazardousMessage]
(
	[PK_ApplicationStringToDotHazardousMessageGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToDotHazardousMessage_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToDotHazardousMessage
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
        FROM track.tblApplicationStringToDotHazardousMessage t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END