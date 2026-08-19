/* {CheckPoint: CREATING TRACKING TABLE for tblAccessibilityConfigurationSettings } */

/****** Object:  Table [track].[tblAccessibilityConfigurationSettings]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAccessibilityConfigurationSettings]
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
    [PK_AccessibilityConfigurationSettingGuid] [UniqueIdentifier] NOT NULL,
    [FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
    CONSTRAINT [PK_track_tblAccessibilityConfigurationSettings_ChangeIndex] PRIMARY KEY CLUSTERED 
    (
        [ChangeIndex] ASC
    )
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilityConfigurationSettings_PK_AccessibilityConfigurationSettingGuid] ON [track].[tblAccessibilityConfigurationSettings]
(
    [PK_AccessibilityConfigurationSettingGuid],
	[ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilityConfigurationSettings_InsertedRowVersion] ON [track].[tblAccessibilityConfigurationSettings]
(
    [InsertedRowVersion] ASC,
    [PK_AccessibilityConfigurationSettingGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilityConfigurationSettings_UpdatedRowVersion] ON [track].[tblAccessibilityConfigurationSettings]
(
    [UpdatedRowVersion] ASC,
    [PK_AccessibilityConfigurationSettingGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilityConfigurationSettings_DeletedRowVersion] ON [track].[tblAccessibilityConfigurationSettings]
(
    [DeletedRowVersion] ASC,
    [PK_AccessibilityConfigurationSettingGuid],
    [DeletedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilityConfigurationSettings_PK_AccessibilityConfigurationSettingGuid_Sync] ON [track].[tblAccessibilityConfigurationSettings]
(
    [PK_AccessibilityConfigurationSettingGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO

CREATE TRIGGER track.trg_insupd_tblAccessibilityConfigurationSettings_DeletedRowVersionUpdate_ForSync
   ON track.tblAccessibilityConfigurationSettings
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
			FROM track.tblAccessibilityConfigurationSettings t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END