/* {CheckPoint: CREATING TRACKING TABLE for tblProductToCompanyGroup } */

/****** Object:  Table [track].[tblProductToCompanyGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProductToCompanyGroup]
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
	[PK_ProductToCompanyGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToCompanyGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToCompanyGroup_PK_ProductToCompanyGroupGuid] ON [track].[tblProductToCompanyGroup]
(
    [PK_ProductToCompanyGroupGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToCompanyGroup_InsertedRowVersion] ON [track].[tblProductToCompanyGroup]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToCompanyGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToCompanyGroup_UpdatedRowVersion] ON [track].[tblProductToCompanyGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToCompanyGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToCompanyGroup_DeletedRowVersion] ON [track].[tblProductToCompanyGroup]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToCompanyGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToCompanyGroup_PK_ProductToCompanyGroupGuid_Sync] ON [track].[tblProductToCompanyGroup]
(
	[PK_ProductToCompanyGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToCompanyGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToCompanyGroup
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
        FROM track.tblProductToCompanyGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END