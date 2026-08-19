/* {CheckPoint: CREATING TRACKING TABLE for tblFuelCardLimitPeriod } */

/****** Object:  Table [track].[tblFuelCardLimitPeriod]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblFuelCardLimitPeriod]
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
	[PK_FuelCardLimitPeriodIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblFuelCardLimitPeriod_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitPeriod_PK_FuelCardLimitPeriodIndex] ON [track].[tblFuelCardLimitPeriod]
(
    [PK_FuelCardLimitPeriodIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitPeriod_InsertedRowVersion] ON [track].[tblFuelCardLimitPeriod]
(
    [InsertedRowVersion] ASC,
    [PK_FuelCardLimitPeriodIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitPeriod_UpdatedRowVersion] ON [track].[tblFuelCardLimitPeriod]
(
    [UpdatedRowVersion] ASC,
    [PK_FuelCardLimitPeriodIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitPeriod_DeletedRowVersion] ON [track].[tblFuelCardLimitPeriod]
(
    [DeletedRowVersion] ASC,
    [PK_FuelCardLimitPeriodIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblFuelCardLimitPeriod_PK_FuelCardLimitPeriodIndex_Sync] ON [track].[tblFuelCardLimitPeriod]
(
	[PK_FuelCardLimitPeriodIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblFuelCardLimitPeriod_DeletedRowVersionUpdate_ForSync
   ON track.tblFuelCardLimitPeriod
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
        FROM track.tblFuelCardLimitPeriod t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END