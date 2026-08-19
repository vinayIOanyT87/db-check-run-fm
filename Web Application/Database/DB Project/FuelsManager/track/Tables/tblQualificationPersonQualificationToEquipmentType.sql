/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationPersonQualificationToEquipmentType } */

/****** Object:  Table [track].[tblQualificationPersonQualificationToEquipmentType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationPersonQualificationToEquipmentType]
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
	[PK_QualificationPersonQualificationToEquipmentTypeGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationPersonQualificationToEquipmentType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToEquipmentType_PK_QualificationPersonQualificationToEquipmentTypeGuid] ON [track].[tblQualificationPersonQualificationToEquipmentType]
(
    [PK_QualificationPersonQualificationToEquipmentTypeGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToEquipmentType_InsertedRowVersion] ON [track].[tblQualificationPersonQualificationToEquipmentType]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationPersonQualificationToEquipmentTypeGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToEquipmentType_UpdatedRowVersion] ON [track].[tblQualificationPersonQualificationToEquipmentType]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationPersonQualificationToEquipmentTypeGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToEquipmentType_DeletedRowVersion] ON [track].[tblQualificationPersonQualificationToEquipmentType]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationPersonQualificationToEquipmentTypeGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationPersonQualificationToEquipmentType_PK_QualificationPersonQualificationToEquipmentTypeGuid_Sync] ON [track].[tblQualificationPersonQualificationToEquipmentType]
(
	[PK_QualificationPersonQualificationToEquipmentTypeGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationPersonQualificationToEquipmentType_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationPersonQualificationToEquipmentType
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
        FROM track.tblQualificationPersonQualificationToEquipmentType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END