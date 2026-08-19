/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableExternalComponentInputPermissive } */

/****** Object:  Table [track].[tblProcessVariableExternalComponentInputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableExternalComponentInputPermissive]
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
	[PK_ProcessVariableProductToPresetExternalComponentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableExternalComponentInputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentInputPermissive_PK_ProcessVariableProductToPresetExternalComponentGuid] ON [track].[tblProcessVariableExternalComponentInputPermissive]
(
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentInputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableExternalComponentInputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentInputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableExternalComponentInputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentInputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableExternalComponentInputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentInputPermissive_PK_ProcessVariableProductToPresetExternalComponentGuid_Sync] ON [track].[tblProcessVariableExternalComponentInputPermissive]
(
	[PK_ProcessVariableProductToPresetExternalComponentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableExternalComponentInputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableExternalComponentInputPermissive
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
        FROM track.tblProcessVariableExternalComponentInputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END