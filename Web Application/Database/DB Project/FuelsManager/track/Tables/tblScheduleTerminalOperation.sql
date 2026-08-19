/* {CheckPoint: CREATING TRACKING TABLE for tblScheduleTerminalOperation } */

/****** Object:  Table [track].[tblScheduleTerminalOperation]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblScheduleTerminalOperation]
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
	[PK_ScheduleTerminalOperationGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblScheduleTerminalOperation_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleTerminalOperation_PK_ScheduleTerminalOperationGuid] ON [track].[tblScheduleTerminalOperation]
(
    [PK_ScheduleTerminalOperationGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleTerminalOperation_InsertedRowVersion] ON [track].[tblScheduleTerminalOperation]
(
    [InsertedRowVersion] ASC,
    [PK_ScheduleTerminalOperationGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleTerminalOperation_UpdatedRowVersion] ON [track].[tblScheduleTerminalOperation]
(
    [UpdatedRowVersion] ASC,
    [PK_ScheduleTerminalOperationGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleTerminalOperation_DeletedRowVersion] ON [track].[tblScheduleTerminalOperation]
(
    [DeletedRowVersion] ASC,
    [PK_ScheduleTerminalOperationGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblScheduleTerminalOperation_PK_ScheduleTerminalOperationGuid_Sync] ON [track].[tblScheduleTerminalOperation]
(
	[PK_ScheduleTerminalOperationGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblScheduleTerminalOperation_DeletedRowVersionUpdate_ForSync
   ON track.tblScheduleTerminalOperation
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
        FROM track.tblScheduleTerminalOperation t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END