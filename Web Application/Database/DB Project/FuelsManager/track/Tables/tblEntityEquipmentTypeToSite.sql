/* {CheckPoint: CREATING TRACKING TABLE for tblEntityEquipmentTypeToSite } */

/****** Object:  Table [track].[tblEntityEquipmentTypeToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityEquipmentTypeToSite]
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
	[PK_EquipmentTypeToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityEquipmentTypeToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityEquipmentTypeToSite_PK_EquipmentTypeToSiteGuid] ON [track].[tblEntityEquipmentTypeToSite]
(
    [PK_EquipmentTypeToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityEquipmentTypeToSite_InsertedRowVersion] ON [track].[tblEntityEquipmentTypeToSite]
(
    [InsertedRowVersion] ASC,
    [PK_EquipmentTypeToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityEquipmentTypeToSite_UpdatedRowVersion] ON [track].[tblEntityEquipmentTypeToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_EquipmentTypeToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityEquipmentTypeToSite_DeletedRowVersion] ON [track].[tblEntityEquipmentTypeToSite]
(
    [DeletedRowVersion] ASC,
    [PK_EquipmentTypeToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityEquipmentTypeToSite_PK_EquipmentTypeToSiteGuid_Sync] ON [track].[tblEntityEquipmentTypeToSite]
(
	[PK_EquipmentTypeToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityEquipmentTypeToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityEquipmentTypeToSite
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
        FROM track.tblEntityEquipmentTypeToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END