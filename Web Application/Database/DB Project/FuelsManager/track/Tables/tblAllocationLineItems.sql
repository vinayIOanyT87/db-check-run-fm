/* {CheckPoint: CREATING TRACKING TABLE for tblAllocationLineItems } */

/****** Object:  Table [track].[tblAllocationLineItems]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblAllocationLineItems]
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
	[PK_AllocationLineItemGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblAllocationLineItems_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAllocationLineItems_PK_AllocationLineItemGuid] ON [track].[tblAllocationLineItems]
(
    [PK_AllocationLineItemGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAllocationLineItems_InsertedRowVersion] ON [track].[tblAllocationLineItems]
(
    [InsertedRowVersion] ASC,
    [PK_AllocationLineItemGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAllocationLineItems_UpdatedRowVersion] ON [track].[tblAllocationLineItems]
(
    [UpdatedRowVersion] ASC,
    [PK_AllocationLineItemGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblAllocationLineItems_DeletedRowVersion] ON [track].[tblAllocationLineItems]
(
    [DeletedRowVersion] ASC,
    [PK_AllocationLineItemGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblAllocationLineItems_PK_AllocationLineItemGuid_Sync] ON [track].[tblAllocationLineItems]
(
	[PK_AllocationLineItemGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblAllocationLineItems_DeletedRowVersionUpdate_ForSync
   ON track.tblAllocationLineItems
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
        FROM track.tblAllocationLineItems t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END