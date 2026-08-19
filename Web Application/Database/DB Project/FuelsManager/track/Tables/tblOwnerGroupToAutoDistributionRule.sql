/* {CheckPoint: CREATING TRACKING TABLE for tblOwnerGroupToAutoDistributionRule } */

/****** Object:  Table [track].[tblOwnerGroupToAutoDistributionRule]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblOwnerGroupToAutoDistributionRule]
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
	[PK_OwnerGroupToAutoDistributionRuleGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblOwnerGroupToAutoDistributionRule_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerGroupToAutoDistributionRule_PK_OwnerGroupToAutoDistributionRuleGuid] ON [track].[tblOwnerGroupToAutoDistributionRule]
(
    [PK_OwnerGroupToAutoDistributionRuleGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerGroupToAutoDistributionRule_InsertedRowVersion] ON [track].[tblOwnerGroupToAutoDistributionRule]
(
    [InsertedRowVersion] ASC,
    [PK_OwnerGroupToAutoDistributionRuleGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerGroupToAutoDistributionRule_UpdatedRowVersion] ON [track].[tblOwnerGroupToAutoDistributionRule]
(
    [UpdatedRowVersion] ASC,
    [PK_OwnerGroupToAutoDistributionRuleGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerGroupToAutoDistributionRule_DeletedRowVersion] ON [track].[tblOwnerGroupToAutoDistributionRule]
(
    [DeletedRowVersion] ASC,
    [PK_OwnerGroupToAutoDistributionRuleGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblOwnerGroupToAutoDistributionRule_PK_OwnerGroupToAutoDistributionRuleGuid_Sync] ON [track].[tblOwnerGroupToAutoDistributionRule]
(
	[PK_OwnerGroupToAutoDistributionRuleGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblOwnerGroupToAutoDistributionRule_DeletedRowVersionUpdate_ForSync
   ON track.tblOwnerGroupToAutoDistributionRule
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
        FROM track.tblOwnerGroupToAutoDistributionRule t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END