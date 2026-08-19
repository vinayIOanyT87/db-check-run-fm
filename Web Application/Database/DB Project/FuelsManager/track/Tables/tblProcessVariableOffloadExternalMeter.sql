/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableOffloadExternalMeter } */

/****** Object:  Table [track].[tblProcessVariableOffloadExternalMeter]   Script Date: 9/20/2016 8:47:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableOffloadExternalMeter]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblProcessVariableOffloadExternalMeter]
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
	[PK_ProcessVariableProductToOffloadExternalMeterGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableOffloadExternalMeter_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END


GO
/****** Object:  Index [IX_track_tblProcessVariableOffloadExternalMeter_InsertContext]    Script Date: 9/20/2016 8:47:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableOffloadExternalMeter]') AND name = N'IX_track_tblProcessVariableOffloadExternalMeter_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableOffloadExternalMeter_InsertedRowVersion] ON [track].[tblProcessVariableOffloadExternalMeter]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableOffloadExternalMeter_UpdateContext]    Script Date: 9/20/2016 8:47:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableOffloadExternalMeter]') AND name = N'IX_track_tblProcessVariableOffloadExternalMeter_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableOffloadExternalMeter_UpdatedRowVersion] ON [track].[tblProcessVariableOffloadExternalMeter]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableOffloadExternalMeter_DeleteContext]    Script Date: 9/20/2016 8:47:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableOffloadExternalMeter]') AND name = N'IX_track_tblProcessVariableOffloadExternalMeter_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableOffloadExternalMeter_DeletedRowVersion] ON [track].[tblProcessVariableOffloadExternalMeter]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableOffloadExternalMeter_PK_ProcessVariableProductToOffloadExternalMeterGuid]
    ON [track].[tblProcessVariableOffloadExternalMeter]([PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableOffloadExternalMeter_PK_ProcessVariableProductToOffloadExternalMeterGuid_Sync] ON [track].[tblProcessVariableOffloadExternalMeter]
(
	[PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableOffloadExternalMeter_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableOffloadExternalMeter
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
        FROM track.tblProcessVariableOffloadExternalMeter t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END