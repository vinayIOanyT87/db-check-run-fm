/* {CheckPoint: CREATING TRACKING TABLE for tblMinorCorrectionType } */

/****** Object:  Table [track].[tblMinorCorrectionType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMinorCorrectionType]
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
	[PK_MinorCorrectionTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMinorCorrectionType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMinorCorrectionType_PK_MinorCorrectionTypeIndex] ON [track].[tblMinorCorrectionType]
(
    [PK_MinorCorrectionTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMinorCorrectionType_InsertedRowVersion] ON [track].[tblMinorCorrectionType]
(
    [InsertedRowVersion] ASC,
    [PK_MinorCorrectionTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMinorCorrectionType_UpdatedRowVersion] ON [track].[tblMinorCorrectionType]
(
    [UpdatedRowVersion] ASC,
    [PK_MinorCorrectionTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMinorCorrectionType_DeletedRowVersion] ON [track].[tblMinorCorrectionType]
(
    [DeletedRowVersion] ASC,
    [PK_MinorCorrectionTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMinorCorrectionType_PK_MinorCorrectionTypeIndex_Sync] ON [track].[tblMinorCorrectionType]
(
	[PK_MinorCorrectionTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMinorCorrectionType_DeletedRowVersionUpdate_ForSync
   ON track.tblMinorCorrectionType
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
        FROM track.tblMinorCorrectionType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END