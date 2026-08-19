CREATE TYPE [dbo].[OwnerCloseoutType] AS TABLE
(
    [Site]               NVARCHAR (30)      NOT NULL,
    [ManagerName]        NVARCHAR (100)     NOT NULL,
	[OwnerName]          NVARCHAR (100)     NOT NULL,
    [ProductName]        NVARCHAR (30)      NOT NULL,
    [CloseoutDate]       DATE               NOT NULL,
	[SiteGuid]           UNIQUEIDENTIFIER   NULL,
    [ManagerCompanyGuid] UNIQUEIDENTIFIER   NULL,
    [OwnerCompanyGuid]   UNIQUEIDENTIFIER   NULL,
    [ProductGuid]        UNIQUEIDENTIFIER   NULL,
    [GrossBookInventory] FLOAT (53)         NULL,
    [NetBookInventory]   FLOAT (53)         NULL,
	[MassBookInventory]  FLOAT (53)         NULL,
    [GrossBookPrice]     FLOAT (53)         NULL,
    [NetBookPrice]       FLOAT (53)         NULL,
    [MassBookPrice]      FLOAT (53)         NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL
)
