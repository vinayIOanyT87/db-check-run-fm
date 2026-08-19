/* {CheckPoint: CREATING TRACKING TABLE for tblPersonnelToRole } */

/****** Object:  Table [track].[tblPersonnelToRole]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPersonnelToRole]
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
	[PK_PersonnelToRoleGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPersonnelToRole_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelToRole_PK_PersonnelToRoleGuid] ON [track].[tblPersonnelToRole]
(
    [PK_PersonnelToRoleGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelToRole_InsertedRowVersion] ON [track].[tblPersonnelToRole]
(
    [InsertedRowVersion] ASC,
    [PK_PersonnelToRoleGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelToRole_UpdatedRowVersion] ON [track].[tblPersonnelToRole]
(
    [UpdatedRowVersion] ASC,
    [PK_PersonnelToRoleGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelToRole_DeletedRowVersion] ON [track].[tblPersonnelToRole]
(
    [DeletedRowVersion] ASC,
    [PK_PersonnelToRoleGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelToRole_PK_PersonnelToRoleGuid_Sync] ON [track].[tblPersonnelToRole]
(
	[PK_PersonnelToRoleGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPersonnelToRole_DeletedRowVersionUpdate_ForSync
   ON track.tblPersonnelToRole
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
        FROM track.tblPersonnelToRole t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END