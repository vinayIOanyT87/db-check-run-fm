/* {CheckPoint: CREATING TRACKING TABLE for tblScheduleHoliday } */

/****** Object:  Table [track].[tblScheduleHoliday]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblScheduleHoliday]
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
	[PK_ScheduleHolidayGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblScheduleHoliday_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleHoliday_PK_ScheduleHolidayGuid] ON [track].[tblScheduleHoliday]
(
    [PK_ScheduleHolidayGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleHoliday_InsertedRowVersion] ON [track].[tblScheduleHoliday]
(
    [InsertedRowVersion] ASC,
    [PK_ScheduleHolidayGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleHoliday_UpdatedRowVersion] ON [track].[tblScheduleHoliday]
(
    [UpdatedRowVersion] ASC,
    [PK_ScheduleHolidayGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblScheduleHoliday_DeletedRowVersion] ON [track].[tblScheduleHoliday]
(
    [DeletedRowVersion] ASC,
    [PK_ScheduleHolidayGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblScheduleHoliday_PK_ScheduleHolidayGuid_Sync] ON [track].[tblScheduleHoliday]
(
	[PK_ScheduleHolidayGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblScheduleHoliday_DeletedRowVersionUpdate_ForSync
   ON track.tblScheduleHoliday
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
        FROM track.tblScheduleHoliday t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END