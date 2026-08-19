/* {CheckPoint: CREATING TRACKING TABLE for tblControllersLogToTransaction } */

/****** Object:  Table [track].[tblControllersLogToTransaction]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblControllersLogToTransaction]
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
	[PK_ControllersLogToTransactionGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblControllersLogToTransaction_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblControllersLogToTransaction_PK_ControllersLogToTransactionGuid] ON [track].[tblControllersLogToTransaction]
(
    [PK_ControllersLogToTransactionGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblControllersLogToTransaction_InsertedRowVersion] ON [track].[tblControllersLogToTransaction]
(
    [InsertedRowVersion] ASC,
    [PK_ControllersLogToTransactionGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblControllersLogToTransaction_UpdatedRowVersion] ON [track].[tblControllersLogToTransaction]
(
    [UpdatedRowVersion] ASC,
    [PK_ControllersLogToTransactionGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblControllersLogToTransaction_DeletedRowVersion] ON [track].[tblControllersLogToTransaction]
(
    [DeletedRowVersion] ASC,
    [PK_ControllersLogToTransactionGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblControllersLogToTransaction_PK_ControllersLogToTransactionGuid_Sync] ON [track].[tblControllersLogToTransaction]
(
	[PK_ControllersLogToTransactionGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblControllersLogToTransaction_DeletedRowVersionUpdate_ForSync
   ON track.tblControllersLogToTransaction
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
        FROM track.tblControllersLogToTransaction t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END