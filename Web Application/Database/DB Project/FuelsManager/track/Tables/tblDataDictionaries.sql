/* {CheckPoint: CREATING TRACKING TABLE for tblDataDictionaries } */

/****** Object:  Table [track].[tblDataDictionaries]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblDataDictionaries]
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
	[PK_DataDictionaryGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblDataDictionaries_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDataDictionaries_PK_DataDictionaryGuid] ON [track].[tblDataDictionaries]
(
    [PK_DataDictionaryGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDataDictionaries_InsertedRowVersion] ON [track].[tblDataDictionaries]
(
    [InsertedRowVersion] ASC,
    [PK_DataDictionaryGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDataDictionaries_UpdatedRowVersion] ON [track].[tblDataDictionaries]
(
    [UpdatedRowVersion] ASC,
    [PK_DataDictionaryGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblDataDictionaries_DeletedRowVersion] ON [track].[tblDataDictionaries]
(
    [DeletedRowVersion] ASC,
    [PK_DataDictionaryGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblDataDictionaries_PK_DataDictionaryGuid_Sync] ON [track].[tblDataDictionaries]
(
	[PK_DataDictionaryGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblDataDictionaries_DeletedRowVersionUpdate_ForSync
   ON track.tblDataDictionaries
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
        FROM track.tblDataDictionaries t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END