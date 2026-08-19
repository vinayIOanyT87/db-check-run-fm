CREATE TABLE [track].[tblConfigurationSetting]
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
	[PK_ConfigurationSettingGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblConfigurationSetting] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblConfigurationSetting_PK_ConfigurationSettingGuid] ON [track].[tblConfigurationSetting]
(
    [PK_ConfigurationSettingGuid]
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblConfigurationSetting_InsertedRowVersion] ON [track].[tblConfigurationSetting]
(
    [InsertedRowVersion] ASC,
    [PK_ConfigurationSettingGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblConfigurationSetting_UpdatedRowVersion] ON [track].[tblConfigurationSetting]
(
    [UpdatedRowVersion] ASC,
    [PK_ConfigurationSettingGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblConfigurationSetting_DeletedRowVersion] ON [track].[tblConfigurationSetting]
(
    [DeletedRowVersion] ASC,
    [PK_ConfigurationSettingGuid],
    [DeletedContext] 
)
GO
CREATE TRIGGER track.trg_insupd_tblConfigurationSetting_DeletedRowVersionUpdate_ForSync 
   ON track.tblConfigurationSetting
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
        FROM track.tblConfigurationSetting t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END  
END
