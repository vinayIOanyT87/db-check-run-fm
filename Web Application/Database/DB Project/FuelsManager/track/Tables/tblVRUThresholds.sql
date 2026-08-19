/* {CheckPoint: CREATING TRACKING TABLE for tblVRUThresholds } */

/****** Object:  Table [track].[tblVRUThresholds]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblVRUThresholds]
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
	[PK_VRUThresholdGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblVRUThresholds_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblVRUThresholds_PK_VRUThresholdGuid] ON [track].[tblVRUThresholds]
(
    [PK_VRUThresholdGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblVRUThresholds_InsertedRowVersion] ON [track].[tblVRUThresholds]
(
    [InsertedRowVersion] ASC,
    [PK_VRUThresholdGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblVRUThresholds_UpdatedRowVersion] ON [track].[tblVRUThresholds]
(
    [UpdatedRowVersion] ASC,
    [PK_VRUThresholdGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblVRUThresholds_DeletedRowVersion] ON [track].[tblVRUThresholds]
(
    [DeletedRowVersion] ASC,
    [PK_VRUThresholdGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblVRUThresholds_PK_VRUThresholdGuid_Sync] ON [track].[tblVRUThresholds]
(
	[PK_VRUThresholdGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblVRUThresholds_DeletedRowVersionUpdate_ForSync
   ON track.tblVRUThresholds
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
        FROM track.tblVRUThresholds t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END