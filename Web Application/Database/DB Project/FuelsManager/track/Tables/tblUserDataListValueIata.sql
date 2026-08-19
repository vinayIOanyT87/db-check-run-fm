/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataListValueIata } */

/****** Object:  Table [track].[tblUserDataListValueIata]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblUserDataListValueIata]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblUserDataListValueIata]
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
	[PK_UserDataListValueIataGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataListValueIata_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END
GO
/****** Object:  Index [IX_track_tblUserDataListValueIata_InsertContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataListValueIata]') AND name = N'IX_track_tblUserDataListValueIata_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIata_InsertContext] ON [track].[tblUserDataListValueIata]
(
    [PK_UserDataListValueIataGuid] ASC,
    [InsertedRowVersion] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_track_tblUserDataListValueIata_UpdateContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataListValueIata]') AND name = N'IX_track_tblUserDataListValueIata_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIata_UpdateContext] ON [track].[tblUserDataListValueIata]
(
    [PK_UserDataListValueIataGuid] ASC,
    [UpdatedRowVersion] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_track_tblUserDataListValueIata_DeleteContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataListValueIata]') AND name = N'IX_track_tblUserDataListValueIata_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIata_DeleteContext] ON [track].[tblUserDataListValueIata]
(
    [PK_UserDataListValueIataGuid] ASC,
    [DeletedRowVersion] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataListValueIata]
    ON [track].[tblUserDataListValueIata]([PK_UserDataListValueIataGuid] ASC);
GO
/****** Object:  Index [IX_tblUserDataListValueIata_PK_CurrentSiteGuid]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataListValueIata]') AND name = N'IX_tblUserDataListValueIata_PK_CurrentSiteGuid')
CREATE NONCLUSTERED INDEX [IX_tblUserDataListValueIata_PK_CurrentSiteGuid] ON [track].[tblUserDataListValueIata]
(
    [PK_UserDataListValueIataGuid] ASC,
    [CurrentSiteGuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_track_tblUserDataListValueIata_PK_ParentFK]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataListValueIata]') AND name = N'IX_track_tblUserDataListValueIata_PK_ParentFK')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIata_PK_ParentFK] ON [track].[tblUserDataListValueIata]
(
    [PK_UserDataListValueIataGuid] ASC,
    [FK_ParentPK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIATA_DeletedRowVersion]
    ON [track].[tblUserDataListValueIATA]([DeletedRowVersion] ASC)
    INCLUDE([DeletedDate], [DeletedContext], [CurrentSiteGuid], [PreviousSiteGuid], [PK_UserDataListValueIATAGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIATA_InsertedRowVersion]
    ON [track].[tblUserDataListValueIATA]([InsertedRowVersion] ASC)
    INCLUDE([InsertedDate], [InsertedContext], [CurrentSiteGuid], [PreviousSiteGuid], [PK_UserDataListValueIATAGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIATA_PK_UserDataListValueIATAGuid]
    ON [track].[tblUserDataListValueIATA]([PK_UserDataListValueIATAGuid] ASC)
    INCLUDE([InsertedDate], [InsertedRowVersion], [InsertedContext], [UpdatedDate], [UpdatedRowVersion], [UpdatedContext], [DeletedDate], [DeletedRowVersion], [DeletedContext], [CurrentSiteGuid], [PreviousSiteGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIATA_UpdatedRowVersion]
    ON [track].[tblUserDataListValueIATA]([UpdatedRowVersion] ASC)
    INCLUDE([UpdatedDate], [UpdatedContext], [CurrentSiteGuid], [PreviousSiteGuid], [PK_UserDataListValueIATAGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueIata_PK_UserDataListValueIataGuid_Sync] ON [track].[tblUserDataListValueIata]
(
	[PK_UserDataListValueIataGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataListValueIata_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataListValueIata
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
        FROM track.tblUserDataListValueIata t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END