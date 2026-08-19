/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonTrainingToPerson } */

/****** Object:  Table [track].[tblQualificationPersonTrainingToPerson]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationPersonTrainingToPerson]
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
	[PK_QualificationPersonTrainingToPersonGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationPersonTrainingToPerson_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToPerson_PK_QualificationPersonTrainingToPersonGuid] ON [track].[tblQualificationPersonTrainingToPerson]
(
    [PK_QualificationPersonTrainingToPersonGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToPerson_InsertedRowVersion] ON [track].[tblQualificationPersonTrainingToPerson]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonTrainingToPersonGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToPerson_UpdatedRowVersion] ON [track].[tblQualificationPersonTrainingToPerson]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonTrainingToPersonGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToPerson_DeletedRowVersion] ON [track].[tblQualificationPersonTrainingToPerson]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonTrainingToPersonGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonTrainingToPerson_PK_QualificationPersonTrainingToPersonGuid_Sync] ON [track].[tblQualificationPersonTrainingToPerson]
(
	[PK_QualificationPersonTrainingToPersonGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationPersonTrainingToPerson_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationPersonTrainingToPerson
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
        FROM track.tblQualificationPersonTrainingToPerson t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END