/* {CheckPoint: CREATING TRACKING TABLE for tblEntityFuelCardLimitToSite } */

/****** Object:  Table [track].[tblEntityFuelCardLimitToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityFuelCardLimitToSite]
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
	[PK_FuelCardLimitToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityFuelCardLimitToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityFuelCardLimitToSite_PK_FuelCardLimitToSiteGuid] ON [track].[tblEntityFuelCardLimitToSite]
(
    [PK_FuelCardLimitToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityFuelCardLimitToSite_InsertedRowVersion] ON [track].[tblEntityFuelCardLimitToSite]
(
    [InsertedRowVersion] ASC,
    [PK_FuelCardLimitToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityFuelCardLimitToSite_UpdatedRowVersion] ON [track].[tblEntityFuelCardLimitToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_FuelCardLimitToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityFuelCardLimitToSite_DeletedRowVersion] ON [track].[tblEntityFuelCardLimitToSite]
(
    [DeletedRowVersion] ASC,
    [PK_FuelCardLimitToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityFuelCardLimitToSite_PK_FuelCardLimitToSiteGuid_Sync] ON [track].[tblEntityFuelCardLimitToSite]
(
	[PK_FuelCardLimitToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityFuelCardLimitToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityFuelCardLimitToSite
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
        FROM track.tblEntityFuelCardLimitToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END