/****** Object:  Table [track].[tblMovementHistory]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMovementHistory]
( 
	[ChangeIndex]				BIGINT IDENTITY(1,1) NOT NULL
	, [InsertedDate]			DATETIMEOFFSET(7) NOT NULL
	, [InsertedContext]			VARBINARY(128) NULL
	, [InsertedRowVersion]		VARBINARY(8) NOT NULL
	, [UpdatedDate]				DATETIMEOFFSET(7) NULL
	, [UpdatedContext]			VARBINARY(128) NULL
	, [UpdatedRowVersion]		VARBINARY(8) NULL
	, [DeletedDate]				DATETIMEOFFSET(7) NULL
	, [DeletedContext]			VARBINARY(128) NULL
	, [DeletedRowVersion]		VARBINARY(8) NULL
	, [CurrentSiteGuid]			UNIQUEIDENTIFIER NULL
	, [PreviousSiteGuid]		UNIQUEIDENTIFIER NULL
	, [PK_MovementHistoryGuid]	UNIQUEIDENTIFIER NOT NULL
	, [FK_ParentPK]				UNIQUEIDENTIFIER NULL
	, [_RowVersion]				ROWVERSION NOT NULL
	CONSTRAINT [PK_track_tblMovementHistory_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementHistory_PK_MovementHistoryGuid] ON [track].[tblMovementHistory]
(
    [PK_MovementHistoryGuid]
    , [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementHistory_InsertedRowVersion] ON [track].[tblMovementHistory]
(
    [InsertedRowVersion] ASC
    , [PK_MovementHistoryGuid]
    , [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementHistory_UpdatedRowVersion] ON [track].[tblMovementHistory]
(
    [UpdatedRowVersion] ASC
    , [PK_MovementHistoryGuid]
    , [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementHistory_DeletedRowVersion] ON [track].[tblMovementHistory]
(
    [DeletedRowVersion] ASC
    , [PK_MovementHistoryGuid]
    , [DeletedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementHistory_PK_MovementHistoryGuid_Sync] ON [track].[tblMovementHistory]
(
	[PK_MovementHistoryGuid] ASC
) INCLUDE([ChangeIndex], [UpdatedContext], [UpdatedRowVersion], [CurrentSiteGuid], [PreviousSiteGuid])
GO

CREATE TRIGGER track.trg_insupd_tblMovementHistory_DeletedRowVersionUpdate_ForSync
   ON track.tblMovementHistory
   AFTER UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    IF ( UPDATE( DeletedDate ) )
    BEGIN
        UPDATE t
            SET DeletedRowVersion = CONVERT(VARBINARY(8), i._RowVersion)
        FROM track.tblMovementHistory t
            INNER JOIN inserted i ON i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d ON d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL AND d.DeletedDate IS NULL
    END
END
