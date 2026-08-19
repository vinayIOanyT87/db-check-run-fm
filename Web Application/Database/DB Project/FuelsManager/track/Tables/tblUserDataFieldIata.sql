/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataFieldIata } */

/****** Object:  Table [track].[tblUserDataFieldIata]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblUserDataFieldIata]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblUserDataFieldIata]
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
	[PK_UserDataFieldIataGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataFieldIata_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END
GO
/****** Object:  Index [IX_track_tblUserDataFieldIata_InsertContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataFieldIata]') AND name = N'IX_track_tblUserDataFieldIata_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIata_InsertContext] ON [track].[tblUserDataFieldIata]
(
    [PK_UserDataFieldIataGuid] ASC,
    [InsertedRowVersion] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_track_tblUserDataFieldIata_UpdateContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataFieldIata]') AND name = N'IX_track_tblUserDataFieldIata_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIata_UpdateContext] ON [track].[tblUserDataFieldIata]
(
    [PK_UserDataFieldIataGuid] ASC,
    [UpdatedRowVersion] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_track_tblUserDataFieldIata_DeleteContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataFieldIata]') AND name = N'IX_track_tblUserDataFieldIata_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIata_DeleteContext] ON [track].[tblUserDataFieldIata]
(
    [PK_UserDataFieldIataGuid] ASC,
    [DeletedRowVersion] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblUserDataFieldIata_PK_CurrentSiteGuid]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataFieldIata]') AND name = N'IX_tblUserDataFieldIata_PK_CurrentSiteGuid')
CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldIata_PK_CurrentSiteGuid] ON [track].[tblUserDataFieldIata]
(
    [PK_UserDataFieldIataGuid] ASC,
    [CurrentSiteGuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_track_tblUserDataFieldIata_PK_ParentFK]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblUserDataFieldIata]') AND name = N'IX_track_tblUserDataFieldIata_PK_ParentFK')
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIata_PK_ParentFK] ON [track].[tblUserDataFieldIata]
(
    [PK_UserDataFieldIataGuid] ASC,
    [FK_ParentPK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIATA_DeletedRowVersion]
    ON [track].[tblUserDataFieldIATA]([DeletedRowVersion] ASC)
    INCLUDE([DeletedDate], [DeletedContext], [CurrentSiteGuid], [PreviousSiteGuid], [PK_UserDataFieldIATAGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIATA_InsertedRowVersion]
    ON [track].[tblUserDataFieldIATA]([InsertedRowVersion] ASC)
    INCLUDE([InsertedDate], [InsertedContext], [CurrentSiteGuid], [PreviousSiteGuid], [PK_UserDataFieldIATAGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIATA_PK_UserDataFieldIATAGuid]
    ON [track].[tblUserDataFieldIATA]([PK_UserDataFieldIATAGuid] ASC)
    INCLUDE([InsertedDate], [InsertedRowVersion], [InsertedContext], [UpdatedDate], [UpdatedRowVersion], [UpdatedContext], [DeletedDate], [DeletedRowVersion], [DeletedContext], [CurrentSiteGuid], [PreviousSiteGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIATA_UpdatedRowVersion]
    ON [track].[tblUserDataFieldIATA]([UpdatedRowVersion] ASC)
    INCLUDE([UpdatedDate], [UpdatedContext], [CurrentSiteGuid], [PreviousSiteGuid], [PK_UserDataFieldIATAGuid]) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldIata_PK_UserDataFieldIataGuid_Sync] ON [track].[tblUserDataFieldIata]
(
	[PK_UserDataFieldIataGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataFieldIata_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataFieldIata
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
        FROM track.tblUserDataFieldIata t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END