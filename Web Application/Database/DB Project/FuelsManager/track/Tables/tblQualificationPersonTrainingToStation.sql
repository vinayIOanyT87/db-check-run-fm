/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonTrainingToStation } */

/****** Object:  Table [track].[tblQualificationPersonTrainingToStation]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationPersonTrainingToStation]
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
	[PK_QualificationPersonTrainingToStationGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationPersonTrainingToStation_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToStation_PK_QualificationPersonTrainingToStationGuid] ON [track].[tblQualificationPersonTrainingToStation]
(
    [PK_QualificationPersonTrainingToStationGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToStation_InsertedRowVersion] ON [track].[tblQualificationPersonTrainingToStation]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonTrainingToStationGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToStation_UpdatedRowVersion] ON [track].[tblQualificationPersonTrainingToStation]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonTrainingToStationGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToStation_DeletedRowVersion] ON [track].[tblQualificationPersonTrainingToStation]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonTrainingToStationGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToStation_PK_QualificationPersonTrainingToStationGuid_Sync] ON [track].[tblQualificationPersonTrainingToStation]
(
	[PK_QualificationPersonTrainingToStationGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationPersonTrainingToStation_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationPersonTrainingToStation
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
        FROM track.tblQualificationPersonTrainingToStation t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END