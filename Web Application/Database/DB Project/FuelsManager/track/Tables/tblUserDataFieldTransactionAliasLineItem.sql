/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataFieldTransactionAliasLineItem } */

/****** Object:  Table [track].[tblUserDataFieldTransactionAliasLineItem]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataFieldTransactionAliasLineItem]
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
	[PK_UserDataFieldTransactionAliasLineItemGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataFieldTransactionAliasLineItem_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldTransactionAliasLineItem_PK_UserDataFieldTransactionAliasLineItemGuid] ON [track].[tblUserDataFieldTransactionAliasLineItem]
(
    [PK_UserDataFieldTransactionAliasLineItemGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldTransactionAliasLineItem_InsertedRowVersion] ON [track].[tblUserDataFieldTransactionAliasLineItem]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataFieldTransactionAliasLineItemGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldTransactionAliasLineItem_UpdatedRowVersion] ON [track].[tblUserDataFieldTransactionAliasLineItem]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataFieldTransactionAliasLineItemGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldTransactionAliasLineItem_DeletedRowVersion] ON [track].[tblUserDataFieldTransactionAliasLineItem]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataFieldTransactionAliasLineItemGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldTransactionAliasLineItem_PK_UserDataFieldTransactionAliasLineItemGuid_Sync] ON [track].[tblUserDataFieldTransactionAliasLineItem]
(
	[PK_UserDataFieldTransactionAliasLineItemGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataFieldTransactionAliasLineItem_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataFieldTransactionAliasLineItem
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
        FROM track.tblUserDataFieldTransactionAliasLineItem t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END