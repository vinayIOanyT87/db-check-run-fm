/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataFieldFuelCard } */

/****** Object:  Table [track].[tblUserDataFieldFuelCard]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataFieldFuelCard]
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
	[PK_UserDataFieldFuelCardGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataFieldFuelCard_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldFuelCard_PK_UserDataFieldFuelCardGuid] ON [track].[tblUserDataFieldFuelCard]
(
    [PK_UserDataFieldFuelCardGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldFuelCard_InsertedRowVersion] ON [track].[tblUserDataFieldFuelCard]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataFieldFuelCardGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldFuelCard_UpdatedRowVersion] ON [track].[tblUserDataFieldFuelCard]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataFieldFuelCardGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldFuelCard_DeletedRowVersion] ON [track].[tblUserDataFieldFuelCard]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataFieldFuelCardGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataFieldFuelCard_PK_UserDataFieldFuelCardGuid_Sync] ON [track].[tblUserDataFieldFuelCard]
(
	[PK_UserDataFieldFuelCardGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataFieldFuelCard_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataFieldFuelCard
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
        FROM track.tblUserDataFieldFuelCard t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END