/* {CheckPoint: CREATING TRACKING TABLE for tblEntityDispatchConfigurationToSite } */

/****** Object:  Table [track].[tblEntityDispatchConfigurationToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityDispatchConfigurationToSite]
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
	[PK_DispatchConfigurationToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityDispatchConfigurationToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDispatchConfigurationToSite_PK_DispatchConfigurationToSiteGuid] ON [track].[tblEntityDispatchConfigurationToSite]
(
    [PK_DispatchConfigurationToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDispatchConfigurationToSite_InsertedRowVersion] ON [track].[tblEntityDispatchConfigurationToSite]
(
    [InsertedRowVersion] ASC,
    [PK_DispatchConfigurationToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDispatchConfigurationToSite_UpdatedRowVersion] ON [track].[tblEntityDispatchConfigurationToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_DispatchConfigurationToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDispatchConfigurationToSite_DeletedRowVersion] ON [track].[tblEntityDispatchConfigurationToSite]
(
    [DeletedRowVersion] ASC,
    [PK_DispatchConfigurationToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityDispatchConfigurationToSite_PK_DispatchConfigurationToSiteGuid_Sync] ON [track].[tblEntityDispatchConfigurationToSite]
(
	[PK_DispatchConfigurationToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityDispatchConfigurationToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityDispatchConfigurationToSite
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
        FROM track.tblEntityDispatchConfigurationToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END