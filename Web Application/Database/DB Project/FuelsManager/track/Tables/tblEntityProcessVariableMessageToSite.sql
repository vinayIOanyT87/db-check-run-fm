/* {CheckPoint: CREATING TRACKING TABLE for tblEntityProcessVariableMessageToSite } */

/****** Object:  Table [track].[tblEntityProcessVariableMessageToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityProcessVariableMessageToSite]
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
	[PK_ProcessVariableMessageToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityProcessVariableMessageToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityProcessVariableMessageToSite_PK_ProcessVariableMessageToSiteGuid] ON [track].[tblEntityProcessVariableMessageToSite]
(
    [PK_ProcessVariableMessageToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityProcessVariableMessageToSite_InsertedRowVersion] ON [track].[tblEntityProcessVariableMessageToSite]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableMessageToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityProcessVariableMessageToSite_UpdatedRowVersion] ON [track].[tblEntityProcessVariableMessageToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableMessageToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityProcessVariableMessageToSite_DeletedRowVersion] ON [track].[tblEntityProcessVariableMessageToSite]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableMessageToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityProcessVariableMessageToSite_PK_ProcessVariableMessageToSiteGuid_Sync] ON [track].[tblEntityProcessVariableMessageToSite]
(
	[PK_ProcessVariableMessageToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityProcessVariableMessageToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityProcessVariableMessageToSite
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
        FROM track.tblEntityProcessVariableMessageToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END