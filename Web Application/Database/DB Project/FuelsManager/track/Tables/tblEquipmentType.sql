/* {CheckPoint: CREATING TRACKING TABLE for tblEquipmentType } */

/****** Object:  Table [track].[tblEquipmentType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEquipmentType]
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
	[PK_EquipmentTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEquipmentType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentType_PK_EquipmentTypeIndex] ON [track].[tblEquipmentType]
(
    [PK_EquipmentTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentType_InsertedRowVersion] ON [track].[tblEquipmentType]
(
    [InsertedRowVersion] ASC,
    [PK_EquipmentTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentType_UpdatedRowVersion] ON [track].[tblEquipmentType]
(
    [UpdatedRowVersion] ASC,
    [PK_EquipmentTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentType_DeletedRowVersion] ON [track].[tblEquipmentType]
(
    [DeletedRowVersion] ASC,
    [PK_EquipmentTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentType_PK_EquipmentTypeIndex_Sync] ON [track].[tblEquipmentType]
(
	[PK_EquipmentTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEquipmentType_DeletedRowVersionUpdate_ForSync
   ON track.tblEquipmentType
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
        FROM track.tblEquipmentType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END