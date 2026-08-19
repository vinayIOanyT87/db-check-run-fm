/* {CheckPoint: CREATING TRACKING TABLE for tblEquipmentTypes } */

/****** Object:  Table [track].[tblEquipmentTypes]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEquipmentTypes]
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
	[PK_EquipmentTypeGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEquipmentTypes_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentTypes_PK_EquipmentTypeGuid] ON [track].[tblEquipmentTypes]
(
    [PK_EquipmentTypeGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentTypes_InsertedRowVersion] ON [track].[tblEquipmentTypes]
(
    [InsertedRowVersion] ASC,
    [PK_EquipmentTypeGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentTypes_UpdatedRowVersion] ON [track].[tblEquipmentTypes]
(
    [UpdatedRowVersion] ASC,
    [PK_EquipmentTypeGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentTypes_DeletedRowVersion] ON [track].[tblEquipmentTypes]
(
    [DeletedRowVersion] ASC,
    [PK_EquipmentTypeGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEquipmentTypes_PK_EquipmentTypeGuid_Sync] ON [track].[tblEquipmentTypes]
(
	[PK_EquipmentTypeGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEquipmentTypes_DeletedRowVersionUpdate_ForSync
   ON track.tblEquipmentTypes
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
        FROM track.tblEquipmentTypes t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END