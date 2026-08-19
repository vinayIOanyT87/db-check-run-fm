/* {CheckPoint: CREATING TRACKING TABLE for tblCustomToolbarType } */

/****** Object:  Table [track].[tblCustomToolbarType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblCustomToolbarType]
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
	[PK_CustomToolbarTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblCustomToolbarType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbarType_PK_CustomToolbarTypeIndex] ON [track].[tblCustomToolbarType]
(
    [PK_CustomToolbarTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbarType_InsertedRowVersion] ON [track].[tblCustomToolbarType]
(
    [InsertedRowVersion] ASC,
    [PK_CustomToolbarTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbarType_UpdatedRowVersion] ON [track].[tblCustomToolbarType]
(
    [UpdatedRowVersion] ASC,
    [PK_CustomToolbarTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbarType_DeletedRowVersion] ON [track].[tblCustomToolbarType]
(
    [DeletedRowVersion] ASC,
    [PK_CustomToolbarTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblCustomToolbarType_PK_CustomToolbarTypeIndex_Sync] ON [track].[tblCustomToolbarType]
(
	[PK_CustomToolbarTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblCustomToolbarType_DeletedRowVersionUpdate_ForSync
   ON track.tblCustomToolbarType
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
        FROM track.tblCustomToolbarType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END