/* {CheckPoint: CREATING TRACKING TABLE for tblMessageFrequencyType } */

/****** Object:  Table [track].[tblMessageFrequencyType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMessageFrequencyType]
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
	[PK_MessageFrequencyTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMessageFrequencyType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageFrequencyType_PK_MessageFrequencyTypeIndex] ON [track].[tblMessageFrequencyType]
(
    [PK_MessageFrequencyTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageFrequencyType_InsertedRowVersion] ON [track].[tblMessageFrequencyType]
(
    [InsertedRowVersion] ASC,
    [PK_MessageFrequencyTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageFrequencyType_UpdatedRowVersion] ON [track].[tblMessageFrequencyType]
(
    [UpdatedRowVersion] ASC,
    [PK_MessageFrequencyTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageFrequencyType_DeletedRowVersion] ON [track].[tblMessageFrequencyType]
(
    [DeletedRowVersion] ASC,
    [PK_MessageFrequencyTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMessageFrequencyType_PK_MessageFrequencyTypeIndex_Sync] ON [track].[tblMessageFrequencyType]
(
	[PK_MessageFrequencyTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMessageFrequencyType_DeletedRowVersionUpdate_ForSync
   ON track.tblMessageFrequencyType
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
        FROM track.tblMessageFrequencyType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END