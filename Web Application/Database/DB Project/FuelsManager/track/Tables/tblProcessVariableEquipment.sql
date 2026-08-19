/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableEquipment } */

/****** Object:  Table [track].[tblProcessVariableEquipment]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableEquipment]
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
	[PK_ProcessVariableEquipmentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableEquipment_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableEquipment_PK_ProcessVariableEquipmentGuid] ON [track].[tblProcessVariableEquipment]
(
    [PK_ProcessVariableEquipmentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableEquipment_InsertedRowVersion] ON [track].[tblProcessVariableEquipment]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableEquipmentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableEquipment_UpdatedRowVersion] ON [track].[tblProcessVariableEquipment]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableEquipmentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableEquipment_DeletedRowVersion] ON [track].[tblProcessVariableEquipment]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableEquipmentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableEquipment_PK_ProcessVariableEquipmentGuid_Sync] ON [track].[tblProcessVariableEquipment]
(
	[PK_ProcessVariableEquipmentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableEquipment_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableEquipment
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
        FROM track.tblProcessVariableEquipment t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END