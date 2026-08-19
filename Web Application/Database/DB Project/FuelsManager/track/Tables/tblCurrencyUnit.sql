/* {CheckPoint: CREATING TRACKING TABLE for tblCurrencyUnit } */

/****** Object:  Table [track].[tblCurrencyUnit]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCurrencyUnit]
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
	[PK_CurrencyUnitIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCurrencyUnit_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyUnit_PK_CurrencyUnitIndex] ON [track].[tblCurrencyUnit]
(
    [PK_CurrencyUnitIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyUnit_InsertedRowVersion] ON [track].[tblCurrencyUnit]
(
    [InsertedRowVersion] ASC,
    [PK_CurrencyUnitIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyUnit_UpdatedRowVersion] ON [track].[tblCurrencyUnit]
(
    [UpdatedRowVersion] ASC,
    [PK_CurrencyUnitIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyUnit_DeletedRowVersion] ON [track].[tblCurrencyUnit]
(
    [DeletedRowVersion] ASC,
    [PK_CurrencyUnitIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCurrencyUnit_PK_CurrencyUnitIndex_Sync] ON [track].[tblCurrencyUnit]
(
	[PK_CurrencyUnitIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCurrencyUnit_DeletedRowVersionUpdate_ForSync
   ON track.tblCurrencyUnit
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
        FROM track.tblCurrencyUnit t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END