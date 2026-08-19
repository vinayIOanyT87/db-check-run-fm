/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableAdditiveOutputPermissive } */

/****** Object:  Table [track].[tblProcessVariableAdditiveOutputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableAdditiveOutputPermissive]
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
	[PK_ProcessVariableProductToPresetInjectorGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableAdditiveOutputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableAdditiveOutputPermissive_PK_ProcessVariableProductToPresetInjectorGuid] ON [track].[tblProcessVariableAdditiveOutputPermissive]
(
    [PK_ProcessVariableProductToPresetInjectorGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableAdditiveOutputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableAdditiveOutputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetInjectorGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableAdditiveOutputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableAdditiveOutputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetInjectorGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableAdditiveOutputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableAdditiveOutputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetInjectorGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableAdditiveOutputPermissive_PK_ProcessVariableProductToPresetInjectorGuid_Sync] ON [track].[tblProcessVariableAdditiveOutputPermissive]
(
	[PK_ProcessVariableProductToPresetInjectorGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableAdditiveOutputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableAdditiveOutputPermissive
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
        FROM track.tblProcessVariableAdditiveOutputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END