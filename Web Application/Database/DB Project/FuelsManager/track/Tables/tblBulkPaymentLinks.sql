/* {CheckPoint: CREATING TRACKING TABLE for tblBulkPaymentLinks } */

/****** Object:  Table [track].[tblBulkPaymentLinks]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblBulkPaymentLinks]
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
	[PK_BulkPaymentLinkGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblBulkPaymentLinks_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblBulkPaymentLinks_PK_BulkPaymentLinkGuid] ON [track].[tblBulkPaymentLinks]
(
    [PK_BulkPaymentLinkGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblBulkPaymentLinks_InsertedRowVersion] ON [track].[tblBulkPaymentLinks]
(
    [InsertedRowVersion] ASC,
    [PK_BulkPaymentLinkGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblBulkPaymentLinks_UpdatedRowVersion] ON [track].[tblBulkPaymentLinks]
(
    [UpdatedRowVersion] ASC,
    [PK_BulkPaymentLinkGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblBulkPaymentLinks_DeletedRowVersion] ON [track].[tblBulkPaymentLinks]
(
    [DeletedRowVersion] ASC,
    [PK_BulkPaymentLinkGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblBulkPaymentLinks_PK_BulkPaymentLinkGuid_Sync] ON [track].[tblBulkPaymentLinks]
(
	[PK_BulkPaymentLinkGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblBulkPaymentLinks_DeletedRowVersionUpdate_ForSync
   ON track.tblBulkPaymentLinks
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
        FROM track.tblBulkPaymentLinks t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END