/* {CheckPoint: CREATING TRACKING TABLE for tblProductToAdditiveProfile } */

/****** Object:  Table [track].[tblProductToAdditiveProfile]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProductToAdditiveProfile]
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
	[PK_ProductToAdditiveProfileGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToAdditiveProfile_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAdditiveProfile_PK_ProductToAdditiveProfileGuid] ON [track].[tblProductToAdditiveProfile]
(
    [PK_ProductToAdditiveProfileGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAdditiveProfile_InsertedRowVersion] ON [track].[tblProductToAdditiveProfile]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToAdditiveProfileGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAdditiveProfile_UpdatedRowVersion] ON [track].[tblProductToAdditiveProfile]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToAdditiveProfileGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAdditiveProfile_DeletedRowVersion] ON [track].[tblProductToAdditiveProfile]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToAdditiveProfileGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToAdditiveProfile_PK_ProductToAdditiveProfileGuid_Sync] ON [track].[tblProductToAdditiveProfile]
(
	[PK_ProductToAdditiveProfileGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToAdditiveProfile_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToAdditiveProfile
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
        FROM track.tblProductToAdditiveProfile t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END