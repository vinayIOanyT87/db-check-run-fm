/* {CheckPoint: CREATING TRACKING TABLE for tblProductToPresetExternalComponent } */

/****** Object:  Table [track].[tblProductToPresetExternalComponent]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProductToPresetExternalComponent]
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
	[PK_ProductToPresetExternalComponentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToPresetExternalComponent_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetExternalComponent_PK_ProductToPresetExternalComponentGuid] ON [track].[tblProductToPresetExternalComponent]
(
    [PK_ProductToPresetExternalComponentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetExternalComponent_InsertedRowVersion] ON [track].[tblProductToPresetExternalComponent]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToPresetExternalComponentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetExternalComponent_UpdatedRowVersion] ON [track].[tblProductToPresetExternalComponent]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToPresetExternalComponentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetExternalComponent_DeletedRowVersion] ON [track].[tblProductToPresetExternalComponent]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToPresetExternalComponentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetExternalComponent_PK_ProductToPresetExternalComponentGuid_Sync] ON [track].[tblProductToPresetExternalComponent]
(
	[PK_ProductToPresetExternalComponentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToPresetExternalComponent_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToPresetExternalComponent
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
        FROM track.tblProductToPresetExternalComponent t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END