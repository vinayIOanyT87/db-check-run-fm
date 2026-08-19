/* {CheckPoint: CREATING TRACKING TABLE for tblEntityLedgerAggregateColumnToSite } */

/****** Object:  Table [track].[tblEntityLedgerAggregateColumnToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityLedgerAggregateColumnToSite]
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
	[PK_LedgerAggregateColumnToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityLedgerAggregateColumnToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityLedgerAggregateColumnToSite_PK_LedgerAggregateColumnToSiteGuid] ON [track].[tblEntityLedgerAggregateColumnToSite]
(
    [PK_LedgerAggregateColumnToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityLedgerAggregateColumnToSite_InsertedRowVersion] ON [track].[tblEntityLedgerAggregateColumnToSite]
(
    [InsertedRowVersion] ASC,
    [PK_LedgerAggregateColumnToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityLedgerAggregateColumnToSite_UpdatedRowVersion] ON [track].[tblEntityLedgerAggregateColumnToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_LedgerAggregateColumnToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityLedgerAggregateColumnToSite_DeletedRowVersion] ON [track].[tblEntityLedgerAggregateColumnToSite]
(
    [DeletedRowVersion] ASC,
    [PK_LedgerAggregateColumnToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityLedgerAggregateColumnToSite_PK_LedgerAggregateColumnToSiteGuid_Sync] ON [track].[tblEntityLedgerAggregateColumnToSite]
(
	[PK_LedgerAggregateColumnToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityLedgerAggregateColumnToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityLedgerAggregateColumnToSite
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
        FROM track.tblEntityLedgerAggregateColumnToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END