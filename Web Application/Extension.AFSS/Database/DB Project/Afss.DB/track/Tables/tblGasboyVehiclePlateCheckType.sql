--Creating Sync Tracking Table for tblGasboyVehiclePlateCheckType
--Creating Sync Tracking Table for tblGasboyVehiclePlateCheckType
CREATE TABLE [track].[tblGasboyVehiclePlateCheckType]( 
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
	[PK_GasboyVehiclePlateCheckTypeIndex] INT NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	CONSTRAINT [PK_track_tblGasboyVehiclePlateCheckType] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyVehiclePlateCheckType_PK_GasboyVehiclePlateCheckTypeIndex] ON [track].[tblGasboyVehiclePlateCheckType]
(
    [PK_GasboyVehiclePlateCheckTypeIndex] ASC
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
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyVehiclePlateCheckType_InsertedRowVersion] ON [track].[tblGasboyVehiclePlateCheckType]
(
    [InsertedRowVersion] ASC
) INCLUDE 
(
    [InsertedDate], 
    [InsertedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_GasboyVehiclePlateCheckTypeIndex]
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyVehiclePlateCheckType_UpdatedRowVersion] ON [track].[tblGasboyVehiclePlateCheckType]
(
    [UpdatedRowVersion] ASC
) INCLUDE 
(
    [UpdatedDate], 
    [UpdatedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_GasboyVehiclePlateCheckTypeIndex]
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblGasboyVehiclePlateCheckType_DeletedRowVersion] ON [track].[tblGasboyVehiclePlateCheckType]
(
    [DeletedRowVersion] ASC
) INCLUDE 
(
    [DeletedDate], 
    [DeletedContext], 
    [CurrentSiteGuid], 
    [PreviousSiteGuid], 
    [PK_GasboyVehiclePlateCheckTypeIndex]
)