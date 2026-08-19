/* {CheckPoint: CREATING TRACKING TABLE for tblGeneralConfigurationAliases } */

/****** Object:  Table [track].[tblGeneralConfigurationAliases]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblGeneralConfigurationAliases]
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
	[PK_GeneralConfigurationAliasGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblGeneralConfigurationAliases_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfigurationAliases_PK_GeneralConfigurationAliasGuid] ON [track].[tblGeneralConfigurationAliases]
(
    [PK_GeneralConfigurationAliasGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfigurationAliases_InsertedRowVersion] ON [track].[tblGeneralConfigurationAliases]
(
    [InsertedRowVersion] ASC,
    [PK_GeneralConfigurationAliasGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfigurationAliases_UpdatedRowVersion] ON [track].[tblGeneralConfigurationAliases]
(
    [UpdatedRowVersion] ASC,
    [PK_GeneralConfigurationAliasGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfigurationAliases_DeletedRowVersion] ON [track].[tblGeneralConfigurationAliases]
(
    [DeletedRowVersion] ASC,
    [PK_GeneralConfigurationAliasGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfigurationAliases_PK_GeneralConfigurationAliasGuid_Sync] ON [track].[tblGeneralConfigurationAliases]
(
	[PK_GeneralConfigurationAliasGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblGeneralConfigurationAliases_DeletedRowVersionUpdate_ForSync
   ON track.tblGeneralConfigurationAliases
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
        FROM track.tblGeneralConfigurationAliases t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END