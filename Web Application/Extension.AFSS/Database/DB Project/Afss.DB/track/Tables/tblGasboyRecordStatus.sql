--Creating Sync Tracking Table for tblGasboyRecordStatus
--Creating Sync Tracking Table for tblGasboyRecordStatus
CREATE TABLE [track].[tblGasboyRecordStatus]( 
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
	[PK_GasboyRecordStatusIndex] INT NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	CONSTRAINT [PK_track_tblGasboyRecordStatus] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyRecordStatus_PK_GasboyRecordStatusIndex] ON [track].[tblGasboyRecordStatus]
(
    [PK_GasboyRecordStatusIndex] ASC
) INCLUDE 
(
    [InsertedDate], 
    [InsertedRowVersion], 
    [InsertedContext], 
    [UpdatedDate], 
    [UpdatedRowVersion], 
    [UpdatedContext], 
    [DeletedDate], 
    [DeletedRowVersion], 
    [DeletedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid] 
) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyRecordStatus_InsertedRowVersion] ON [track].[tblGasboyRecordStatus]
(
    [InsertedRowVersion] ASC
) INCLUDE 
(
    [InsertedDate], 
    [InsertedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_GasboyRecordStatusIndex]
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyRecordStatus_UpdatedRowVersion] ON [track].[tblGasboyRecordStatus]
(
    [UpdatedRowVersion] ASC
) INCLUDE 
(
    [UpdatedDate], 
    [UpdatedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_GasboyRecordStatusIndex]
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyRecordStatus_DeletedRowVersion] ON [track].[tblGasboyRecordStatus]
(
    [DeletedRowVersion] ASC
) INCLUDE 
(
    [DeletedDate], 
    [DeletedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_GasboyRecordStatusIndex]
)