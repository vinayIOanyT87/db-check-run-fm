/* {CheckPoint: CREATING TRACKING TABLE for tblOpcUaServer } */

/****** Object:  Table [track].[tblOpcUaServer]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblOpcUaServer]
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
	[PK_OpcUaServerGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblOpcUaServer_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOpcUaServer_PK_OpcUaServerGuid] ON [track].[tblOpcUaServer]
(
    [PK_OpcUaServerGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOpcUaServer_InsertedRowVersion] ON [track].[tblOpcUaServer]
(
    [InsertedRowVersion] ASC,
    [PK_OpcUaServerGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOpcUaServer_UpdatedRowVersion] ON [track].[tblOpcUaServer]
(
    [UpdatedRowVersion] ASC,
    [PK_OpcUaServerGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOpcUaServer_DeletedRowVersion] ON [track].[tblOpcUaServer]
(
    [DeletedRowVersion] ASC,
    [PK_OpcUaServerGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblOpcUaServer_PK_OpcUaServerGuid_Sync] ON [track].[tblOpcUaServer]
(
	[PK_OpcUaServerGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblOpcUaServer_DeletedRowVersionUpdate_ForSync
   ON track.tblOpcUaServer
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
        FROM track.tblOpcUaServer t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END