/* {CheckPoint: CREATING TRACKING TABLE for tblWeightedAverageCosts } */

/****** Object:  Table [track].[tblWeightedAverageCosts]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblWeightedAverageCosts]
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
	[PK_WeightedAverageCostGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblWeightedAverageCosts_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWeightedAverageCosts_PK_WeightedAverageCostGuid] ON [track].[tblWeightedAverageCosts]
(
    [PK_WeightedAverageCostGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWeightedAverageCosts_InsertedRowVersion] ON [track].[tblWeightedAverageCosts]
(
    [InsertedRowVersion] ASC,
    [PK_WeightedAverageCostGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWeightedAverageCosts_UpdatedRowVersion] ON [track].[tblWeightedAverageCosts]
(
    [UpdatedRowVersion] ASC,
    [PK_WeightedAverageCostGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblWeightedAverageCosts_DeletedRowVersion] ON [track].[tblWeightedAverageCosts]
(
    [DeletedRowVersion] ASC,
    [PK_WeightedAverageCostGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblWeightedAverageCosts_PK_WeightedAverageCostGuid_Sync] ON [track].[tblWeightedAverageCosts]
(
	[PK_WeightedAverageCostGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblWeightedAverageCosts_DeletedRowVersionUpdate_ForSync
   ON track.tblWeightedAverageCosts
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
        FROM track.tblWeightedAverageCosts t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END