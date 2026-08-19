/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableExternalMeterOutputPermissive } */

/****** Object:  Table [track].[tblProcessVariableExternalMeterOutputPermissive]   Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterOutputPermissive]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblProcessVariableExternalMeterOutputPermissive]
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
	CONSTRAINT [PK_track_tblProcessVariableExternalMeterOutputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END


GO
/****** Object:  Index [IX_track_tblProcessVariableExternalMeterOutputPermissive_InsertContext]    Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterOutputPermissive]') AND name = N'IX_track_tblProcessVariableExternalMeterOutputPermissive_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterOutputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableExternalMeterOutputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableExternalMeterOutputPermissive_UpdateContext]    Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterOutputPermissive]') AND name = N'IX_track_tblProcessVariableExternalMeterOutputPermissive_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterOutputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableExternalMeterOutputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableExternalMeterOutputPermissive_DeleteContext]    Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterOutputPermissive]') AND name = N'IX_track_tblProcessVariableExternalMeterOutputPermissive_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterOutputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableExternalMeterOutputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableExternalMeterOutputPermissive_PK_ProcessVariableProductToOffloadExternalMeterGuid]
    ON [track].[tblProcessVariableExternalMeterOutputPermissive]([PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterOutputPermissive_PK_ProcessVariableProductToOffloadExternalMeterGuid_Sync] ON [track].[tblProcessVariableExternalMeterOutputPermissive]
(
	[PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableExternalMeterOutputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableExternalMeterOutputPermissive
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
        FROM track.tblProcessVariableExternalMeterOutputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END