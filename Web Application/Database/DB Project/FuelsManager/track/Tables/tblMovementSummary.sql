/* {CheckPoint: CREATING TRACKING TABLE for tblMovementSummary } */

/****** Object:  Table [track].[tblMovementSummary]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMovementSummary]
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
	[PK_MovementSummaryGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMovementSummary_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementSummary_PK_MovementSummaryGuid] ON [track].[tblMovementSummary]
(
    [PK_MovementSummaryGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementSummary_InsertedRowVersion] ON [track].[tblMovementSummary]
(
    [InsertedRowVersion] ASC,
    [PK_MovementSummaryGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementSummary_UpdatedRowVersion] ON [track].[tblMovementSummary]
(
    [UpdatedRowVersion] ASC,
    [PK_MovementSummaryGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMovementSummary_DeletedRowVersion] ON [track].[tblMovementSummary]
(
    [DeletedRowVersion] ASC,
    [PK_MovementSummaryGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMovementSummary_PK_MovementSummaryGuid_Sync] ON [track].[tblMovementSummary]
(
	[PK_MovementSummaryGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMovementSummary_DeletedRowVersionUpdate_ForSync
   ON track.tblMovementSummary
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
        FROM track.tblMovementSummary t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END