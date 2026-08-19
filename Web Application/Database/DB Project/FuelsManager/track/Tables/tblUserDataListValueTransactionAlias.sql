/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataListValueTransactionAlias } */

/****** Object:  Table [track].[tblUserDataListValueTransactionAlias]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataListValueTransactionAlias]
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
	[PK_UserDataListValueTransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataListValueTransactionAlias_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueTransactionAlias_PK_UserDataListValueTransactionAliasGuid] ON [track].[tblUserDataListValueTransactionAlias]
(
    [PK_UserDataListValueTransactionAliasGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueTransactionAlias_InsertedRowVersion] ON [track].[tblUserDataListValueTransactionAlias]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataListValueTransactionAliasGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueTransactionAlias_UpdatedRowVersion] ON [track].[tblUserDataListValueTransactionAlias]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataListValueTransactionAliasGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueTransactionAlias_DeletedRowVersion] ON [track].[tblUserDataListValueTransactionAlias]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataListValueTransactionAliasGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueTransactionAlias_PK_UserDataListValueTransactionAliasGuid_Sync] ON [track].[tblUserDataListValueTransactionAlias]
(
	[PK_UserDataListValueTransactionAliasGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataListValueTransactionAlias_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataListValueTransactionAlias
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
        FROM track.tblUserDataListValueTransactionAlias t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END