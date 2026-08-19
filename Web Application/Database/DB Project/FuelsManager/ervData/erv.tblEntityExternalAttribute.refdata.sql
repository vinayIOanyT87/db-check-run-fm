SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [erv].[tblEntityExternalAttribute]'
PRINT ''

DECLARE @ErvExtAttributeInserted bigint
DECLARE @ErvExtAttributeUpdated bigint
DECLARE @ErvExtAttributeDeleted bigint

SET @ErvExtAttributeInserted = 0
SET @ErvExtAttributeUpdated = 0
SET @ErvExtAttributeDeleted = 0

DECLARE @tblEntityExternalAttributeRefData TABLE
(
	[ActionType] VARCHAR (50),
	[OldEntitySegmentTemplateGuid] UNIQUEIDENTIFIER,
	[EntitySegmentTemplateGuid] UNIQUEIDENTIFIER,
	[OldInternalFieldName] NVARCHAR(100),
	[InternalFieldName] NVARCHAR(100),
	[OldRelationshipTableName] VARCHAR(250),
	[RelationshipTableName] VARCHAR(250),
	[OldRelationshipName] NVARCHAR(100),
	[RelationshipName] NVARCHAR(100),
	[OldCreatedDate] DATETIMEOFFSET(7),
	[CreatedDate] DATETIMEOFFSET(7),
	[OldCreatedBy] [dbo].[udtUserID],
	[CreatedBy] [dbo].[udtUserID],
	[OldUpdatedDate] DATETIMEOFFSET(7),
	[UpdatedDate] DATETIMEOFFSET(7),
	[OldUpdatedBy] [dbo].[udtUserID],
	[UpdatedBy] [dbo].[udtUserID]
);


