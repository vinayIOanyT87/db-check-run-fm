/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableComponentOutputPermissive } */

/****** Object:  Table [track].[tblProcessVariableComponentOutputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableComponentOutputPermissive]
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
	[PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableComponentOutputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableComponentOutputPermissive_PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid] ON [track].[tblProcessVariableComponentOutputPermissive]
(
    [PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableComponentOutputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableComponentOutputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableComponentOutputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableComponentOutputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableComponentOutputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableComponentOutputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableComponentOutputPermissive_PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid_Sync] ON [track].[tblProcessVariableComponentOutputPermissive]
(
	[PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableComponentOutputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableComponentOutputPermissive
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
        FROM track.tblProcessVariableComponentOutputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END