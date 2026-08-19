/* {CheckPoint: CREATING TRACKING TABLE for tblTestEquipmentResults } */

/****** Object:  Table [track].[tblTestEquipmentResults]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTestEquipmentResults]
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
	[PK_TestEquipmentResultGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTestEquipmentResults_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestEquipmentResults_PK_TestEquipmentResultGuid] ON [track].[tblTestEquipmentResults]
(
    [PK_TestEquipmentResultGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestEquipmentResults_InsertedRowVersion] ON [track].[tblTestEquipmentResults]
(
    [InsertedRowVersion] ASC,
    [PK_TestEquipmentResultGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestEquipmentResults_UpdatedRowVersion] ON [track].[tblTestEquipmentResults]
(
    [UpdatedRowVersion] ASC,
    [PK_TestEquipmentResultGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestEquipmentResults_DeletedRowVersion] ON [track].[tblTestEquipmentResults]
(
    [DeletedRowVersion] ASC,
    [PK_TestEquipmentResultGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTestEquipmentResults_PK_TestEquipmentResultGuid_Sync] ON [track].[tblTestEquipmentResults]
(
	[PK_TestEquipmentResultGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTestEquipmentResults_DeletedRowVersionUpdate_ForSync
   ON track.tblTestEquipmentResults
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
        FROM track.tblTestEquipmentResults t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END