/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableRecipeInputPermissive } */

/****** Object:  Table [track].[tblProcessVariableRecipeInputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableRecipeInputPermissive]
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
	[PK_ProcessVariableProductToPresetRecipeGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableRecipeInputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableRecipeInputPermissive_PK_ProcessVariableProductToPresetRecipeGuid] ON [track].[tblProcessVariableRecipeInputPermissive]
(
    [PK_ProcessVariableProductToPresetRecipeGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableRecipeInputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableRecipeInputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetRecipeGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableRecipeInputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableRecipeInputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetRecipeGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableRecipeInputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableRecipeInputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetRecipeGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableRecipeInputPermissive_PK_ProcessVariableProductToPresetRecipeGuid_Sync] ON [track].[tblProcessVariableRecipeInputPermissive]
(
	[PK_ProcessVariableProductToPresetRecipeGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableRecipeInputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableRecipeInputPermissive
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
        FROM track.tblProcessVariableRecipeInputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END