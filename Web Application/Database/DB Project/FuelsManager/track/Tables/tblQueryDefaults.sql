/* {CheckPoint: CREATING TRACKING TABLE for tblQueryDefaults } */

/****** Object:  Table [track].[tblQueryDefaults]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQueryDefaults]
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
	[PK_QueryDefaultGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQueryDefaults_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQueryDefaults_PK_QueryDefaultGuid] ON [track].[tblQueryDefaults]
(
    [PK_QueryDefaultGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQueryDefaults_InsertedRowVersion] ON [track].[tblQueryDefaults]
(
    [InsertedRowVersion] ASC,
    [PK_QueryDefaultGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQueryDefaults_UpdatedRowVersion] ON [track].[tblQueryDefaults]
(
    [UpdatedRowVersion] ASC,
    [PK_QueryDefaultGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQueryDefaults_DeletedRowVersion] ON [track].[tblQueryDefaults]
(
    [DeletedRowVersion] ASC,
    [PK_QueryDefaultGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQueryDefaults_PK_QueryDefaultGuid_Sync] ON [track].[tblQueryDefaults]
(
	[PK_QueryDefaultGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQueryDefaults_DeletedRowVersionUpdate_ForSync
   ON track.tblQueryDefaults
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
        FROM track.tblQueryDefaults t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END