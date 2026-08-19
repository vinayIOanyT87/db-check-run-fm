/* {CheckPoint: CREATING TRACKING TABLE for tblCloseoutInventory } */

/****** Object:  Table [track].[tblCloseoutInventory]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCloseoutInventory]
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
	[PK_CloseoutInventoryGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCloseoutInventory_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCloseoutInventory_PK_CloseoutInventoryGuid] ON [track].[tblCloseoutInventory]
(
    [PK_CloseoutInventoryGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCloseoutInventory_InsertedRowVersion] ON [track].[tblCloseoutInventory]
(
    [InsertedRowVersion] ASC,
    [PK_CloseoutInventoryGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCloseoutInventory_UpdatedRowVersion] ON [track].[tblCloseoutInventory]
(
    [UpdatedRowVersion] ASC,
    [PK_CloseoutInventoryGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCloseoutInventory_DeletedRowVersion] ON [track].[tblCloseoutInventory]
(
    [DeletedRowVersion] ASC,
    [PK_CloseoutInventoryGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCloseoutInventory_PK_CloseoutInventoryGuid_Sync] ON [track].[tblCloseoutInventory]
(
	[PK_CloseoutInventoryGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCloseoutInventory_DeletedRowVersionUpdate_ForSync
   ON track.tblCloseoutInventory
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
        FROM track.tblCloseoutInventory t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END