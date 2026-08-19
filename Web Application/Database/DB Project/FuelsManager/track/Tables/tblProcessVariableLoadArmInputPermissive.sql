/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableLoadArmInputPermissive } */

/****** Object:  Table [track].[tblProcessVariableLoadArmInputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProcessVariableLoadArmInputPermissive]
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
	CONSTRAINT [PK_track_tblProcessVariableLoadArmInputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableLoadArmInputPermissive_PK_ProcessVariableLoadArmGuid] ON [track].[tblProcessVariableLoadArmInputPermissive]
(
    [PK_ProcessVariableLoadArmGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableLoadArmInputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableLoadArmInputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableLoadArmGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableLoadArmInputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableLoadArmInputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableLoadArmGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableLoadArmInputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableLoadArmInputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableLoadArmGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableLoadArmInputPermissive_PK_ProcessVariableLoadArmGuid_Sync] ON [track].[tblProcessVariableLoadArmInputPermissive]
(
	[PK_ProcessVariableLoadArmGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProcessVariableLoadArmInputPermissive_DeletedRowVersionUpdate_ForSync
   ON track.tblProcessVariableLoadArmInputPermissive
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
        FROM track.tblProcessVariableLoadArmInputPermissive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END