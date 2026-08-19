/* {CheckPoint: CREATING TRACKING TABLE for tblManagerToAutoDistributionRule } */

/****** Object:  Table [track].[tblManagerToAutoDistributionRule]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblManagerToAutoDistributionRule]
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
	[PK_ManagerToAutoDistributionRuleGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblManagerToAutoDistributionRule_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblManagerToAutoDistributionRule_PK_ManagerToAutoDistributionRuleGuid] ON [track].[tblManagerToAutoDistributionRule]
(
    [PK_ManagerToAutoDistributionRuleGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblManagerToAutoDistributionRule_InsertedRowVersion] ON [track].[tblManagerToAutoDistributionRule]
(
    [InsertedRowVersion] ASC,
    [PK_ManagerToAutoDistributionRuleGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblManagerToAutoDistributionRule_UpdatedRowVersion] ON [track].[tblManagerToAutoDistributionRule]
(
    [UpdatedRowVersion] ASC,
    [PK_ManagerToAutoDistributionRuleGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblManagerToAutoDistributionRule_DeletedRowVersion] ON [track].[tblManagerToAutoDistributionRule]
(
    [DeletedRowVersion] ASC,
    [PK_ManagerToAutoDistributionRuleGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblManagerToAutoDistributionRule_PK_ManagerToAutoDistributionRuleGuid_Sync] ON [track].[tblManagerToAutoDistributionRule]
(
	[PK_ManagerToAutoDistributionRuleGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblManagerToAutoDistributionRule_DeletedRowVersionUpdate_ForSync
   ON track.tblManagerToAutoDistributionRule
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
        FROM track.tblManagerToAutoDistributionRule t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END