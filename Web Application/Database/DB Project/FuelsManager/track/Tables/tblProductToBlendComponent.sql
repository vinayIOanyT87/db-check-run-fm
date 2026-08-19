/* {CheckPoint: CREATING TRACKING TABLE for tblProductToBlendComponent } */

/****** Object:  Table [track].[tblProductToBlendComponent]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProductToBlendComponent]
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
	[PK_ProductToBlendComponentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToBlendComponent_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToBlendComponent_PK_ProductToBlendComponentGuid] ON [track].[tblProductToBlendComponent]
(
    [PK_ProductToBlendComponentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToBlendComponent_InsertedRowVersion] ON [track].[tblProductToBlendComponent]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToBlendComponentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToBlendComponent_UpdatedRowVersion] ON [track].[tblProductToBlendComponent]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToBlendComponentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToBlendComponent_DeletedRowVersion] ON [track].[tblProductToBlendComponent]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToBlendComponentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToBlendComponent_PK_ProductToBlendComponentGuid_Sync] ON [track].[tblProductToBlendComponent]
(
	[PK_ProductToBlendComponentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToBlendComponent_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToBlendComponent
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
        FROM track.tblProductToBlendComponent t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END