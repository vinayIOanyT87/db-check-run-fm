/* {CheckPoint: CREATING TRACKING TABLE for tblTestSetTankResults } */

/****** Object:  Table [track].[tblTestSetTankResults]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTestSetTankResults]
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
	[PK_TestSetTankResultGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTestSetTankResults_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetTankResults_PK_TestSetTankResultGuid] ON [track].[tblTestSetTankResults]
(
    [PK_TestSetTankResultGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetTankResults_InsertedRowVersion] ON [track].[tblTestSetTankResults]
(
    [InsertedRowVersion] ASC,
    [PK_TestSetTankResultGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetTankResults_UpdatedRowVersion] ON [track].[tblTestSetTankResults]
(
    [UpdatedRowVersion] ASC,
    [PK_TestSetTankResultGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetTankResults_DeletedRowVersion] ON [track].[tblTestSetTankResults]
(
    [DeletedRowVersion] ASC,
    [PK_TestSetTankResultGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTestSetTankResults_PK_TestSetTankResultGuid_Sync] ON [track].[tblTestSetTankResults]
(
	[PK_TestSetTankResultGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTestSetTankResults_DeletedRowVersionUpdate_ForSync
   ON track.tblTestSetTankResults
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
        FROM track.tblTestSetTankResults t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END