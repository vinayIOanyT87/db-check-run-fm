/* {CheckPoint: CREATING TRACKING TABLE for tblMeterToEquipment } */

--Creating Sync Tracking Table for tblMeterToEquipment
CREATE TABLE [track].[tblMeterToEquipment]
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
	[PK_MeterToEquipmentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMeterToEquipment_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeterToEquipment_PK_MeterToEquipmentGuid] ON [track].[tblMeterToEquipment]
(
	[PK_MeterToEquipmentGuid] ASC
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeterToEquipment_InsertedRowVersion] ON [track].[tblMeterToEquipment]
(
	[InsertedRowVersion] ASC,
	[PK_MeterToEquipmentGuid],
	[InsertedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeterToEquipment_UpdatedRowVersion] ON [track].[tblMeterToEquipment]
(
	[UpdatedRowVersion] ASC,
	[PK_MeterToEquipmentGuid],
	[UpdatedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeterToEquipment_DeletedRowVersion] ON [track].[tblMeterToEquipment]
(
	[DeletedRowVersion] ASC,
	[PK_MeterToEquipmentGuid],
	[DeletedContext]
)
GO
 
CREATE TRIGGER track.trg_insupd_tblMeterToEquipment_DeletedRowVersionUpdate_ForSync
   ON track.tblMeterToEquipment
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
        FROM track.tblMeterToEquipment t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END
GO
