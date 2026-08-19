/* {CheckPoint: CREATING TRACKING TABLE for tblGeneralConfiguration } */

/****** Object:  Table [track].[tblGeneralConfiguration]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblGeneralConfiguration]
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
	[PK_GeneralConfigurationGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblGeneralConfiguration_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfiguration_PK_GeneralConfigurationGuid] ON [track].[tblGeneralConfiguration]
(
    [PK_GeneralConfigurationGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfiguration_InsertedRowVersion] ON [track].[tblGeneralConfiguration]
(
    [InsertedRowVersion] ASC,
    [PK_GeneralConfigurationGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfiguration_UpdatedRowVersion] ON [track].[tblGeneralConfiguration]
(
    [UpdatedRowVersion] ASC,
    [PK_GeneralConfigurationGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfiguration_DeletedRowVersion] ON [track].[tblGeneralConfiguration]
(
    [DeletedRowVersion] ASC,
    [PK_GeneralConfigurationGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGeneralConfiguration_PK_GeneralConfigurationGuid_Sync] ON [track].[tblGeneralConfiguration]
(
	[PK_GeneralConfigurationGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblGeneralConfiguration_DeletedRowVersionUpdate_ForSync
   ON track.tblGeneralConfiguration
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
        FROM track.tblGeneralConfiguration t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END