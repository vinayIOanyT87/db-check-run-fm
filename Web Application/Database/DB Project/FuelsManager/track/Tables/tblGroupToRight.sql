/* {CheckPoint: CREATING TRACKING TABLE for tblGroupToRight } */

/****** Object:  Table [track].[tblGroupToRight]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblGroupToRight]
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
	[PK_GroupToRightGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblGroupToRight_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToRight_PK_GroupToRightGuid] ON [track].[tblGroupToRight]
(
    [PK_GroupToRightGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToRight_InsertedRowVersion] ON [track].[tblGroupToRight]
(
    [InsertedRowVersion] ASC,
    [PK_GroupToRightGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToRight_UpdatedRowVersion] ON [track].[tblGroupToRight]
(
    [UpdatedRowVersion] ASC,
    [PK_GroupToRightGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblGroupToRight_DeletedRowVersion] ON [track].[tblGroupToRight]
(
    [DeletedRowVersion] ASC,
    [PK_GroupToRightGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGroupToRight_PK_GroupToRightGuid_Sync] ON [track].[tblGroupToRight]
(
	[PK_GroupToRightGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblGroupToRight_DeletedRowVersionUpdate_ForSync
   ON track.tblGroupToRight
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
        FROM track.tblGroupToRight t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END