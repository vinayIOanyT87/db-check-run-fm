/* {CheckPoint: CREATING TRACKING TABLE for tblEquipmentQualityTagLog } */

/****** Object:  Table [track].[tblEquipmentQualityTagLog]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEquipmentQualityTagLog]
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
	[PK_EquipmentQualityTagLogGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEquipmentQualityTagLog_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentQualityTagLog_PK_EquipmentQualityTagLogGuid] ON [track].[tblEquipmentQualityTagLog]
(
    [PK_EquipmentQualityTagLogGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentQualityTagLog_InsertedRowVersion] ON [track].[tblEquipmentQualityTagLog]
(
    [InsertedRowVersion] ASC,
    [PK_EquipmentQualityTagLogGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentQualityTagLog_UpdatedRowVersion] ON [track].[tblEquipmentQualityTagLog]
(
    [UpdatedRowVersion] ASC,
    [PK_EquipmentQualityTagLogGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentQualityTagLog_DeletedRowVersion] ON [track].[tblEquipmentQualityTagLog]
(
    [DeletedRowVersion] ASC,
    [PK_EquipmentQualityTagLogGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentQualityTagLog_PK_EquipmentQualityTagLogGuid_Sync] ON [track].[tblEquipmentQualityTagLog]
(
	[PK_EquipmentQualityTagLogGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEquipmentQualityTagLog_DeletedRowVersionUpdate_ForSync
   ON track.tblEquipmentQualityTagLog
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
        FROM track.tblEquipmentQualityTagLog t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END