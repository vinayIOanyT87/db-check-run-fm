/* {CheckPoint: CREATING TRACKING TABLE for tblTransactionTypes } */

/****** Object:  Table [track].[tblTransactionTypes]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTransactionTypes]
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
	[PK_TransactionTypesIndex] [smallint] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTransactionTypes_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionTypes_PK_TransactionTypesIndex] ON [track].[tblTransactionTypes]
(
    [PK_TransactionTypesIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionTypes_InsertedRowVersion] ON [track].[tblTransactionTypes]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionTypesIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionTypes_UpdatedRowVersion] ON [track].[tblTransactionTypes]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionTypesIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionTypes_DeletedRowVersion] ON [track].[tblTransactionTypes]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionTypesIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTransactionTypes_PK_TransactionTypesIndex_Sync] ON [track].[tblTransactionTypes]
(
	[PK_TransactionTypesIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTransactionTypes_DeletedRowVersionUpdate_ForSync
   ON track.tblTransactionTypes
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
        FROM track.tblTransactionTypes t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END