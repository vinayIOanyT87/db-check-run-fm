/* {CheckPoint: CREATING TRACKING TABLE for tblTestSetEquipmentResults } */

/****** Object:  Table [track].[tblTestSetEquipmentResults]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTestSetEquipmentResults]
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
	[PK_TestSetEquipmentResultGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTestSetEquipmentResults_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetEquipmentResults_PK_TestSetEquipmentResultGuid] ON [track].[tblTestSetEquipmentResults]
(
    [PK_TestSetEquipmentResultGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetEquipmentResults_InsertedRowVersion] ON [track].[tblTestSetEquipmentResults]
(
    [InsertedRowVersion] ASC,
    [PK_TestSetEquipmentResultGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetEquipmentResults_UpdatedRowVersion] ON [track].[tblTestSetEquipmentResults]
(
    [UpdatedRowVersion] ASC,
    [PK_TestSetEquipmentResultGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestSetEquipmentResults_DeletedRowVersion] ON [track].[tblTestSetEquipmentResults]
(
    [DeletedRowVersion] ASC,
    [PK_TestSetEquipmentResultGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTestSetEquipmentResults_PK_TestSetEquipmentResultGuid_Sync] ON [track].[tblTestSetEquipmentResults]
(
	[PK_TestSetEquipmentResultGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTestSetEquipmentResults_DeletedRowVersionUpdate_ForSync
   ON track.tblTestSetEquipmentResults
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
        FROM track.tblTestSetEquipmentResults t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END