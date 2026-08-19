/* {CheckPoint: CREATING TRACKING TABLE for tblProductToOffloadExternalMeter } */

/****** Object:  Table [track].[tblProductToOffloadExternalMeter]   Script Date: 9/16/2016 7:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblProductToOffloadExternalMeter]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblProductToOffloadExternalMeter]
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
	[PK_ProductToOffloadExternalMeterGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToOffloadExternalMeter_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END


GO
/****** Object:  Index [IX_track_tblProductToOffloadExternalMeter_InsertContext]    Script Date: 9/16/2016 7:26:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProductToOffloadExternalMeter]') AND name = N'IX_track_tblProductToOffloadExternalMeter_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProductToOffloadExternalMeter_InsertContext] ON [track].[tblProductToOffloadExternalMeter]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToOffloadExternalMeterGuid] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProductToOffloadExternalMeter_UpdateContext]    Script Date: 9/16/2016 7:26:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProductToOffloadExternalMeter]') AND name = N'IX_track_tblProductToOffloadExternalMeter_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProductToOffloadExternalMeter_UpdateContext] ON [track].[tblProductToOffloadExternalMeter]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToOffloadExternalMeterGuid] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProductToOffloadExternalMeter_DeleteContext]    Script Date: 9/16/2016 7:26:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProductToOffloadExternalMeter]') AND name = N'IX_track_tblProductToOffloadExternalMeter_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProductToOffloadExternalMeter_DeleteContext] ON [track].[tblProductToOffloadExternalMeter]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToOffloadExternalMeterGuid] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE NONCLUSTERED INDEX [IX_tblProductToOffloadExternalMeter_PK_ProductToOffloadExternalMeterGuid]
    ON [track].[tblProductToOffloadExternalMeter]([PK_ProductToOffloadExternalMeterGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToOffloadExternalMeter_PK_ProductToOffloadExternalMeterGuid_Sync] ON [track].[tblProductToOffloadExternalMeter]
(
	[PK_ProductToOffloadExternalMeterGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToOffloadExternalMeter_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToOffloadExternalMeter
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
        FROM track.tblProductToOffloadExternalMeter t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END