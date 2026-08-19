/* {CheckPoint: CREATING TRACKING TABLE for tblMessageLocationType } */

/****** Object:  Table [track].[tblMessageLocationType]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblMessageLocationType]
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
	[PK_MessageLocationTypeIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblMessageLocationType_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageLocationType_PK_MessageLocationTypeIndex] ON [track].[tblMessageLocationType]
(
    [PK_MessageLocationTypeIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageLocationType_InsertedRowVersion] ON [track].[tblMessageLocationType]
(
    [InsertedRowVersion] ASC,
    [PK_MessageLocationTypeIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageLocationType_UpdatedRowVersion] ON [track].[tblMessageLocationType]
(
    [UpdatedRowVersion] ASC,
    [PK_MessageLocationTypeIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblMessageLocationType_DeletedRowVersion] ON [track].[tblMessageLocationType]
(
    [DeletedRowVersion] ASC,
    [PK_MessageLocationTypeIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblMessageLocationType_PK_MessageLocationTypeIndex_Sync] ON [track].[tblMessageLocationType]
(
	[PK_MessageLocationTypeIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblMessageLocationType_DeletedRowVersionUpdate_ForSync
   ON track.tblMessageLocationType
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
        FROM track.tblMessageLocationType t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END