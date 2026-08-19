--Creating Sync Tracking Table for tblExternalStationTransactionError
--Creating Sync Tracking Table for tblExternalStationTransactionError
CREATE TABLE [track].[tblExternalStationTransactionError]( 
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
	[PK_ExternalStationTransactionErrorGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	CONSTRAINT [PK_track_tblExternalStationTransactionError] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblExternalStationTransactionError_PK_ExternalStationTransactionErrorGuid] ON [track].[tblExternalStationTransactionError]
(
    [PK_ExternalStationTransactionErrorGuid] ASC
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
CREATE NONCLUSTERED INDEX [IX_track_tblExternalStationTransactionError_InsertedRowVersion] ON [track].[tblExternalStationTransactionError]
(
    [InsertedRowVersion] ASC
) INCLUDE 
(
    [InsertedDate], 
    [InsertedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_ExternalStationTransactionErrorGuid]
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblExternalStationTransactionError_UpdatedRowVersion] ON [track].[tblExternalStationTransactionError]
(
    [UpdatedRowVersion] ASC
) INCLUDE 
(
    [UpdatedDate], 
    [UpdatedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_ExternalStationTransactionErrorGuid]
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblExternalStationTransactionError_DeletedRowVersion] ON [track].[tblExternalStationTransactionError]
(
    [DeletedRowVersion] ASC
) INCLUDE 
(
    [DeletedDate], 
    [DeletedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_ExternalStationTransactionErrorGuid]
)