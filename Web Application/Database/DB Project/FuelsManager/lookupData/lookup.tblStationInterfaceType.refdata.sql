SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [lookup].[tblStationInterfaceType]'
PRINT ''

DECLARE @StationInterfaceTypeRefDataInserted bigint
DECLARE @StationInterfaceTypeRefDataUpdated bigint
DECLARE @StationInterfaceTypeRefDataDeleted bigint

SET @StationInterfaceTypeRefDataInserted = 0
SET @StationInterfaceTypeRefDataUpdated = 0
SET @StationInterfaceTypeRefDataDeleted = 0

DECLARE @tblStationInterfaceTypeRefData TABLE
(
	[ActionType] VARCHAR (50)
    ,[OldStationInterfaceTypeIndex] INT
    ,[StationInterfaceTypeIndex] INT
    ,[OldStationInterfaceTypeCode] NVARCHAR (100)
    ,[StationInterfaceTypeCode] NVARCHAR (100)
	,[OldStationInterfaceTypeName]  NVARCHAR (100)
	,[StationInterfaceTypeName]  NVARCHAR (100)
	,[OldStationInterfaceTypeGuid]  NVARCHAR (100)
	,[StationInterfaceTypeGuid]  NVARCHAR (100)
    ,[OldCreatedDate] DATETIMEOFFSET (7)
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[OldCreatedBy] NVARCHAR (255)
    ,[CreatedBy] NVARCHAR (255)
    ,[OldUpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[OldUpdatedBy] NVARCHAR (255)
    ,[UpdatedBy] NVARCHAR (255)
);

; MERGE INTO [lookup].[tblStationInterfaceType] AS Target
USING (VALUES
(0, N'ACCULOADIII_Q', N'ACCULOADIII Q', N'566e311b-ef75-4783-b6c1-945f87f4d4e8', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(1, N'PROXIMITY_CARD_READER', N'PROXIMITY CARD READER', N'24e0c206-9b2a-414a-b1d1-c04d6e8748ff', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(2, N'PASS_CONTROLLER', N'PASS CONTROLLER', N'd440070a-b8b0-4818-b166-8ac72db24540', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(3, N'VAREC_DET', N'VAREC DET', N'8ea5885a-03f6-48f8-a077-3db0a4ab3205', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(4, N'MANUAL', N'MANUAL', N'f0fdda37-c13a-4773-a680-4f6545581ba4', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(5, N'MICROLOAD_NET', N'MICROLOAD NET', N'984e7a39-c75c-4e86-8541-6cb49208a34a', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(6, N'DANLOAD6000', N'DANLOAD6000', N'030ef16c-859b-47aa-8460-26dd253ad405', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(7, N'MULTILOAD_II_SMP', N'MULTILOAD II SMP', N'0496f430-3ad3-4370-a569-ff08ca383efc', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(8, N'SIGNATURE', N'SIGNATURE', N'5df457f7-f076-454d-9c5f-819e6c538736', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(9, N'METER', N'METER', N'44e55dbb-852e-4bc4-aa52-c9f9eb7a552c', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(10, N'ACCULOADIII_SA', N'ACCULOADIII SA', N'1f96b192-95d7-45aa-ad4d-ea76e7498226', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(11, N'CONTREC1010', N'CONTREC1010', N'4dfdda79-f0e9-41ef-ae2f-5ee8d2f19a4e', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(12, N'MULTILOAD_II', N'MULTILOAD II', N'50ab4577-e18d-4055-955d-75743142b8b8', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(13, N'CONTREC1010_RA', N'CONTREC1010 RA', N'daf80843-5609-4522-897c-4648c63b30f3', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'6/18/2012 1:02:24 PM +00:00', N'Administrator')
,(14, N'SCULLY', N'SCULLY', N'0abbe440-cbed-11ec-b6b9-103d1cbd9c45', N'5/4/2022 4:48:42 PM -04:00', N'Administrator', N'5/4/2022 4:48:42 PM -04:00', N'Administrator')
,(15, N'RCU_II_OPEN', N'RCU_II_OPEN', N'0abbe441-cbed-11ec-b6b9-103d1cbd9c45', N'5/4/2022 4:48:42 PM -04:00', N'Administrator', N'5/4/2022 4:48:42 PM -04:00', N'Administrator')
,(16, N'RCU_II_RCU', N'RCU_II_RCU', N'0abbe442-cbed-11ec-b6b9-103d1cbd9c45', N'5/4/2022 4:48:42 PM -04:00', N'Administrator', N'5/4/2022 4:48:42 PM -04:00', N'Administrator')
,(17, N'HID_CARD_READER', N'HID_CARD_READER', N'0abbe443-cbed-11ec-b6b9-103d1cbd9c45', N'5/4/2022 4:48:42 PM -04:00', N'Administrator', N'5/4/2022 4:48:42 PM -04:00', N'Administrator')
,(18, N'REVUELTARADMTX', N'REVUELTARADMTX', N'0abbe444-cbed-11ec-b6b9-103d1cbd9c45', N'5/4/2022 4:48:42 PM -04:00', N'Administrator', N'5/4/2022 4:48:42 PM -04:00', N'Administrator')
,(19, N'ODSP_CARD_READER', N'OSDP_CARD_READER', N'905CCF60-ECB9-41FB-8BA4-AEB292CEB806', N'8/25/2023 10:56:00 AM -04:00', N'Administrator', N'8/25/2023 10:56:00 AM -04:00', N'Administrator')
,(20, N'MAX_TYPE', N'MAX TYPE', N'0a94e34b-f209-4fd9-adca-21ee63ad7b71', N'6/18/2012 1:02:24 PM +00:00', N'Administrator', N'5/4/2022 4:48:42 PM -04:00', N'Administrator')
) AS Source ([StationInterfaceTypeIndex], [StationInterfaceTypeCode], [StationInterfaceTypeName], [StationInterfaceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[StationInterfaceTypeIndex] = Source.[StationInterfaceTypeIndex])
WHEN MATCHED AND (Target.[StationInterfaceTypeCode] <> Source.[StationInterfaceTypeCode] 
					OR Target.[StationInterfaceTypeName] <> Source.[StationInterfaceTypeName]
					OR Target.[StationInterfaceTypeGuid] <> Source.[StationInterfaceTypeGuid]) THEN
	UPDATE SET 
				[StationInterfaceTypeCode] = Source.[StationInterfaceTypeCode]
				, [StationInterfaceTypeName] = Source.[StationInterfaceTypeName]
				, [StationInterfaceTypeGuid] = Source.[StationInterfaceTypeGuid]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([StationInterfaceTypeIndex], [StationInterfaceTypeCode], [StationInterfaceTypeName], [StationInterfaceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[StationInterfaceTypeIndex],Source.[StationInterfaceTypeCode],Source.[StationInterfaceTypeName],Source.[StationInterfaceTypeGuid],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[StationInterfaceTypeIndex],
   inserted.[StationInterfaceTypeIndex],
   deleted.[StationInterfaceTypeCode],
   inserted.[StationInterfaceTypeCode],
   deleted.[StationInterfaceTypeName],
   inserted.[StationInterfaceTypeName],
   deleted.[StationInterfaceTypeGuid],
   inserted.[StationInterfaceTypeGuid],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblStationInterfaceTypeRefData;

SELECT @StationInterfaceTypeRefDataInserted = COUNT(*) FROM @tblStationInterfaceTypeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @StationInterfaceTypeRefDataUpdated = COUNT(*) FROM @tblStationInterfaceTypeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @StationInterfaceTypeRefDataDeleted = COUNT(*) FROM @tblStationInterfaceTypeRefData WHERE ActionType IN ( 'DELETE' )

IF (@StationInterfaceTypeRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @StationInterfaceTypeRefDataInserted) + ' NEW RECORDS INSERTED INTO [lookup].[tblStationInterfaceType] **'
	PRINT ''
END

IF (@StationInterfaceTypeRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @StationInterfaceTypeRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [lookup].[tblStationInterfaceType] **'
	PRINT ''
	SELECT * FROM @tblStationInterfaceTypeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
