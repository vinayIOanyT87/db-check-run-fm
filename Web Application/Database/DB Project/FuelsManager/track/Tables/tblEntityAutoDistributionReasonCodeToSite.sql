/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAutoDistributionReasonCodeToSite } */

/****** Object:  Table [track].[tblEntityAutoDistributionReasonCodeToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAutoDistributionReasonCodeToSite]
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
	[PK_AutoDistributionReasonCodeToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAutoDistributionReasonCodeToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionReasonCodeToSite_PK_AutoDistributionReasonCodeToSiteGuid] ON [track].[tblEntityAutoDistributionReasonCodeToSite]
(
    [PK_AutoDistributionReasonCodeToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionReasonCodeToSite_InsertedRowVersion] ON [track].[tblEntityAutoDistributionReasonCodeToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AutoDistributionReasonCodeToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionReasonCodeToSite_UpdatedRowVersion] ON [track].[tblEntityAutoDistributionReasonCodeToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AutoDistributionReasonCodeToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionReasonCodeToSite_DeletedRowVersion] ON [track].[tblEntityAutoDistributionReasonCodeToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AutoDistributionReasonCodeToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionReasonCodeToSite_PK_AutoDistributionReasonCodeToSiteGuid_Sync] ON [track].[tblEntityAutoDistributionReasonCodeToSite]
(
	[PK_AutoDistributionReasonCodeToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAutoDistributionReasonCodeToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAutoDistributionReasonCodeToSite
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
        FROM track.tblEntityAutoDistributionReasonCodeToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END