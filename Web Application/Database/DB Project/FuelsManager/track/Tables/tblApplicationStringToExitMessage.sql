/* {CheckPoint: CREATING TRACKING TABLE for tblApplicationStringToExitMessage } */

/****** Object:  Table [track].[tblApplicationStringToExitMessage]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblApplicationStringToExitMessage]
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
	[PK_ApplicationStringToExitMessageGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblApplicationStringToExitMessage_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToExitMessage_PK_ApplicationStringToExitMessageGuid] ON [track].[tblApplicationStringToExitMessage]
(
    [PK_ApplicationStringToExitMessageGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToExitMessage_InsertedRowVersion] ON [track].[tblApplicationStringToExitMessage]
(
    [InsertedRowVersion] ASC,
    [PK_ApplicationStringToExitMessageGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToExitMessage_UpdatedRowVersion] ON [track].[tblApplicationStringToExitMessage]
(
    [UpdatedRowVersion] ASC,
    [PK_ApplicationStringToExitMessageGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToExitMessage_DeletedRowVersion] ON [track].[tblApplicationStringToExitMessage]
(
    [DeletedRowVersion] ASC,
    [PK_ApplicationStringToExitMessageGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblApplicationStringToExitMessage_PK_ApplicationStringToExitMessageGuid_Sync] ON [track].[tblApplicationStringToExitMessage]
(
	[PK_ApplicationStringToExitMessageGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblApplicationStringToExitMessage_DeletedRowVersionUpdate_ForSync
   ON track.tblApplicationStringToExitMessage
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
        FROM track.tblApplicationStringToExitMessage t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END