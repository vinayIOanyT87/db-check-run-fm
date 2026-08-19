/* {CheckPoint: CREATING TRACKING TABLE for tblTankQualityTagLog } */

/****** Object:  Table [track].[tblTankQualityTagLog]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTankQualityTagLog]
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
	[PK_TankQualityTagLogGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTankQualityTagLog_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankQualityTagLog_PK_TankQualityTagLogGuid] ON [track].[tblTankQualityTagLog]
(
    [PK_TankQualityTagLogGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankQualityTagLog_InsertedRowVersion] ON [track].[tblTankQualityTagLog]
(
    [InsertedRowVersion] ASC,
    [PK_TankQualityTagLogGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankQualityTagLog_UpdatedRowVersion] ON [track].[tblTankQualityTagLog]
(
    [UpdatedRowVersion] ASC,
    [PK_TankQualityTagLogGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTankQualityTagLog_DeletedRowVersion] ON [track].[tblTankQualityTagLog]
(
    [DeletedRowVersion] ASC,
    [PK_TankQualityTagLogGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTankQualityTagLog_PK_TankQualityTagLogGuid_Sync] ON [track].[tblTankQualityTagLog]
(
	[PK_TankQualityTagLogGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTankQualityTagLog_DeletedRowVersionUpdate_ForSync
   ON track.tblTankQualityTagLog
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
        FROM track.tblTankQualityTagLog t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END