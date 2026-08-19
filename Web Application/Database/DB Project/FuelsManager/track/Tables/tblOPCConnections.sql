/* {CheckPoint: CREATING TRACKING TABLE for tblOPCConnections } */

/****** Object:  Table [track].[tblOPCConnections]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblOPCConnections]
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
	[PK_OPCConnectionGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblOPCConnections_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOPCConnections_PK_OPCConnectionGuid] ON [track].[tblOPCConnections]
(
    [PK_OPCConnectionGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOPCConnections_InsertedRowVersion] ON [track].[tblOPCConnections]
(
    [InsertedRowVersion] ASC,
    [PK_OPCConnectionGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOPCConnections_UpdatedRowVersion] ON [track].[tblOPCConnections]
(
    [UpdatedRowVersion] ASC,
    [PK_OPCConnectionGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOPCConnections_DeletedRowVersion] ON [track].[tblOPCConnections]
(
    [DeletedRowVersion] ASC,
    [PK_OPCConnectionGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblOPCConnections_PK_OPCConnectionGuid_Sync] ON [track].[tblOPCConnections]
(
	[PK_OPCConnectionGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblOPCConnections_DeletedRowVersionUpdate_ForSync
   ON track.tblOPCConnections
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
        FROM track.tblOPCConnections t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END