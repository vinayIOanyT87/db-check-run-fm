/* {CheckPoint: CREATING TRACKING TABLE for tblTransactionAliasToAutoDistributionRule } */

/****** Object:  Table [track].[tblTransactionAliasToAutoDistributionRule]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTransactionAliasToAutoDistributionRule]
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
	[PK_TransactionAliasToAutoDistributionRuleGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTransactionAliasToAutoDistributionRule_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliasToAutoDistributionRule_PK_TransactionAliasToAutoDistributionRuleGuid] ON [track].[tblTransactionAliasToAutoDistributionRule]
(
    [PK_TransactionAliasToAutoDistributionRuleGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliasToAutoDistributionRule_InsertedRowVersion] ON [track].[tblTransactionAliasToAutoDistributionRule]
(
    [InsertedRowVersion] ASC,
    [PK_TransactionAliasToAutoDistributionRuleGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliasToAutoDistributionRule_UpdatedRowVersion] ON [track].[tblTransactionAliasToAutoDistributionRule]
(
    [UpdatedRowVersion] ASC,
    [PK_TransactionAliasToAutoDistributionRuleGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliasToAutoDistributionRule_DeletedRowVersion] ON [track].[tblTransactionAliasToAutoDistributionRule]
(
    [DeletedRowVersion] ASC,
    [PK_TransactionAliasToAutoDistributionRuleGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTransactionAliasToAutoDistributionRule_PK_TransactionAliasToAutoDistributionRuleGuid_Sync] ON [track].[tblTransactionAliasToAutoDistributionRule]
(
	[PK_TransactionAliasToAutoDistributionRuleGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTransactionAliasToAutoDistributionRule_DeletedRowVersionUpdate_ForSync
   ON track.tblTransactionAliasToAutoDistributionRule
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
        FROM track.tblTransactionAliasToAutoDistributionRule t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END