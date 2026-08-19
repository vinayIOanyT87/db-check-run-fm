/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataFieldProduct } */

/****** Object:  Table [track].[tblUserDataFieldProduct]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataFieldProduct]
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
	[PK_UserDataFieldProductGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataFieldProduct_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldProduct_PK_UserDataFieldProductGuid] ON [track].[tblUserDataFieldProduct]
(
    [PK_UserDataFieldProductGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldProduct_InsertedRowVersion] ON [track].[tblUserDataFieldProduct]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataFieldProductGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldProduct_UpdatedRowVersion] ON [track].[tblUserDataFieldProduct]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataFieldProductGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldProduct_DeletedRowVersion] ON [track].[tblUserDataFieldProduct]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataFieldProductGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldProduct_PK_UserDataFieldProductGuid_Sync] ON [track].[tblUserDataFieldProduct]
(
	[PK_UserDataFieldProductGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataFieldProduct_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataFieldProduct
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
        FROM track.tblUserDataFieldProduct t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END