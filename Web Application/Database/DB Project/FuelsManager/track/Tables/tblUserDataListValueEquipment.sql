/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataListValueEquipment } */

/****** Object:  Table [track].[tblUserDataListValueEquipment]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataListValueEquipment]
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
	[PK_UserDataListValueEquipmentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataListValueEquipment_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueEquipment_PK_UserDataListValueEquipmentGuid] ON [track].[tblUserDataListValueEquipment]
(
    [PK_UserDataListValueEquipmentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueEquipment_InsertedRowVersion] ON [track].[tblUserDataListValueEquipment]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataListValueEquipmentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueEquipment_UpdatedRowVersion] ON [track].[tblUserDataListValueEquipment]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataListValueEquipmentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueEquipment_DeletedRowVersion] ON [track].[tblUserDataListValueEquipment]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataListValueEquipmentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueEquipment_PK_UserDataListValueEquipmentGuid_Sync] ON [track].[tblUserDataListValueEquipment]
(
	[PK_UserDataListValueEquipmentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataListValueEquipment_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataListValueEquipment
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
        FROM track.tblUserDataListValueEquipment t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END