/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonQualificationToPerson } */

/****** Object:  Table [track].[tblQualificationPersonQualificationToPerson]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationPersonQualificationToPerson]
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
	[PK_QualificationPersonQualificationToPersonGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationPersonQualificationToPerson_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToPerson_PK_QualificationPersonQualificationToPersonGuid] ON [track].[tblQualificationPersonQualificationToPerson]
(
    [PK_QualificationPersonQualificationToPersonGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToPerson_InsertedRowVersion] ON [track].[tblQualificationPersonQualificationToPerson]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonQualificationToPersonGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToPerson_UpdatedRowVersion] ON [track].[tblQualificationPersonQualificationToPerson]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonQualificationToPersonGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToPerson_DeletedRowVersion] ON [track].[tblQualificationPersonQualificationToPerson]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonQualificationToPersonGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToPerson_PK_QualificationPersonQualificationToPersonGuid_Sync] ON [track].[tblQualificationPersonQualificationToPerson]
(
	[PK_QualificationPersonQualificationToPersonGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationPersonQualificationToPerson_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationPersonQualificationToPerson
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
        FROM track.tblQualificationPersonQualificationToPerson t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END