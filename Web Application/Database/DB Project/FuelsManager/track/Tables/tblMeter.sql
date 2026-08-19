/* {CheckPoint: CREATING TRACKING TABLE for tblMeter } */

--Creating Sync Tracking Table for tblMeter
CREATE TABLE [track].[tblMeter]
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
	[PK_MeterGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMeter_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeter_PK_MeterGuid] ON [track].[tblMeter]
(
	[PK_MeterGuid] ASC
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeter_InsertedRowVersion] ON [track].[tblMeter]
(
	[InsertedRowVersion] ASC,
	[PK_MeterGuid],
	[InsertedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeter_UpdatedRowVersion] ON [track].[tblMeter]
(
	[UpdatedRowVersion] ASC,
	[PK_MeterGuid],
	[UpdatedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblMeter_DeletedRowVersion] ON [track].[tblMeter]
(
	[DeletedRowVersion] ASC,
	[PK_MeterGuid],
	[DeletedContext]
)
GO
 
CREATE TRIGGER track.trg_insupd_tblMeter_DeletedRowVersionUpdate_ForSync
   ON track.tblMeter
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
        FROM track.tblMeter t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END
GO
