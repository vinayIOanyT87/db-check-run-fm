/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableNoAdditiveInputPermissive } */

/****** Object:  Table [track].[tblProcessVariableNoAdditiveInputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableNoAdditiveInputPermissive]
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
	[PK_ProcessVariableLoadArmGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProcessVariableNoAdditiveInputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableNoAdditiveInputPermissive_PK_ProcessVariableLoadArmGuid] ON [track].[tblProcessVariableNoAdditiveInputPermissive]
(
    [PK_ProcessVariableLoadArmGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableNoAdditiveInputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableNoAdditiveInputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableLoadArmGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableNoAdditiveInputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableNoAdditiveInputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableLoadArmGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableNoAdditiveInputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableNoAdditiveInputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableLoadArmGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableNoAdditiveInputPermissive_PK_ProcessVariableLoadArmGuid_Sync] ON [track].[tblProcessVariableNoAdditiveInputPermissive]
(
	[PK_ProcessVariableLoadArmGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableNoAdditiveInputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableNoAdditiveInputPermissive
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
        FROM track.tblProcessVariableNoAdditiveInputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END