/* {CheckPoint: CREATING TRACKING TABLE for tblPointAccessGroupToAlarmTest } */

/****** Object:  Table [track].[tblPointAccessGroupToAlarmTest]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointAccessGroupToAlarmTest]
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
	[PK_PointAccessGroupToAlarmTestGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointAccessGroupToAlarmTest_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToAlarmTest_PK_PointAccessGroupToAlarmTestGuid] ON [track].[tblPointAccessGroupToAlarmTest]
(
    [PK_PointAccessGroupToAlarmTestGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToAlarmTest_InsertedRowVersion] ON [track].[tblPointAccessGroupToAlarmTest]
(
    [InsertedRowVersion] ASC,
    [PK_PointAccessGroupToAlarmTestGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToAlarmTest_UpdatedRowVersion] ON [track].[tblPointAccessGroupToAlarmTest]
(
    [UpdatedRowVersion] ASC,
    [PK_PointAccessGroupToAlarmTestGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToAlarmTest_DeletedRowVersion] ON [track].[tblPointAccessGroupToAlarmTest]
(
    [DeletedRowVersion] ASC,
    [PK_PointAccessGroupToAlarmTestGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToAlarmTest_PK_PointAccessGroupToAlarmTestGuid_Sync] ON [track].[tblPointAccessGroupToAlarmTest]
(
	[PK_PointAccessGroupToAlarmTestGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointAccessGroupToAlarmTest_DeletedRowVersionUpdate_ForSync
   ON track.tblPointAccessGroupToAlarmTest
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
        FROM track.tblPointAccessGroupToAlarmTest t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END