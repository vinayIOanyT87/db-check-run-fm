/* {CheckPoint: CREATING TRACKING TABLE for tblPointAccessGroupToExposedPointSetting } */

/****** Object:  Table [track].[tblPointAccessGroupToExposedPointSetting]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointAccessGroupToExposedPointSetting]
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
	[PK_PointAccessGroupToExposedSettingGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointAccessGroupToExposedPointSetting_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToExposedPointSetting_PK_PointAccessGroupToExposedSettingGuid] ON [track].[tblPointAccessGroupToExposedPointSetting]
(
    [PK_PointAccessGroupToExposedSettingGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToExposedPointSetting_InsertedRowVersion] ON [track].[tblPointAccessGroupToExposedPointSetting]
(
    [InsertedRowVersion] ASC,
    [PK_PointAccessGroupToExposedSettingGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToExposedPointSetting_UpdatedRowVersion] ON [track].[tblPointAccessGroupToExposedPointSetting]
(
    [UpdatedRowVersion] ASC,
    [PK_PointAccessGroupToExposedSettingGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToExposedPointSetting_DeletedRowVersion] ON [track].[tblPointAccessGroupToExposedPointSetting]
(
    [DeletedRowVersion] ASC,
    [PK_PointAccessGroupToExposedSettingGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToExposedPointSetting_PK_PointAccessGroupToExposedSettingGuid_Sync] ON [track].[tblPointAccessGroupToExposedPointSetting]
(
	[PK_PointAccessGroupToExposedSettingGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointAccessGroupToExposedPointSetting_DeletedRowVersionUpdate_ForSync
   ON track.tblPointAccessGroupToExposedPointSetting
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
        FROM track.tblPointAccessGroupToExposedPointSetting t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END