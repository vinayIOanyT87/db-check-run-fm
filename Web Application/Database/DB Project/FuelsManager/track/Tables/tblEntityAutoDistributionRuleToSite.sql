/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAutoDistributionRuleToSite } */

/****** Object:  Table [track].[tblEntityAutoDistributionRuleToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAutoDistributionRuleToSite]
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
	[PK_AutoDistributionRuleToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAutoDistributionRuleToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionRuleToSite_PK_AutoDistributionRuleToSiteGuid] ON [track].[tblEntityAutoDistributionRuleToSite]
(
    [PK_AutoDistributionRuleToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionRuleToSite_InsertedRowVersion] ON [track].[tblEntityAutoDistributionRuleToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AutoDistributionRuleToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionRuleToSite_UpdatedRowVersion] ON [track].[tblEntityAutoDistributionRuleToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AutoDistributionRuleToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionRuleToSite_DeletedRowVersion] ON [track].[tblEntityAutoDistributionRuleToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AutoDistributionRuleToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAutoDistributionRuleToSite_PK_AutoDistributionRuleToSiteGuid_Sync] ON [track].[tblEntityAutoDistributionRuleToSite]
(
	[PK_AutoDistributionRuleToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAutoDistributionRuleToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAutoDistributionRuleToSite
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
        FROM track.tblEntityAutoDistributionRuleToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END