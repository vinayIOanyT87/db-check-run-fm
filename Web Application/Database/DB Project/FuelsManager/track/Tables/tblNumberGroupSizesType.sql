/* {CheckPoint: CREATING TRACKING TABLE for tblNumberGroupSizesType } */

/****** Object:  Table [track].[tblNumberGroupSizesType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblNumberGroupSizesType]
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
	[PK_NumberGroupSizesTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblNumberGroupSizesType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblNumberGroupSizesType_PK_NumberGroupSizesTypeIndex] ON [track].[tblNumberGroupSizesType]
(
    [PK_NumberGroupSizesTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblNumberGroupSizesType_InsertedRowVersion] ON [track].[tblNumberGroupSizesType]
(
    [InsertedRowVersion] ASC,
    [PK_NumberGroupSizesTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblNumberGroupSizesType_UpdatedRowVersion] ON [track].[tblNumberGroupSizesType]
(
    [UpdatedRowVersion] ASC,
    [PK_NumberGroupSizesTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblNumberGroupSizesType_DeletedRowVersion] ON [track].[tblNumberGroupSizesType]
(
    [DeletedRowVersion] ASC,
    [PK_NumberGroupSizesTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblNumberGroupSizesType_PK_NumberGroupSizesTypeIndex_Sync] ON [track].[tblNumberGroupSizesType]
(
	[PK_NumberGroupSizesTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblNumberGroupSizesType_DeletedRowVersionUpdate_ForSync
   ON track.tblNumberGroupSizesType
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
        FROM track.tblNumberGroupSizesType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END