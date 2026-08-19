/* {CheckPoint: CREATING TRACKING TABLE for tblAutoDistributionReasonCodes } */

/****** Object:  Table [track].[tblAutoDistributionReasonCodes]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAutoDistributionReasonCodes]
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
	[PK_AutoDistributionReasonCodeGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAutoDistributionReasonCodes_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAutoDistributionReasonCodes_PK_AutoDistributionReasonCodeGuid] ON [track].[tblAutoDistributionReasonCodes]
(
    [PK_AutoDistributionReasonCodeGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAutoDistributionReasonCodes_InsertedRowVersion] ON [track].[tblAutoDistributionReasonCodes]
(
    [InsertedRowVersion] ASC,
    [PK_AutoDistributionReasonCodeGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAutoDistributionReasonCodes_UpdatedRowVersion] ON [track].[tblAutoDistributionReasonCodes]
(
    [UpdatedRowVersion] ASC,
    [PK_AutoDistributionReasonCodeGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAutoDistributionReasonCodes_DeletedRowVersion] ON [track].[tblAutoDistributionReasonCodes]
(
    [DeletedRowVersion] ASC,
    [PK_AutoDistributionReasonCodeGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAutoDistributionReasonCodes_PK_AutoDistributionReasonCodeGuid_Sync] ON [track].[tblAutoDistributionReasonCodes]
(
	[PK_AutoDistributionReasonCodeGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAutoDistributionReasonCodes_DeletedRowVersionUpdate_ForSync
   ON track.tblAutoDistributionReasonCodes
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
        FROM track.tblAutoDistributionReasonCodes t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END