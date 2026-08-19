/* {CheckPoint: CREATING TRACKING TABLE for tblMajorCorrectionType } */

/****** Object:  Table [track].[tblMajorCorrectionType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMajorCorrectionType]
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
	[PK_MajorCorrectionTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMajorCorrectionType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMajorCorrectionType_PK_MajorCorrectionTypeIndex] ON [track].[tblMajorCorrectionType]
(
    [PK_MajorCorrectionTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMajorCorrectionType_InsertedRowVersion] ON [track].[tblMajorCorrectionType]
(
    [InsertedRowVersion] ASC,
    [PK_MajorCorrectionTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMajorCorrectionType_UpdatedRowVersion] ON [track].[tblMajorCorrectionType]
(
    [UpdatedRowVersion] ASC,
    [PK_MajorCorrectionTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMajorCorrectionType_DeletedRowVersion] ON [track].[tblMajorCorrectionType]
(
    [DeletedRowVersion] ASC,
    [PK_MajorCorrectionTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMajorCorrectionType_PK_MajorCorrectionTypeIndex_Sync] ON [track].[tblMajorCorrectionType]
(
	[PK_MajorCorrectionTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMajorCorrectionType_DeletedRowVersionUpdate_ForSync
   ON track.tblMajorCorrectionType
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
        FROM track.tblMajorCorrectionType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END