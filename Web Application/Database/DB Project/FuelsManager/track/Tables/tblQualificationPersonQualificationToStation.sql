/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonQualificationToStation } */

/****** Object:  Table [track].[tblQualificationPersonQualificationToStation]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationPersonQualificationToStation]
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
	[PK_QualificationPersonQualificationToStationGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationPersonQualificationToStation_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToStation_PK_QualificationPersonQualificationToStationGuid] ON [track].[tblQualificationPersonQualificationToStation]
(
    [PK_QualificationPersonQualificationToStationGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToStation_InsertedRowVersion] ON [track].[tblQualificationPersonQualificationToStation]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonQualificationToStationGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToStation_UpdatedRowVersion] ON [track].[tblQualificationPersonQualificationToStation]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonQualificationToStationGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToStation_DeletedRowVersion] ON [track].[tblQualificationPersonQualificationToStation]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonQualificationToStationGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToStation_PK_QualificationPersonQualificationToStationGuid_Sync] ON [track].[tblQualificationPersonQualificationToStation]
(
	[PK_QualificationPersonQualificationToStationGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationPersonQualificationToStation_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationPersonQualificationToStation
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
        FROM track.tblQualificationPersonQualificationToStation t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END