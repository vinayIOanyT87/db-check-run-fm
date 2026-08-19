/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableExternalMeterInputPermissive } */

/****** Object:  Table [track].[tblProcessVariableExternalMeterInputPermissive]   Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterInputPermissive]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblProcessVariableExternalMeterInputPermissive]
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
	CONSTRAINT [PK_track_tblProcessVariableExternalMeterInputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END


GO
/****** Object:  Index [IX_track_tblProcessVariableExternalMeterInputPermissive_InsertContext]    Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterInputPermissive]') AND name = N'IX_track_tblProcessVariableExternalMeterInputPermissive_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterInputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableExternalMeterInputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableExternalMeterInputPermissive_UpdateContext]    Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterInputPermissive]') AND name = N'IX_track_tblProcessVariableExternalMeterInputPermissive_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterInputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableExternalMeterInputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableExternalMeterInputPermissive_DeleteContext]    Script Date: 9/29/2016 1:00:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableExternalMeterInputPermissive]') AND name = N'IX_track_tblProcessVariableExternalMeterInputPermissive_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterInputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableExternalMeterInputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableExternalMeterInputPermissive_PK_ProcessVariableProductToOffloadExternalMeterGuid]
    ON [track].[tblProcessVariableExternalMeterInputPermissive]([PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalMeterInputPermissive_PK_ProcessVariableProductToOffloadExternalMeterGuid_Sync] ON [track].[tblProcessVariableExternalMeterInputPermissive]
(
	[PK_ProcessVariableProductToOffloadExternalMeterGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableExternalMeterInputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableExternalMeterInputPermissive
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
        FROM track.tblProcessVariableExternalMeterInputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END