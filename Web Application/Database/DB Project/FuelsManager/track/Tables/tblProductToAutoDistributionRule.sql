/* {CheckPoint: CREATING TRACKING TABLE for tblProductToAutoDistributionRule } */

/****** Object:  Table [track].[tblProductToAutoDistributionRule]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblProductToAutoDistributionRule]
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
	[PK_ProductToAutoDistributionRuleGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblProductToAutoDistributionRule_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAutoDistributionRule_PK_ProductToAutoDistributionRuleGuid] ON [track].[tblProductToAutoDistributionRule]
(
    [PK_ProductToAutoDistributionRuleGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAutoDistributionRule_InsertedRowVersion] ON [track].[tblProductToAutoDistributionRule]
(
    [InsertedRowVersion] ASC,
    [PK_ProductToAutoDistributionRuleGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAutoDistributionRule_UpdatedRowVersion] ON [track].[tblProductToAutoDistributionRule]
(
    [UpdatedRowVersion] ASC,
    [PK_ProductToAutoDistributionRuleGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblProductToAutoDistributionRule_DeletedRowVersion] ON [track].[tblProductToAutoDistributionRule]
(
    [DeletedRowVersion] ASC,
    [PK_ProductToAutoDistributionRuleGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblProductToAutoDistributionRule_PK_ProductToAutoDistributionRuleGuid_Sync] ON [track].[tblProductToAutoDistributionRule]
(
	[PK_ProductToAutoDistributionRuleGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblProductToAutoDistributionRule_DeletedRowVersionUpdate_ForSync
   ON track.tblProductToAutoDistributionRule
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
        FROM track.tblProductToAutoDistributionRule t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END