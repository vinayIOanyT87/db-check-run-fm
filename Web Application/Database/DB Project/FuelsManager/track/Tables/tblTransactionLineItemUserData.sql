/* {CheckPoint: CREATING TRACKING TABLE for tblTransactionLineItemUserData } */

/****** Object:  Table [track].[tblTransactionLineItemUserData]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTransactionLineItemUserData]
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
	[PK_TransactionLineItemUserDataGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTransactionLineItemUserData_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLineItemUserData_PK_TransactionLineItemUserDataGuid] ON [track].[tblTransactionLineItemUserData]
(
    [PK_TransactionLineItemUserDataGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLineItemUserData_InsertedRowVersion] ON [track].[tblTransactionLineItemUserData]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionLineItemUserDataGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLineItemUserData_UpdatedRowVersion] ON [track].[tblTransactionLineItemUserData]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionLineItemUserDataGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLineItemUserData_DeletedRowVersion] ON [track].[tblTransactionLineItemUserData]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionLineItemUserDataGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTransactionLineItemUserData_PK_TransactionLineItemUserDataGuid_Sync] ON [track].[tblTransactionLineItemUserData]
(
	[PK_TransactionLineItemUserDataGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTransactionLineItemUserData_DeletedRowVersionUpdate_ForSync
   ON track.tblTransactionLineItemUserData
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
        FROM track.tblTransactionLineItemUserData t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END