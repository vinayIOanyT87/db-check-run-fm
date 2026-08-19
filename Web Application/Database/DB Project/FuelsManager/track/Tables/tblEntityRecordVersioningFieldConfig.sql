/* {CheckPoint: CREATING TRACKING TABLE for tblEntityRecordVersioningFieldConfig } */

/****** Object:  Table [track].[tblEntityRecordVersioningFieldConfig]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityRecordVersioningFieldConfig]
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
	[PK_FieldConfigGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityRecordVersioningFieldConfig_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityRecordVersioningFieldConfig_PK_FieldConfigGuid] ON [track].[tblEntityRecordVersioningFieldConfig]
(
    [PK_FieldConfigGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityRecordVersioningFieldConfig_InsertedRowVersion] ON [track].[tblEntityRecordVersioningFieldConfig]
(
    [InsertedRowVersion] ASC,
    [PK_FieldConfigGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityRecordVersioningFieldConfig_UpdatedRowVersion] ON [track].[tblEntityRecordVersioningFieldConfig]
(
    [UpdatedRowVersion] ASC,
    [PK_FieldConfigGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityRecordVersioningFieldConfig_DeletedRowVersion] ON [track].[tblEntityRecordVersioningFieldConfig]
(
    [DeletedRowVersion] ASC,
    [PK_FieldConfigGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityRecordVersioningFieldConfig_PK_FieldConfigGuid_Sync] ON [track].[tblEntityRecordVersioningFieldConfig]
(
	[PK_FieldConfigGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityRecordVersioningFieldConfig_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityRecordVersioningFieldConfig
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
        FROM track.tblEntityRecordVersioningFieldConfig t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END