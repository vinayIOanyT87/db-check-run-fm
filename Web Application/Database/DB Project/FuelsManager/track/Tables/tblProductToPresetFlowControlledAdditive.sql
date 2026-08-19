/* {CheckPoint: CREATING TRACKING TABLE for tblProductToPresetFlowControlledAdditive } */

/****** Object:  Table [track].[tblProductToPresetFlowControlledAdditive]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProductToPresetFlowControlledAdditive]
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
	[PK_ProductToPresetFlowControlledAdditiveGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToPresetFlowControlledAdditive_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetFlowControlledAdditive_PK_ProductToPresetFlowControlledAdditiveGuid] ON [track].[tblProductToPresetFlowControlledAdditive]
(
    [PK_ProductToPresetFlowControlledAdditiveGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetFlowControlledAdditive_InsertedRowVersion] ON [track].[tblProductToPresetFlowControlledAdditive]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToPresetFlowControlledAdditiveGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetFlowControlledAdditive_UpdatedRowVersion] ON [track].[tblProductToPresetFlowControlledAdditive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToPresetFlowControlledAdditiveGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetFlowControlledAdditive_DeletedRowVersion] ON [track].[tblProductToPresetFlowControlledAdditive]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToPresetFlowControlledAdditiveGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToPresetFlowControlledAdditive_PK_ProductToPresetFlowControlledAdditiveGuid_Sync] ON [track].[tblProductToPresetFlowControlledAdditive]
(
	[PK_ProductToPresetFlowControlledAdditiveGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToPresetFlowControlledAdditive_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToPresetFlowControlledAdditive
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
        FROM track.tblProductToPresetFlowControlledAdditive t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END