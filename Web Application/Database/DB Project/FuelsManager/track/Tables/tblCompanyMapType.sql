/* {CheckPoint: CREATING TRACKING TABLE for tblCompanyMapType } */

/****** Object:  Table [track].[tblCompanyMapType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCompanyMapType]
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
	[PK_CompanyMapTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCompanyMapType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyMapType_PK_CompanyMapTypeIndex] ON [track].[tblCompanyMapType]
(
    [PK_CompanyMapTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyMapType_InsertedRowVersion] ON [track].[tblCompanyMapType]
(
    [InsertedRowVersion] ASC,
    [PK_CompanyMapTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyMapType_UpdatedRowVersion] ON [track].[tblCompanyMapType]
(
    [UpdatedRowVersion] ASC,
    [PK_CompanyMapTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCompanyMapType_DeletedRowVersion] ON [track].[tblCompanyMapType]
(
    [DeletedRowVersion] ASC,
    [PK_CompanyMapTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCompanyMapType_PK_CompanyMapTypeIndex_Sync] ON [track].[tblCompanyMapType]
(
	[PK_CompanyMapTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCompanyMapType_DeletedRowVersionUpdate_ForSync
   ON track.tblCompanyMapType
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
        FROM track.tblCompanyMapType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END