/* {CheckPoint: CREATING TRACKING TABLE for tblOwnerToAutoDistributionRule } */

/****** Object:  Table [track].[tblOwnerToAutoDistributionRule]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblOwnerToAutoDistributionRule]
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
	[PK_OwnerToAutoDistributionRuleGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblOwnerToAutoDistributionRule_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerToAutoDistributionRule_PK_OwnerToAutoDistributionRuleGuid] ON [track].[tblOwnerToAutoDistributionRule]
(
    [PK_OwnerToAutoDistributionRuleGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerToAutoDistributionRule_InsertedRowVersion] ON [track].[tblOwnerToAutoDistributionRule]
(
    [InsertedRowVersion] ASC,
    [PK_OwnerToAutoDistributionRuleGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerToAutoDistributionRule_UpdatedRowVersion] ON [track].[tblOwnerToAutoDistributionRule]
(
    [UpdatedRowVersion] ASC,
    [PK_OwnerToAutoDistributionRuleGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblOwnerToAutoDistributionRule_DeletedRowVersion] ON [track].[tblOwnerToAutoDistributionRule]
(
    [DeletedRowVersion] ASC,
    [PK_OwnerToAutoDistributionRuleGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblOwnerToAutoDistributionRule_PK_OwnerToAutoDistributionRuleGuid_Sync] ON [track].[tblOwnerToAutoDistributionRule]
(
	[PK_OwnerToAutoDistributionRuleGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblOwnerToAutoDistributionRule_DeletedRowVersionUpdate_ForSync
   ON track.tblOwnerToAutoDistributionRule
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
        FROM track.tblOwnerToAutoDistributionRule t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END