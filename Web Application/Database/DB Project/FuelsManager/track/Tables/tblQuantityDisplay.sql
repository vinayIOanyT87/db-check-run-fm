/* {CheckPoint: CREATING TRACKING TABLE for tblQuantityDisplay } */

/****** Object:  Table [track].[tblQuantityDisplay]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblQuantityDisplay]
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
	[PK_QuantityDisplayIndex] [tinyint] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblQuantityDisplay_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQuantityDisplay_PK_QuantityDisplayIndex] ON [track].[tblQuantityDisplay]
(
    [PK_QuantityDisplayIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQuantityDisplay_InsertedRowVersion] ON [track].[tblQuantityDisplay]
(
    [InsertedRowVersion] ASC,
    [PK_QuantityDisplayIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQuantityDisplay_UpdatedRowVersion] ON [track].[tblQuantityDisplay]
(
    [UpdatedRowVersion] ASC,
    [PK_QuantityDisplayIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblQuantityDisplay_DeletedRowVersion] ON [track].[tblQuantityDisplay]
(
    [DeletedRowVersion] ASC,
    [PK_QuantityDisplayIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblQuantityDisplay_PK_QuantityDisplayIndex_Sync] ON [track].[tblQuantityDisplay]
(
	[PK_QuantityDisplayIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblQuantityDisplay_DeletedRowVersionUpdate_ForSync
   ON track.tblQuantityDisplay
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
        FROM track.tblQuantityDisplay t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END