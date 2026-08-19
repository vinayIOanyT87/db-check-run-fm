/* {CheckPoint: CREATING TRACKING TABLE for tblTestSetStatus } */

/****** Object:  Table [track].[tblTestSetStatus]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTestSetStatus]
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
	[PK_TestSetStatusIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTestSetStatus_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetStatus_PK_TestSetStatusIndex] ON [track].[tblTestSetStatus]
(
    [PK_TestSetStatusIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetStatus_InsertedRowVersion] ON [track].[tblTestSetStatus]
(
    [InsertedRowVersion] ASC,
    [PK_TestSetStatusIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetStatus_UpdatedRowVersion] ON [track].[tblTestSetStatus]
(
    [UpdatedRowVersion] ASC,
    [PK_TestSetStatusIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetStatus_DeletedRowVersion] ON [track].[tblTestSetStatus]
(
    [DeletedRowVersion] ASC,
    [PK_TestSetStatusIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTestSetStatus_PK_TestSetStatusIndex_Sync] ON [track].[tblTestSetStatus]
(
	[PK_TestSetStatusIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTestSetStatus_DeletedRowVersionUpdate_ForSync
   ON track.tblTestSetStatus
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
        FROM track.tblTestSetStatus t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END