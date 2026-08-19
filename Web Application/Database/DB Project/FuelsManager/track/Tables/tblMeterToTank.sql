/* {CheckPoint: CREATING TRACKING TABLE for tblMeterToTank } */

/****** Object:  Table [track].[tblMeterToTank]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMeterToTank]
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
	[PK_MeterToTankGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMeterToTank_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMeterToTank_PK_MeterToTankGuid] ON [track].[tblMeterToTank]
(
    [PK_MeterToTankGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMeterToTank_InsertedRowVersion] ON [track].[tblMeterToTank]
(
    [InsertedRowVersion] ASC,
    [PK_MeterToTankGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMeterToTank_UpdatedRowVersion] ON [track].[tblMeterToTank]
(
    [UpdatedRowVersion] ASC,
    [PK_MeterToTankGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMeterToTank_DeletedRowVersion] ON [track].[tblMeterToTank]
(
    [DeletedRowVersion] ASC,
    [PK_MeterToTankGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMeterToTank_PK_MeterToTankGuid_Sync] ON [track].[tblMeterToTank]
(
	[PK_MeterToTankGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMeterToTank_DeletedRowVersionUpdate_ForSync
   ON track.tblMeterToTank
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
        FROM track.tblMeterToTank t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END