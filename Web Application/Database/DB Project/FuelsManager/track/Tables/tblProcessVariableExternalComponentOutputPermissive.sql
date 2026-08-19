/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableExternalComponentOutputPermissive } */

/****** Object:  Table [track].[tblProcessVariableExternalComponentOutputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableExternalComponentOutputPermissive]
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
	CONSTRAINT [PK_track_tblProcessVariableExternalComponentOutputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentOutputPermissive_PK_ProcessVariableProductToPresetExternalComponentGuid] ON [track].[tblProcessVariableExternalComponentOutputPermissive]
(
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentOutputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableExternalComponentOutputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentOutputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableExternalComponentOutputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentOutputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableExternalComponentOutputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetExternalComponentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableExternalComponentOutputPermissive_PK_ProcessVariableProductToPresetExternalComponentGuid_Sync] ON [track].[tblProcessVariableExternalComponentOutputPermissive]
(
	[PK_ProcessVariableProductToPresetExternalComponentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableExternalComponentOutputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableExternalComponentOutputPermissive
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
        FROM track.tblProcessVariableExternalComponentOutputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END