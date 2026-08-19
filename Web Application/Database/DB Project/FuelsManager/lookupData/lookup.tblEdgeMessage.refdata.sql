SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [lookup].[tblEdgeMessage]'
PRINT ''

DECLARE @EdgeMessageRefDataInserted bigint
DECLARE @EdgeMessageRefDataUpdated bigint
DECLARE @EdgeMessageRefDataDeleted bigint

SET @EdgeMessageRefDataInserted = 0
SET @EdgeMessageRefDataUpdated = 0
SET @EdgeMessageRefDataDeleted = 0

DECLARE @tblEdgeMessageRefData TABLE
(
	[ActionType] VARCHAR (50)
    ,[OldEdgeMessageIndex] INT
    ,[EdgeMessageIndex] INT
    ,[OldEdgeMessageCode] NVARCHAR (100)
    ,[EdgeMessageCode] NVARCHAR (100)
	,[OldEdgeMessageName]  NVARCHAR (100)
	,[EdgeMessageName]  NVARCHAR (100)
	,[OldEdgeMessageGuid]  NVARCHAR (100)
	,[EdgeMessageGuid]  NVARCHAR (100)
    ,[OldCreatedDate] DATETIMEOFFSET (7)
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[OldCreatedBy] NVARCHAR (255)
    ,[CreatedBy] NVARCHAR (255)
    ,[OldUpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[OldUpdatedBy] NVARCHAR (255)
    ,[UpdatedBy] NVARCHAR (255)
);

; MERGE INTO [lookup].[tblEdgeMessage] AS Target
USING (VALUES
(0, N'InValid', N'InValid', N'FC2C5674-040D-4F8A-B7DF-92EF48286E39', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(1, N'Heartbeat', N'Heartbeat', N'3E6D67E6-D167-4F1D-AA5D-A9E30393361D', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(2, N'SoftwareVersion', N'SoftwareVersion', N'42F17866-5CFF-41B6-A72A-0A3D64BA66AF', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(3, N'DeviceStatus', N'DeviceStatus', N'EB5612C7-BDCC-4C96-AC72-DE3F5AC9E92D', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(4, N'Enraf854TankGauge', N'Enraf854TankGauge', N'4DFD0067-7DD2-4012-BBD5-F6A6C4C78C67', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(5, N'Enraf854TankGaugeDensity', N'Enraf854TankGaugeDensity', N'3D764789-F470-4D4A-9C82-66D4A434739F', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(6, N'ModbusIntegerRegisterBlock', N'ModbusIntegerRegisterBlock', N'A522D188-47B6-4846-A959-D81A5DA2A0E4', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(7, N'GenericScalingPoint', N'GenericScalingPoint', N'038FEF4C-F34B-4207-88AE-841BE4702FA9', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(8, N'ITTBarton3500ATG', N'ITTBarton3500ATG', N'3602746E-2FC1-48CA-A466-1A06C1D48E40', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(9, N'VeederRootTLS350', N'VeederRootTLS350', N'8E4E1033-835E-41DF-A045-917FCF9DF524', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(10, N'VeederRootSystemStatus', N'VeederRootSystemStatus', N'CA3D063E-3703-4B0F-9AF1-132F7D9B2B4D', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(11, N'VeederRootLeakTest', N'VeederRootLeakTest', N'5820DB29-3665-436F-BEF0-E745951BB7BD', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(12, N'VeederRootSystemAlarms', N'VeederRootSystemAlarms', N'3E02D411-8B0E-4A22-B0C8-EC554072A4E8', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(13, N'VeederRootInventoryReport', N'VeederRootInventoryReport', N'49A0CE2A-365A-4FB8-8E72-EA7E05859ED9', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(14, N'VeederRootInTankStatusReport', N'VeederRootInTankStatusReport', N'9E64373E-D89E-45BC-A445-EF6977014EA5', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'10/04/2022 1:03:42 PM +00:00', N'Administrator')
,(15, N'VeederRootLiquidSensorStatusReport', N'VeederRootLiquidSensorStatusReport', N'4DF00D2C-3FF0-474A-840A-DD17E4A12FC5', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(16, N'ModbusInventory', N'ModbusInventory', N'830AC061-E211-43B9-83E0-CABBA565FF32', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(17, N'ModbusDensityAndAlarm', N'ModbusDensityAndAlarm', N'BC7FEE45-7B71-44BA-86BB-2BED5A97029C', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(18, N'ModbusFacilityStatus', N'ModbusFacilityStatus', N'B5C0896B-422F-409E-AA40-2DAF15CCCFAE', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(19, N'ModbusStorage', N'ModbusStorage', N'59D1BD58-7670-4D4D-8A14-F35641E4FF6D', N'10/04/2022 1:03:42 PM +00:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(20, N'CommandStatus', N'CommandStatus', N'E3C1EEA6-84DD-4932-8AE0-B91CA2DE4107', N'11/20/2023 1:03:42 PM +00:00', N'Administrator', N'11/20/2023 11:04:54 AM -04:00', N'Administrator')
,(21, N'WAGOPLC', N'WAGOPLC', N'E84BDEF1-41B4-4987-8355-175C59F3A702', N'11/20/2023 1:03:42 PM +00:00', N'Administrator', N'11/20/2023 11:04:54 AM -04:00', N'Administrator')
) AS Source ([EdgeMessageIndex], [EdgeMessageCode], [EdgeMessageName], [EdgeMessageGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[EdgeMessageIndex] = Source.[EdgeMessageIndex])
WHEN MATCHED AND (Target.[EdgeMessageCode] <> Source.[EdgeMessageCode] 
					OR Target.[EdgeMessageName] <> Source.[EdgeMessageName]
					OR Target.[EdgeMessageGuid] <> Source.[EdgeMessageGuid]) THEN
	UPDATE SET 
				[EdgeMessageCode] = Source.[EdgeMessageCode]
				, [EdgeMessageName] = Source.[EdgeMessageName]
				, [EdgeMessageGuid] = Source.[EdgeMessageGuid]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([EdgeMessageIndex], [EdgeMessageCode], [EdgeMessageName], [EdgeMessageGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[EdgeMessageIndex],Source.[EdgeMessageCode],Source.[EdgeMessageName],Source.[EdgeMessageGuid],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[EdgeMessageIndex],
   inserted.[EdgeMessageIndex],
   deleted.[EdgeMessageCode],
   inserted.[EdgeMessageCode],
   deleted.[EdgeMessageName],
   inserted.[EdgeMessageName],
   deleted.[EdgeMessageGuid],
   inserted.[EdgeMessageGuid],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblEdgeMessageRefData;

SELECT @EdgeMessageRefDataInserted = COUNT(*) FROM @tblEdgeMessageRefData WHERE ActionType IN ( 'INSERT' );
SELECT @EdgeMessageRefDataUpdated = COUNT(*) FROM @tblEdgeMessageRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @EdgeMessageRefDataDeleted = COUNT(*) FROM @tblEdgeMessageRefData WHERE ActionType IN ( 'DELETE' )

IF (@EdgeMessageRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @EdgeMessageRefDataInserted) + ' NEW RECORDS INSERTED INTO [lookup].[tblEdgeMessage] **'
	PRINT ''
END

IF (@EdgeMessageRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @EdgeMessageRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [lookup].[tblEdgeMessage] **'
	PRINT ''
	SELECT * FROM @tblEdgeMessageRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
