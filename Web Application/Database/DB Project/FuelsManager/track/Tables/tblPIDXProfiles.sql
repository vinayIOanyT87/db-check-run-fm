/* {CheckPoint: CREATING TRACKING TABLE for tblPIDXProfiles } */

/****** Object:  Table [track].[tblPIDXProfiles]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPIDXProfiles]
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
	[PK_PIDXProfileGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPIDXProfiles_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfiles_PK_PIDXProfileGuid] ON [track].[tblPIDXProfiles]
(
    [PK_PIDXProfileGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfiles_InsertedRowVersion] ON [track].[tblPIDXProfiles]
(
    [InsertedRowVersion] ASC,
    [PK_PIDXProfileGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfiles_UpdatedRowVersion] ON [track].[tblPIDXProfiles]
(
    [UpdatedRowVersion] ASC,
    [PK_PIDXProfileGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfiles_DeletedRowVersion] ON [track].[tblPIDXProfiles]
(
    [DeletedRowVersion] ASC,
    [PK_PIDXProfileGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfiles_PK_PIDXProfileGuid_Sync] ON [track].[tblPIDXProfiles]
(
	[PK_PIDXProfileGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPIDXProfiles_DeletedRowVersionUpdate_ForSync
   ON track.tblPIDXProfiles
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
        FROM track.tblPIDXProfiles t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END