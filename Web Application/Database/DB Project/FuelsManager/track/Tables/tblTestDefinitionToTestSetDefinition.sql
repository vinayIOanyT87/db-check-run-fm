/* {CheckPoint: CREATING TRACKING TABLE for tblTestDefinitionToTestSetDefinition } */

/****** Object:  Table [track].[tblTestDefinitionToTestSetDefinition]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblTestDefinitionToTestSetDefinition]
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
	[PK_TestDefinitionToTestSetDefinitionGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblTestDefinitionToTestSetDefinition_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestDefinitionToTestSetDefinition_PK_TestDefinitionToTestSetDefinitionGuid] ON [track].[tblTestDefinitionToTestSetDefinition]
(
    [PK_TestDefinitionToTestSetDefinitionGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestDefinitionToTestSetDefinition_InsertedRowVersion] ON [track].[tblTestDefinitionToTestSetDefinition]
(
    [InsertedRowVersion] ASC,
    [PK_TestDefinitionToTestSetDefinitionGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestDefinitionToTestSetDefinition_UpdatedRowVersion] ON [track].[tblTestDefinitionToTestSetDefinition]
(
    [UpdatedRowVersion] ASC,
    [PK_TestDefinitionToTestSetDefinitionGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblTestDefinitionToTestSetDefinition_DeletedRowVersion] ON [track].[tblTestDefinitionToTestSetDefinition]
(
    [DeletedRowVersion] ASC,
    [PK_TestDefinitionToTestSetDefinitionGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblTestDefinitionToTestSetDefinition_PK_TestDefinitionToTestSetDefinitionGuid_Sync] ON [track].[tblTestDefinitionToTestSetDefinition]
(
	[PK_TestDefinitionToTestSetDefinitionGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblTestDefinitionToTestSetDefinition_DeletedRowVersionUpdate_ForSync
   ON track.tblTestDefinitionToTestSetDefinition
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
        FROM track.tblTestDefinitionToTestSetDefinition t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END