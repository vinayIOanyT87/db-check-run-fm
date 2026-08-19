CREATE TYPE [dbo].[GasboyStationEventType] AS TABLE
(
	[GasboyStationEventGuid] UNIQUEIDENTIFIER NOT NULL, 
    [ExternalStationLogGuid] UNIQUEIDENTIFIER NOT NULL, 
	[EventID] INT NULL,
    [LookupGasboyEventErrorClassCodeIndex] INT NULL, 
	[ErrorCode] INT NULL, 
    [FleetID] INT NULL, 
    [ObjectID] INT NULL, 
	[LookupGasboyEventObjectTypeIndex] INT NULL,
	[DeviceName] NVARCHAR(100) NULL,
	[Field1] NVARCHAR(100) NULL,
	[Field2] NVARCHAR(100) NULL,
	[Field3] NVARCHAR(100) NULL,
	[Field4] NVARCHAR(100) NULL,
	[Field5] NVARCHAR(100) NULL,
	[Field6] NVARCHAR(100) NULL,
	[Field7] NVARCHAR(100) NULL,
	[Field8] NVARCHAR(100) NULL,
	CreatedUpdatedBy dbo.udtUserID NOT NULL
)
