/* {CheckPoint: CREATING TRACKING TABLE for tblStandardFieldType } */

/****** Object:  Table [track].[tblStandardFieldType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblStandardFieldType]
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
	[PK_StandardFieldTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblStandardFieldType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStandardFieldType_PK_StandardFieldTypeIndex] ON [track].[tblStandardFieldType]
(
    [PK_StandardFieldTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStandardFieldType_InsertedRowVersion] ON [track].[tblStandardFieldType]
(
    [InsertedRowVersion] ASC,
    [PK_StandardFieldTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStandardFieldType_UpdatedRowVersion] ON [track].[tblStandardFieldType]
(
    [UpdatedRowVersion] ASC,
    [PK_StandardFieldTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblStandardFieldType_DeletedRowVersion] ON [track].[tblStandardFieldType]
(
    [DeletedRowVersion] ASC,
    [PK_StandardFieldTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblStandardFieldType_PK_StandardFieldTypeIndex_Sync] ON [track].[tblStandardFieldType]
(
	[PK_StandardFieldTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblStandardFieldType_DeletedRowVersionUpdate_ForSync
   ON track.tblStandardFieldType
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
        FROM track.tblStandardFieldType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END