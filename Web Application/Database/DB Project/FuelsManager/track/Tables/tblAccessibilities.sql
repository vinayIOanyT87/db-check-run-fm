/* {CheckPoint: CREATING TRACKING TABLE for tblAccessibilities } */

/****** Object:  Table [track].[tblAccessibilities]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAccessibilities]
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
    [PK_AccessibilityGuid] [UniqueIdentifier] NOT NULL,
    [FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
    CONSTRAINT [PK_track_tblAccessibilities_ChangeIndex] PRIMARY KEY CLUSTERED 
    (
        [ChangeIndex] ASC
    )
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilities_PK_AccessibilityGuid] ON [track].[tblAccessibilities]
(
    [PK_AccessibilityGuid],
	[ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilities_InsertedRowVersion] ON [track].[tblAccessibilities]
(
    [InsertedRowVersion] ASC,
    [PK_AccessibilityGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilities_UpdatedRowVersion] ON [track].[tblAccessibilities]
(
    [UpdatedRowVersion] ASC,
    [PK_AccessibilityGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilities_DeletedRowVersion] ON [track].[tblAccessibilities]
(
    [DeletedRowVersion] ASC,
    [PK_AccessibilityGuid],
    [DeletedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAccessibilities_PK_AccessibilityGuid_Sync] ON [track].[tblAccessibilities]
(
    [PK_AccessibilityGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO

CREATE TRIGGER track.trg_insupd_tblAccessibilities_DeletedRowVersionUpdate_ForSync
   ON track.tblAccessibilities
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
			FROM track.tblAccessibilities t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END