/* {CheckPoint: CREATING TRACKING TABLE for tblFuelCardLimitLineItem } */

/****** Object:  Table [track].[tblFuelCardLimitLineItem]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblFuelCardLimitLineItem]
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
	[PK_FuelCardLimitLineItemGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblFuelCardLimitLineItem_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitLineItem_PK_FuelCardLimitLineItemGuid] ON [track].[tblFuelCardLimitLineItem]
(
    [PK_FuelCardLimitLineItemGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitLineItem_InsertedRowVersion] ON [track].[tblFuelCardLimitLineItem]
(
    [InsertedRowVersion] ASC,
    [PK_FuelCardLimitLineItemGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitLineItem_UpdatedRowVersion] ON [track].[tblFuelCardLimitLineItem]
(
    [UpdatedRowVersion] ASC,
    [PK_FuelCardLimitLineItemGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitLineItem_DeletedRowVersion] ON [track].[tblFuelCardLimitLineItem]
(
    [DeletedRowVersion] ASC,
    [PK_FuelCardLimitLineItemGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitLineItem_PK_FuelCardLimitLineItemGuid_Sync] ON [track].[tblFuelCardLimitLineItem]
(
	[PK_FuelCardLimitLineItemGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblFuelCardLimitLineItem_DeletedRowVersionUpdate_ForSync
   ON track.tblFuelCardLimitLineItem
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
        FROM track.tblFuelCardLimitLineItem t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END