/* {CheckPoint: CREATING TRACKING TABLE for tblResetPeriod } */

/****** Object:  Table [track].[tblResetPeriod]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblResetPeriod]
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
	[PK_ResetPeriodIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblResetPeriod_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblResetPeriod_PK_ResetPeriodIndex] ON [track].[tblResetPeriod]
(
    [PK_ResetPeriodIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblResetPeriod_InsertedRowVersion] ON [track].[tblResetPeriod]
(
    [InsertedRowVersion] ASC,
    [PK_ResetPeriodIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblResetPeriod_UpdatedRowVersion] ON [track].[tblResetPeriod]
(
    [UpdatedRowVersion] ASC,
    [PK_ResetPeriodIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblResetPeriod_DeletedRowVersion] ON [track].[tblResetPeriod]
(
    [DeletedRowVersion] ASC,
    [PK_ResetPeriodIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblResetPeriod_PK_ResetPeriodIndex_Sync] ON [track].[tblResetPeriod]
(
	[PK_ResetPeriodIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblResetPeriod_DeletedRowVersionUpdate_ForSync
   ON track.tblResetPeriod
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
        FROM track.tblResetPeriod t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END