SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [lookup].[tblApplicationStringType]'
PRINT ''

DECLARE @ApplicationStringTypeRefDataInserted bigint
DECLARE @ApplicationStringTypeRefDataUpdated bigint
DECLARE @ApplicationStringTypeRefDataDeleted bigint

SET @ApplicationStringTypeRefDataInserted = 0
SET @ApplicationStringTypeRefDataUpdated = 0
SET @ApplicationStringTypeRefDataDeleted = 0

DECLARE @tblApplicationStringTypeRefData TABLE
(	
	[ActionType] VARCHAR(50),
	[OldApplicationStringTypeIndex] INT ,
	[ApplicationStringTypeIndex] INT ,
	[OldApplicationStringTypeCode] NVARCHAR(100),
	[ApplicationStringTypeCode] NVARCHAR(100),
	[OldApplicationStringTypeName] NVARCHAR(100),
	[ApplicationStringTypeName] NVARCHAR(100),
	[OldApplicationStringTypeGuid] UNIQUEIDENTIFIER ,
	[ApplicationStringTypeGuid] UNIQUEIDENTIFIER ,
	[OldCreatedDate] DATETIMEOFFSET(7),
	[CreatedDate] DATETIMEOFFSET(7),
	[OldCreatedBy] [dbo].[udtUserID],
	[CreatedBy] [dbo].[udtUserID],
	[OldUpdatedDate] DATETIMEOFFSET(7),
	[UpdatedDate] DATETIMEOFFSET(7),
	[OldUpdatedBy] [dbo].[udtUserID],
	[UpdatedBy] [dbo].[udtUserID]
);


; MERGE INTO [lookup].[tblApplicationStringType] AS Target
USING (VALUES
(0, N'DOT_HAZARDOUS_MESSAGE', N'DOT HAZARDOUS MESSAGE', N'd3a14888-9702-46e7-8947-f9e1942445a7', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(1, N'PRODUCT_MESSAGE', N'PRODUCT MESSAGE', N'f971cd27-d648-413b-9d5f-e69538a6dc07', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(2, N'ALLOCATION_GROUP', N'ALLOCATION GROUP', N'8e2fb38e-5990-45de-b805-cdca3a7253d2', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(3, N'PRODUCT_GROUP', N'PRODUCT GROUP', N'22b9dcd7-57f2-4328-a036-c5d5757ac00c', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(4, N'COMPANY_TYPE', N'COMPANY TYPE', N'7dec768d-f4fd-470a-92c8-09864a6b9b15', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(5, N'ADDITIVE_PROFILE', N'ADDITIVE PROFILE', N'9c814aa4-596a-4e86-ba12-d5a198f2da46', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(6, N'ALARM_EVENT_CATEGORY', N'ALARM EVENT CATEGORY', N'0fa0d70e-7d91-47ce-9fc8-3ef08820ef1f', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(7, N'EMAIL_ADDRESS', N'EMAIL ADDRESS', N'bc48a642-02e0-43bd-89e6-630289cea911', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(8, N'COMPANY_GROUP', N'COMPANY GROUP', N'34497065-84f2-4d23-a656-fc2fd5cbad92', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(9, N'ENTRY_MESSAGE', N'ENTRY MESSAGE', N'b2915e2d-bc65-4883-a33c-a4f7db0a568d', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(10, N'EXIT_MESSAGE', N'EXIT MESSAGE', N'ef6fa165-64cc-4363-aac0-d2894b3ed217', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(11, N'PROCESS_VARIABLE_MESSAGE', N'PROCESS VARIABLE MESSAGE', N'9d53d289-3b82-4bae-81f5-cbd9c81c7fe6', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(12, N'FOOT_NOTE', N'FOOT NOTE', N'02293997-2932-4576-97b6-5aa2d0064836', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(13, N'SHIPTO_STATE', N'SHIPTO STATE', N'362fffb9-89f1-4fb3-a947-cc6d8ad520ab', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2012-06-18T09:06:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(14, N'FUEL_CARD_TYPE', N'FUEL CARD TYPE', N'3fcce30b-c1f3-480a-981f-c61cbaaca0cb', CAST(N'2014-03-18T16:13:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2014-03-18T16:13:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(15, N'POINT_TEMPLATE_TYPE', N'POINT TYPE', N'fb02a6db-78ea-4daf-a7bf-0fde27a50406', CAST(N'2014-03-18T16:13:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2014-03-18T16:13:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(16, N'SITE_CERTIFICATE', N'SITE_CERTIFICATE', N'06cb1eba-ae29-43c5-a846-5674ee0b5b35', CAST(N'2014-03-18T16:13:12.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2014-03-18T16:13:12.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(17, N'POINT_CATEGORY', N'POINT CATEGORY', N'1004f625-173c-4036-816d-73b1910d1c7b', CAST(N'2016-07-06T12:00:00.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2016-07-06T12:00:00.0000000+00:00' AS DateTimeOffset), N'Administrator'),
(18, N'MAX_STRING_TYPE', N'MAX STRING TYPE', N'd999c0ec-9499-4c67-a2b7-6adada4a242f', CAST(N'2016-07-06T12:00:00.0000000+00:00' AS DateTimeOffset), N'Administrator', CAST(N'2016-07-06T12:00:00.0000000+00:00' AS DateTimeOffset), N'Administrator')
)  AS Source ([ApplicationStringTypeIndex], [ApplicationStringTypeCode], [ApplicationStringTypeName], [ApplicationStringTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[ApplicationStringTypeIndex] = Source.[ApplicationStringTypeIndex])
WHEN MATCHED AND (Target.[ApplicationStringTypeCode] <> Source.[ApplicationStringTypeCode] 
					OR Target.[ApplicationStringTypeName] <> Source.[ApplicationStringTypeName]
					OR Target.[ApplicationStringTypeGuid] <> Source.[ApplicationStringTypeGuid]) THEN
	UPDATE SET [ApplicationStringTypeCode] = Source.[ApplicationStringTypeCode]
				, [ApplicationStringTypeName] = Source.[ApplicationStringTypeName]
				, [ApplicationStringTypeGuid] = Source.[ApplicationStringTypeGuid]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([ApplicationStringTypeIndex], [ApplicationStringTypeCode], [ApplicationStringTypeName], [ApplicationStringTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[ApplicationStringTypeIndex],Source.[ApplicationStringTypeCode],Source.[ApplicationStringTypeName],Source.[ApplicationStringTypeGuid],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[ApplicationStringTypeIndex],
   inserted.[ApplicationStringTypeIndex],
   deleted.[ApplicationStringTypeCode],
   inserted.[ApplicationStringTypeCode],
   deleted.[ApplicationStringTypeName],
   inserted.[ApplicationStringTypeName],
   deleted.[ApplicationStringTypeGuid],
   inserted.[ApplicationStringTypeGuid],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblApplicationStringTypeRefData;

SELECT @ApplicationStringTypeRefDataInserted = COUNT(*) FROM @tblApplicationStringTypeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ApplicationStringTypeRefDataUpdated = COUNT(*) FROM @tblApplicationStringTypeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ApplicationStringTypeRefDataDeleted = COUNT(*) FROM @tblApplicationStringTypeRefData WHERE ActionType IN ( 'DELETE' )

IF (@ApplicationStringTypeRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ApplicationStringTypeRefDataInserted) + ' NEW RECORDS INSERTED INTO [lookup].[tblApplicationStringType] **'
	PRINT ''
END

IF (@ApplicationStringTypeRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ApplicationStringTypeRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [lookup].[tblApplicationStringType] **'
	PRINT ''
	SELECT * FROM @tblApplicationStringTypeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
