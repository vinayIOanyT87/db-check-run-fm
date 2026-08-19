/* {CheckPoint: CREATING TRACKING TABLE for tblPersonnelRole } */

/****** Object:  Table [track].[tblPersonnelRole]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPersonnelRole]
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
	[PK_PersonnelRoleIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPersonnelRole_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelRole_PK_PersonnelRoleIndex] ON [track].[tblPersonnelRole]
(
    [PK_PersonnelRoleIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelRole_InsertedRowVersion] ON [track].[tblPersonnelRole]
(
    [InsertedRowVersion] ASC,
    [PK_PersonnelRoleIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelRole_UpdatedRowVersion] ON [track].[tblPersonnelRole]
(
    [UpdatedRowVersion] ASC,
    [PK_PersonnelRoleIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelRole_DeletedRowVersion] ON [track].[tblPersonnelRole]
(
    [DeletedRowVersion] ASC,
    [PK_PersonnelRoleIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPersonnelRole_PK_PersonnelRoleIndex_Sync] ON [track].[tblPersonnelRole]
(
	[PK_PersonnelRoleIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPersonnelRole_DeletedRowVersionUpdate_ForSync
   ON track.tblPersonnelRole
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
        FROM track.tblPersonnelRole t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END