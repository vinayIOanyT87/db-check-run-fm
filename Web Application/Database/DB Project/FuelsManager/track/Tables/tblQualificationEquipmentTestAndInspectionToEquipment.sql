/* {CheckPoint: CREATING TRACKING TABLE for tblQualificationEquipmentTestAndInspectionToEquipment } */

/****** Object:  Table [track].[tblQualificationEquipmentTestAndInspectionToEquipment]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQualificationEquipmentTestAndInspectionToEquipment]
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
	[PK_QualificationEquipmentTestAndInspectionToEquipmentGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQualificationEquipmentTestAndInspectionToEquipment_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTestAndInspectionToEquipment_PK_QualificationEquipmentTestAndInspectionToEquipmentGuid] ON [track].[tblQualificationEquipmentTestAndInspectionToEquipment]
(
    [PK_QualificationEquipmentTestAndInspectionToEquipmentGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTestAndInspectionToEquipment_InsertedRowVersion] ON [track].[tblQualificationEquipmentTestAndInspectionToEquipment]
(
    [InsertedRowVersion] ASC,
    [PK_QualificationEquipmentTestAndInspectionToEquipmentGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTestAndInspectionToEquipment_UpdatedRowVersion] ON [track].[tblQualificationEquipmentTestAndInspectionToEquipment]
(
    [UpdatedRowVersion] ASC,
    [PK_QualificationEquipmentTestAndInspectionToEquipmentGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTestAndInspectionToEquipment_DeletedRowVersion] ON [track].[tblQualificationEquipmentTestAndInspectionToEquipment]
(
    [DeletedRowVersion] ASC,
    [PK_QualificationEquipmentTestAndInspectionToEquipmentGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQualificationEquipmentTestAndInspectionToEquipment_PK_QualificationEquipmentTestAndInspectionToEquipmentGuid_Sync] ON [track].[tblQualificationEquipmentTestAndInspectionToEquipment]
(
	[PK_QualificationEquipmentTestAndInspectionToEquipmentGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQualificationEquipmentTestAndInspectionToEquipment_DeletedRowVersionUpdate_ForSync
   ON track.tblQualificationEquipmentTestAndInspectionToEquipment
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
        FROM track.tblQualificationEquipmentTestAndInspectionToEquipment t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END