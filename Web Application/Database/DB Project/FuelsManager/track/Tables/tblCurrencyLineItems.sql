/* {CheckPoint: CREATING TRACKING TABLE for tblCurrencyLineItems } */

/****** Object:  Table [track].[tblCurrencyLineItems]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCurrencyLineItems]
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
	[PK_CurrencyLineItemGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCurrencyLineItems_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyLineItems_PK_CurrencyLineItemGuid] ON [track].[tblCurrencyLineItems]
(
    [PK_CurrencyLineItemGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyLineItems_InsertedRowVersion] ON [track].[tblCurrencyLineItems]
(
    [InsertedRowVersion] ASC,
    [PK_CurrencyLineItemGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyLineItems_UpdatedRowVersion] ON [track].[tblCurrencyLineItems]
(
    [UpdatedRowVersion] ASC,
    [PK_CurrencyLineItemGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyLineItems_DeletedRowVersion] ON [track].[tblCurrencyLineItems]
(
    [DeletedRowVersion] ASC,
    [PK_CurrencyLineItemGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyLineItems_PK_CurrencyLineItemGuid_Sync] ON [track].[tblCurrencyLineItems]
(
	[PK_CurrencyLineItemGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCurrencyLineItems_DeletedRowVersionUpdate_ForSync
   ON track.tblCurrencyLineItems
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
        FROM track.tblCurrencyLineItems t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END