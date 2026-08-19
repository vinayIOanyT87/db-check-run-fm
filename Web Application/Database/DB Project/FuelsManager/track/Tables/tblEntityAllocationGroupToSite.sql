/* {CheckPoint: CREATING TRACKING TABLE for tblEntityAllocationGroupToSite } */

/****** Object:  Table [track].[tblEntityAllocationGroupToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityAllocationGroupToSite]
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
	[PK_AllocationGroupToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityAllocationGroupToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAllocationGroupToSite_PK_AllocationGroupToSiteGuid] ON [track].[tblEntityAllocationGroupToSite]
(
    [PK_AllocationGroupToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAllocationGroupToSite_InsertedRowVersion] ON [track].[tblEntityAllocationGroupToSite]
(
    [InsertedRowVersion] ASC,
    [PK_AllocationGroupToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAllocationGroupToSite_UpdatedRowVersion] ON [track].[tblEntityAllocationGroupToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_AllocationGroupToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityAllocationGroupToSite_DeletedRowVersion] ON [track].[tblEntityAllocationGroupToSite]
(
    [DeletedRowVersion] ASC,
    [PK_AllocationGroupToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityAllocationGroupToSite_PK_AllocationGroupToSiteGuid_Sync] ON [track].[tblEntityAllocationGroupToSite]
(
	[PK_AllocationGroupToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityAllocationGroupToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityAllocationGroupToSite
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
        FROM track.tblEntityAllocationGroupToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END