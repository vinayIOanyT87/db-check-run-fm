/* {CheckPoint: CREATING TRACKING TABLE for tblGroupToTransactionAlias } */

/****** Object:  Table [track].[tblGroupToTransactionAlias]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblGroupToTransactionAlias]
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
	[PK_GroupToTransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblGroupToTransactionAlias_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToTransactionAlias_PK_GroupToTransactionAliasGuid] ON [track].[tblGroupToTransactionAlias]
(
    [PK_GroupToTransactionAliasGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToTransactionAlias_InsertedRowVersion] ON [track].[tblGroupToTransactionAlias]
(
    [InsertedRowVersion] ASC,
    [PK_GroupToTransactionAliasGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToTransactionAlias_UpdatedRowVersion] ON [track].[tblGroupToTransactionAlias]
(
    [UpdatedRowVersion] ASC,
    [PK_GroupToTransactionAliasGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToTransactionAlias_DeletedRowVersion] ON [track].[tblGroupToTransactionAlias]
(
    [DeletedRowVersion] ASC,
    [PK_GroupToTransactionAliasGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGroupToTransactionAlias_PK_GroupToTransactionAliasGuid_Sync] ON [track].[tblGroupToTransactionAlias]
(
	[PK_GroupToTransactionAliasGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblGroupToTransactionAlias_DeletedRowVersionUpdate_ForSync
   ON track.tblGroupToTransactionAlias
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
        FROM track.tblGroupToTransactionAlias t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END