MERGE INTO [erv].[tblEntityExternalAttribute] AS Target
USING (VALUES

	(N'6215d201-d83c-481f-8455-e47dc7b2929e', N'39e9bb24-0e4a-435a-8b35-bcfd8c9cd44b', NULL, N'[map].[tblQualificationEquipmentTagAndLicenseToEquipment]', N'Tags and Licences', N'11/28/2012 12:35:34 PM -05:00', N'', N'11/28/2012 12:35:34 PM -05:00', N''),
	(N'498bce52-62b0-4e48-ae84-f59793359e47', N'39e9bb24-0e4a-435a-8b35-bcfd8c9cd44b', NULL, N'[map].[tblQualificationEquipmentTestAndInspectionToEquipment]', N'Tests and Inspections', N'11/28/2012 12:35:34 PM -05:00', N'', N'11/28/2012 12:35:34 PM -05:00', N''),
	(N'a54a2688-79f9-48d0-b41b-5212306abb40', N'39e9bb24-0e4a-435a-8b35-bcfd8c9cd44b', N'ProductGuid', N'[dbo].[tblProducts]', N'Product', N'11/28/2012 12:35:34 PM -05:00', N'', N'11/28/2012 12:35:34 PM -05:00', N''),
	(N'2dd80b3e-7cae-4d9c-acb3-7760a3f0c030', N'39e9bb24-0e4a-435a-8b35-bcfd8c9cd44b', N'CompanyGuid', N'[dbo].[tblCompanies]', N'Company', N'11/28/2012 12:35:34 PM -05:00', N'', N'11/28/2012 12:35:34 PM -05:00', N''),
	(N'31c817ae-fada-4d1b-b0a0-437f96c69b99', N'39e9bb24-0e4a-435a-8b35-bcfd8c9cd44b', N'FuelCardGuid', N'[dbo].[tblFuelsCards]', N'Fuel Card', N'11/28/2012 12:35:34 PM -05:00', N'', N'11/28/2012 12:35:34 PM -05:00', N''),
	(N'922adb48-4047-49aa-89de-d29da2c7e8ec', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[dbo].[tblEquipment]', N'Equipment', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'9960ca1d-85eb-4872-afdd-6c00db5181c2', N'44642d4c-6cdd-4bde-b246-68edc01a064f', N'CustomerBillToTypeApplicationStringGuid', N'[dbo].[tblApplicationString]', N'Bill To Type', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'de37839a-49e8-4111-934f-df234d78d971', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblCompanyAuthorizedCarrierToCompany]', N'AuthorizedShipTo', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'881c637b-d20b-4d4d-971d-fa216dc133b5', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblCompanyPersonnelAssignedToCompany]', N'Drivers', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'00e5a726-5f24-4243-aa4d-0ef047e1f396', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblProductToUnavailableInventoryCompany]', N'UnavailableInventories', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'745dba6a-8953-4810-9db7-245221c1b883', N'44642d4c-6cdd-4bde-b246-68edc01a064f', N'ShipperTypeApplicationStringGuid', N'[dbo].[tblApplicationString]', N'Shipper Type', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'4596f6b5-44cd-47c0-81f6-583cdd960853', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblProductToCompany]', N'ShipToAuthorizedProducts', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'ec42f81e-78bf-42bf-ad8d-23e709164ff7', N'44642d4c-6cdd-4bde-b246-68edc01a064f', N'CustomerShipToTypeApplicationStringGuid', N'[dbo].[tblApplicationString]', N'Ship To Type', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'2ea44e7d-58a5-41d7-b8e7-d47312c935d6', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblCompanyAuthorizedCarrierToCompany]', N'AuthorizedCarriers', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'199a6cab-fe3b-443f-9265-7f0ccf53f1f6', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[map.tblProductToSupplierProductCompany]', N'SupplierAuthorizedProducts', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'bf08c828-b388-4561-8e74-6fa712772dc1', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[dbo].[tblScheduleCompanyAccess]', N'AccessSchedule', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'81e22da0-d00d-4c01-bb20-e9bcdceca2d8', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblQualificationCompanyCertificateAndPermitToCompany]', N'CertificatesAndPermits', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'c3b095e4-8f7a-4557-a59e-721859bbf159', N'44642d4c-6cdd-4bde-b246-68edc01a064f', NULL, N'[map].[tblCompanyCompanyToUserGroup]', N'UserGroups', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'e967772a-08cc-4fc2-8bca-6ba85a88c065', N'44642d4c-6cdd-4bde-b246-68edc01a064f', N'IATAGuid', N'[dbo].[tblIATA]', N'DeliveryLocation', N'3/12/2013 1:25:54 PM -04:00', N'', N'3/12/2013 1:25:54 PM -04:00', N''),
	(N'07f6c3b1-07d6-4d25-b836-8ddbc263f337', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', N'TrackingProductGuid', N'[dbo].[tblProducts]', N'Tracking Product', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'1c234db3-23c7-4aa4-a08e-ec58ea7ce87f', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', NULL, N'[map].[tblProductToCompany], [map].[tblProductToCompanyGroup]', N'AuthorizedCustomers', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'28d33e94-8400-4082-b6ed-f3a803106e43', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', NULL, N'[map].[tblApplicationStringToProductMessage], [map].[tblApplicationStringToDotHazardousMessage]', N'Messages', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'f9ac745a-a8e8-4b9a-aa31-42ab6067bdb4', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', N'ComponentTolerance', N'[dbo].[tblProducts]', N'Allowable Tolerance', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'776506f1-4e83-4849-ad85-70f289c1391b', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', NULL, N'[map].[tblProductToUnavailableInventoryCompany]', N'UnavailableInventories', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'9ffb5408-81d3-43d2-b1de-2f7e6869d939', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', NULL, N'[map].[tblProductToSupplierProductCompany]', N'SupplierAuthorizedProducts', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'4bd576af-9e99-4838-bd86-fd47480c782e', N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', NULL, N'[map].[tblProductToTransactionAliasExclusion]', N'TransactionAliasExclusion', N'5/28/2013 10:23:21 AM -04:00', N'', N'5/28/2013 10:23:21 AM -04:00', N''),
	(N'41e80dcd-554a-406d-a41e-8e3229a74b8a', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', N'SupervisorPersonnelGuid', N'[dbo].[tblPersonnel]', N'Supervisor', N'6/19/2013 12:26:37 PM -04:00', N'', N'6/19/2013 12:26:37 PM -04:00', N''),
	(N'0e5310f4-61cf-4c49-8Ab6-f4682ee130db', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', NULL, N'[map].[tblCompanyPersonnelAssignedToCompany]', N'Carrier', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'fe1b5765-9ab6-493c-b2da-527260d2bd27', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', N'AssignedEquipmentGuid', N'[dbo].[tblEquipment]', N'AssignedEquipment', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'833a207a-d409-44e5-bbb8-8257cecacedb', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', NULL, N'[map].[tblQualificationPersonQualificationToPerson]', N'Qualification', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'7ffda6fd-76c5-4fd5-94e4-6ffcdd3bfce5', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', NULL, N'[map].[tblQualificationPersonLicenseToPerson]', N'License', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'076501f4-73d7-4dfd-bfce-fd46fcbceb6f', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', NULL, N'[map].[tblQualificationPersonTrainingToPerson]', N'Training', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'fde4ab3e-f161-4b48-8b7e-1f8739c404fc', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', NULL, N'[map].[tblPersonnelToRole]', N'Roles', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'8cadbb66-9c84-4088-b96b-2926ccc090ce', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', NULL, N'[dbo].[tblSchedulePersonnelAccess]', N'Schedule', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'49414380-3b13-484c-aff6-4ad6ee230a6f', N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', N'UserGuid', N'[dbo].[tblUsers]', N'User', N'6/19/2013 12:26:38 PM -04:00', N'', N'6/19/2013 12:26:38 PM -04:00', N''),
	(N'cb277f41-cf9e-405b-919b-c480385a956e', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[map].[tblGroupToTransactionAlias]', N'UserGroups', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'8e25c2a8-0f26-471f-9a51-742c5b117248', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[dbo].[tblTransactionAliasFields]', N'Fields', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'f5d0e48f-9f82-415a-8df0-57954bc8aee5', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[dbo].[tblUserDataFieldTransactionAlias], [dbo].[tblUserDataListValueTransactionAlias], [dbo].[tblUserDataFieldTransactionAliasLineItem], [dbo].[tblUserDataListValueTransactionAliasLineItem]', N'UserData', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'69e81351-e855-4c6f-9df5-001c10f82768', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[dbo].[tblTransactionAliasFields]', N'FieldOrder', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'4966fe06-990b-4181-bb6d-0344d37b77d6', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[map].[tblProductToTransactionAliasExclusion]', N'Products', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'2cfb4871-f4bb-45fc-9a0c-ecae6cf859ce', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[map].[tblTransactionAliasToStatus]', N'Statuses', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'294be689-cbd9-4b82-bd16-1500a4514639', N'a7aae550-f952-41f1-b556-c1533882612b', NULL, N'[map].[tblAssociatedTransactionAliases]', N'Associations', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N''),
	(N'5750b7ae-db3a-4d7b-8378-6215ef4f8927', N'a7aae550-f952-41f1-b556-c1533882612b', N'AssociatedTransactionAliasGuid', N'[dbo].[tblTransactionAliases]', N'AssociatedTransactionAlias', N'7/2/2013 10:14:11 AM -04:00', N'', N'7/2/2013 10:14:11 AM -04:00', N'')
 ) AS Source ([EntityExternalAttributeGuid], [EntitySegmentTemplateGuid], [InternalFieldName], [RelationshipTableName], [RelationshipName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[EntityExternalAttributeGuid] = Source.[EntityExternalAttributeGuid])
WHEN MATCHED AND EXISTS (SELECT Target.[EntitySegmentTemplateGuid]
						, Target.[InternalFieldName]
						, Target.[RelationshipTableName]
						, Target.[RelationshipName]
						, Target.[CreatedDate]
						, Target.[CreatedBy]
						, Target.[UpdatedDate]
						, Target.[UpdatedBy] 
						EXCEPT 
						SELECT Source.[EntitySegmentTemplateGuid]
						, Source.[InternalFieldName]
						, Source.[RelationshipTableName]
						, Source.[RelationshipName]
						, Source.[CreatedDate]
						, Source.[CreatedBy]
						, Source.[UpdatedDate]
						, Source.[UpdatedBy]) THEN
	UPDATE SET [EntitySegmentTemplateGuid] = Source.[EntitySegmentTemplateGuid]
				, [InternalFieldName] = Source.[InternalFieldName]
				, [RelationshipTableName] = Source.[RelationshipTableName]
				, [RelationshipName] = Source.[RelationshipName]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([EntityExternalAttributeGuid], [EntitySegmentTemplateGuid], [InternalFieldName], [RelationshipTableName], [RelationshipName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[EntityExternalAttributeGuid], Source.[EntitySegmentTemplateGuid], Source.[InternalFieldName], Source.[RelationshipTableName], Source.[RelationshipName], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[EntitySegmentTemplateGuid],
   inserted.[EntitySegmentTemplateGuid],
   deleted.[InternalFieldName],
   inserted.[InternalFieldName],
   deleted.[RelationshipTableName],
   inserted.[RelationshipTableName],
   deleted.[RelationshipName],
   inserted.[RelationshipName],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblEntityExternalAttributeRefData;

SELECT @ErvExtAttributeInserted = COUNT(*) FROM @tblEntityExternalAttributeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ErvExtAttributeUpdated = COUNT(*) FROM @tblEntityExternalAttributeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ErvExtAttributeDeleted = COUNT(*) FROM @tblEntityExternalAttributeRefData WHERE ActionType IN ( 'DELETE' )

IF (@ErvExtAttributeInserted = 0 AND @ErvExtAttributeUpdated = 0)
BEGIN
	PRINT '** No Changes Detected for [erv].[tblEntityExternalAttribute] **'
	PRINT ''
END

IF (@ErvExtAttributeInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ErvExtAttributeInserted) + ' NEW RECORDS INSERTED INTO [erv].[tblEntityExternalAttribute] **'
	PRINT ''
END

IF (@ErvExtAttributeUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ErvExtAttributeUpdated) + ' EXISTING RECORDS UPDATED IN [erv].[tblEntityExternalAttribute] **'
	PRINT ''
	SELECT * FROM @tblEntityExternalAttributeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
