/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataListValuePersonnel } */

/****** Object:  Table [track].[tblUserDataListValuePersonnel]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataListValuePersonnel]
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
	[PK_UserDataListValuePersonnelGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataListValuePersonnel_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValuePersonnel_PK_UserDataListValuePersonnelGuid] ON [track].[tblUserDataListValuePersonnel]
(
    [PK_UserDataListValuePersonnelGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValuePersonnel_InsertedRowVersion] ON [track].[tblUserDataListValuePersonnel]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataListValuePersonnelGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValuePersonnel_UpdatedRowVersion] ON [track].[tblUserDataListValuePersonnel]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataListValuePersonnelGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValuePersonnel_DeletedRowVersion] ON [track].[tblUserDataListValuePersonnel]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataListValuePersonnelGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValuePersonnel_PK_UserDataListValuePersonnelGuid_Sync] ON [track].[tblUserDataListValuePersonnel]
(
	[PK_UserDataListValuePersonnelGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataListValuePersonnel_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataListValuePersonnel
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
        FROM track.tblUserDataListValuePersonnel t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END