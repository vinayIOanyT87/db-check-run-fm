/* {CheckPoint: CREATING TRACKING TABLE for tblEntityDotHazardousMessagesToSite } */

/****** Object:  Table [track].[tblEntityDotHazardousMessagesToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityDotHazardousMessagesToSite]
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
	[PK_DotHazardousMessagesToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityDotHazardousMessagesToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDotHazardousMessagesToSite_PK_DotHazardousMessagesToSiteGuid] ON [track].[tblEntityDotHazardousMessagesToSite]
(
    [PK_DotHazardousMessagesToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDotHazardousMessagesToSite_InsertedRowVersion] ON [track].[tblEntityDotHazardousMessagesToSite]
(
    [InsertedRowVersion] ASC,
    [PK_DotHazardousMessagesToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDotHazardousMessagesToSite_UpdatedRowVersion] ON [track].[tblEntityDotHazardousMessagesToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_DotHazardousMessagesToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDotHazardousMessagesToSite_DeletedRowVersion] ON [track].[tblEntityDotHazardousMessagesToSite]
(
    [DeletedRowVersion] ASC,
    [PK_DotHazardousMessagesToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityDotHazardousMessagesToSite_PK_DotHazardousMessagesToSiteGuid_Sync] ON [track].[tblEntityDotHazardousMessagesToSite]
(
	[PK_DotHazardousMessagesToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityDotHazardousMessagesToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityDotHazardousMessagesToSite
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
        FROM track.tblEntityDotHazardousMessagesToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END