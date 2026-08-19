/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariablePresetInjector } */

/****** Object:  Table [track].[tblProcessVariablePresetInjector]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariablePresetInjector]
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
	[PK_ProcessVariablePresetInjectorGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariablePresetInjector_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariablePresetInjector_PK_ProcessVariablePresetInjectorGuid] ON [track].[tblProcessVariablePresetInjector]
(
    [PK_ProcessVariablePresetInjectorGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariablePresetInjector_InsertedRowVersion] ON [track].[tblProcessVariablePresetInjector]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariablePresetInjectorGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariablePresetInjector_UpdatedRowVersion] ON [track].[tblProcessVariablePresetInjector]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariablePresetInjectorGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariablePresetInjector_DeletedRowVersion] ON [track].[tblProcessVariablePresetInjector]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariablePresetInjectorGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariablePresetInjector_PK_ProcessVariablePresetInjectorGuid_Sync] ON [track].[tblProcessVariablePresetInjector]
(
	[PK_ProcessVariablePresetInjectorGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariablePresetInjector_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariablePresetInjector
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
        FROM track.tblProcessVariablePresetInjector t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END