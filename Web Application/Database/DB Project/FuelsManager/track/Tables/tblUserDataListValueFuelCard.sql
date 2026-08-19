/* {CheckPoint: CREATING TRACKING TABLE for tblUserDataListValueFuelCard } */

/****** Object:  Table [track].[tblUserDataListValueFuelCard]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblUserDataListValueFuelCard]
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
	[PK_UserDataListValueFuelCardGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblUserDataListValueFuelCard_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueFuelCard_PK_UserDataListValueFuelCardGuid] ON [track].[tblUserDataListValueFuelCard]
(
    [PK_UserDataListValueFuelCardGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueFuelCard_InsertedRowVersion] ON [track].[tblUserDataListValueFuelCard]
(
    [InsertedRowVersion] ASC,
    [PK_UserDataListValueFuelCardGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueFuelCard_UpdatedRowVersion] ON [track].[tblUserDataListValueFuelCard]
(
    [UpdatedRowVersion] ASC,
    [PK_UserDataListValueFuelCardGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueFuelCard_DeletedRowVersion] ON [track].[tblUserDataListValueFuelCard]
(
    [DeletedRowVersion] ASC,
    [PK_UserDataListValueFuelCardGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblUserDataListValueFuelCard_PK_UserDataListValueFuelCardGuid_Sync] ON [track].[tblUserDataListValueFuelCard]
(
	[PK_UserDataListValueFuelCardGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblUserDataListValueFuelCard_DeletedRowVersionUpdate_ForSync
   ON track.tblUserDataListValueFuelCard
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
        FROM track.tblUserDataListValueFuelCard t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END