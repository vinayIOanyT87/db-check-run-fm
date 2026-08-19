SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [lookup].[tblStandardFieldType]'
PRINT ''

DECLARE @StandardFieldTypeRefDataInserted bigint
DECLARE @StandardFieldTypeRefDataUpdated bigint
DECLARE @StandardFieldTypeRefDataDeleted bigint

SET @StandardFieldTypeRefDataInserted = 0
SET @StandardFieldTypeRefDataUpdated = 0
SET @StandardFieldTypeRefDataDeleted = 0

DECLARE @tblStandardFieldTypeRefData TABLE
(
	[ActionType] VARCHAR (50),
	[StandardFieldTypeIndex] [INT],
	[OldStandardFieldTypeIndex] [INT],
	[StandardFieldTypeCode] [nvarchar](100),
	[OldStandardFieldTypeCode] [nvarchar](100),
	[StandardFieldTypeName] [nvarchar](100),
	[OldStandardFieldTypeName] [nvarchar](100),
	[StandardFieldTypeGuid]  [UNIQUEIDENTIFIER],
	[OldStandardFieldTypeGuid]  [UNIQUEIDENTIFIER],
	[CreatedDate] [datetimeoffset](7),
	[OldCreatedDate] [datetimeoffset](7),
	[CreatedBy] [dbo].[udtUserID],
	[OldCreatedBy] [dbo].[udtUserID],
	[UpdatedDate] [datetimeoffset](7),
	[OldUpdatedDate] [datetimeoffset](7),
	[UpdatedBy] [dbo].[udtUserID],
	[OldUpdatedBy] [dbo].[udtUserID]
);

; MERGE INTO [lookup].[tblStandardFieldType] AS Target
USING (VALUES
(1, N'BEGIN_INVENTORY', N'BEGIN INVENTORY', N'f9d18386-9ed4-4044-911e-b7728ac4ee88', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(2, N'BOOK_INVENTORY', N'BOOK INVENTORY', N'a9f3356e-05b0-4b85-93da-837e29764045', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(3, N'INVENTORY_DATE', N'INVENTORY DATE', N'6b1cf6ec-34d5-42e0-8035-f5f4a76344c5', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(4, N'ASSET_ID', N'ASSET ID', N'9054fc77-d705-4ec6-aa1e-a3f20dd6361e', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(5, N'METER_START', N'METER START', N'b454edc8-99c2-49a0-bc28-ee628f3d333a', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(6, N'METER_STOP', N'METER STOP', N'f51eadd9-b6f5-4cbe-9b2a-4377d400da5e', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(7, N'DIFFERENTIAL', N'DIFFERENTIAL', N'ce164c9e-cb05-4b0a-911a-9e131eb0583a', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(8, N'TOTAL_VOLUME', N'TOTAL VOLUME', N'34acbf3f-9fe8-4b59-9c4c-7b4ea3815f31', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(9, N'VARIANCE', N'VARIANCE', N'e901a0a5-2c6c-466a-908c-b5912dcd9458', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(10, N'TRANSACTION_DATE', N'TRANSACTION DATE', N'c06745a5-66c9-469a-9500-3a7a3f6c244a', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(11, N'TRANSACTION_ID', N'TRANSACTION ID', N'540dac37-5a49-4827-b031-b706a20208e1', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(12, N'PHYSICAL_INVENTORY', N'PHYSICAL INVENTORY', N'5bb1f014-26da-4792-b254-7c61f63af20f', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(13, N'TOTAL_VARIANCE', N'TOTAL VARIANCE', N'0f619a1b-2363-4c28-86b3-4a4062e8c3c6', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(14, N'RECEIPTS', N'RECEIPTS', N'4a8469c4-08d4-4f56-bfae-e4e3b9dab274', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(15, N'ADJUSTMENTS', N'ADJUSTMENTS', N'60847da7-07a6-48b9-80a6-38838d568c44', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(16, N'DEFUELS', N'DEFUELS', N'3e83e7b2-bf03-4a08-b174-0922028d36e2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(17, N'ISSUES', N'ISSUES', N'df1efb2d-9ab9-4409-9cba-c62ddc0cd442', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(18, N'LOAD_RACK', N'LOAD RACK', N'bdb4694c-7655-4496-b06d-299a0764c08c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(19, N'REQUEST', N'REQUEST', N'e19592fb-24bb-4666-a7bf-1ed888ad5cff', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(20, N'ROTATION', N'ROTATION', N'd9e49688-6f86-4692-acba-952733ade1f8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(21, N'TRANSFERS', N'TRANSFERS', N'be543e15-fcdd-4a00-9b22-04fd212e4efe', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(22, N'TRANSACTION_TYPE', N'TRANSACTION TYPE', N'2d2705b1-92c3-4180-9ea1-46a5789a519b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(23, N'VOLUME', N'VOLUME', N'9e02c3f1-a83b-406b-9117-0806774a4951', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(24, N'CONSUMER', N'CONSUMER', N'a918d8bb-16d5-488c-868c-69681e51461b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(25, N'DESTINATION_REGISTRATION_ID', N'DESTINATION REGISTRATION ID', N'38f4f0c5-e81b-4da6-a58a-8429ddccc35c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(26, N'SERIAL_NUMBER', N'SERIAL NUMBER', N'234c358f-5d59-457e-82c4-30b884e89757', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(27, N'PRODUCT', N'PRODUCT', N'a7e91da3-812b-47ab-bd6e-40dbce1cbd6c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(28, N'LOCATION', N'LOCATION', N'dabce6f7-b3f6-4302-bb94-510f640df6b8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(29, N'INVENTORY', N'INVENTORY', N'57d318b8-af28-4a5d-a38e-7db7fcd5a7b3', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(30, N'TEMPERATURE', N'TEMPERATURE', N'76ed71b2-42dd-4e8c-9630-5e31abd3c976', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(31, N'DENSITY', N'DENSITY', N'8e6475dc-a446-40a0-859b-01eed5a0b628', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(32, N'VCF', N'VCF', N'4ce83096-908b-4d0f-aa14-91064659b19b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(33, N'SITE', N'SITE', N'ecfc15ff-168e-4345-907c-6fcde0fa29a7', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(34, N'BILLED_VOLUME', N'BILLED VOLUME', N'810a7be7-bbb4-430f-81c8-709435d33208', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(35, N'MEASURED_VOLUME', N'MEASURED VOLUME', N'b2424178-db21-4684-b8ea-c6c7b0007cdc', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(36, N'BILL_OF_LADING_NUMBER', N'BILL OF LADING NUMBER', N'fdb21ad5-0e52-4d86-8950-20e7b3bb0a9b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(37, N'BOOK_RECEIPTS', N'BOOK RECEIPTS', N'226542c4-abaf-4486-b670-57de6601e32d', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(38, N'ASSIGNED', N'ASSIGNED', N'd53c9cdb-8007-4229-b760-16ba49eab90b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(39, N'REMAINING', N'REMAINING', N'42158763-8ef4-4c43-9e70-ad6ceafe3a43', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(40, N'OWNER', N'OWNER', N'ebebf225-556b-4c8b-ba57-7a74ed52e0f8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(41, N'MANAGER', N'MANAGER', N'6d46655e-4dd3-403a-931e-129ca9ad7e18', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(42, N'TYPE_MAX', N'TYPE MAX', N'97dfaa4c-1ec4-4ab4-aae8-5f8958c6bdba', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(43, N'TOTAL_PHYSICAL_INVENTORY', N'TOTAL PHYSICAL INVENTORY', N'2b534098-7f6c-44ae-a4aa-2a2ba697b023', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(44, N'BILLTOID', N'BILLTOID', N'3087866a-3f4f-4e4c-9f44-33da75ee7ca9', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(45, N'SHIPTOID', N'SHIPTOID', N'09787820-7a5f-41ec-a982-a74dfc789c69', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(46, N'TRANSACTION_ALIAS', N'TRANSACTION ALIAS', N'4d9fd368-ad85-4bb7-9c37-45d9c4186a35', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(47, N'DOCUMENT_NUMBER', N'DOCUMENT NUMBER', N'2c854238-e415-44af-aa35-e4f29363f851', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(48, N'PO_NUMBER', N'PO NUMBER', N'b1d7725b-5c00-4c83-8d39-b7bd91f7325d', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(49, N'SCHEDULED_DATE', N'SCHEDULED DATE', N'dbd452ea-6095-4317-b896-13f5fe57d7f0', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(50, N'ORDER_STATUS', N'ORDER STATUS', N'a62e52ab-1b62-4aa9-8b5e-9ad20f459f23', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(51, N'EFFECTIVE_DATE', N'EFFECTIVE DATE', N'4887ed24-44a6-4b80-aa41-93d6ad58ebd9', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(52, N'EXPIRATION_DATE', N'EXPIRATION DATE', N'1e0b535e-cbc8-4721-ac0a-5b2f8d0e3306', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(53, N'ETA', N'ETA', N'875d4cd0-11b5-443d-84a8-77c1a7f71f6c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(54, N'BOL_NUMBER', N'BOL NUMBER', N'22025771-6452-416c-a510-720d2ea697bc', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(55, N'SHIPPER', N'SHIPPER', N'478a7df9-7db7-4864-9097-c334b044b872', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(56, N'BOL_STATUS', N'BOL STATUS', N'b1a89c5d-8bc5-4f09-8c2a-5bd395020d25', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(57, N'BOL_DATE_TIME', N'BOL DATE TIME', N'e6d4cdc2-78e1-49b0-ad90-f42013202d91', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(58, N'BOL_CARRIER', N'BOL CARRIER', N'1f50cd92-5c6b-4aaa-b1c2-3dc07dd82c63', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(59, N'BOL_MANAGER', N'BOL MANAGER', N'1d02d93e-8d1c-4c12-9f8d-7b21b4db0016', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(60, N'BOL_OWNER', N'BOL OWNER', N'ce6d4657-c6b0-4cbb-a415-94e26c9cac98', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(61, N'TOTAL_ACTIVITY', N'TOTAL ACTIVITY', N'e4fb8937-5aac-48bc-90fe-21fed6dacec0', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(62, N'CLOSEOUT_DATE', N'CLOSEOUT DATE', N'af0f0853-0017-429e-840f-19b56c3c4de4', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(63, N'ESTIMATED_DATE_FROM', N'ESTIMATED DATE FROM', N'18a37ce2-b3bf-4091-ac19-701160bc4a7c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(64, N'ESTIMATED_DATE_TO', N'ESTIMATED DATE TO', N'1e44cdbb-c551-49a5-9bb9-03428b34c0e9', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(65, N'ORDER_CONFIRM_NUMBER', N'ORDER CONFIRM NUMBER', N'5b8e7133-178a-4ef3-b3a6-2fd30367df23', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(66, N'STANDING_OFFER_NUMBER', N'STANDING OFFER NUMBER', N'2d2fccb4-9c76-40b9-92d1-2acb06ac86b6', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(67, N'REQUIRED_DATE', N'REQUIRED DATE', N'9696ce43-fe0f-49f8-90f6-ccb830c3b0d1', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(68, N'SUPPLIER', N'SUPPLIER', N'3a8ce9ed-42a3-497d-b158-3f6e16a46068', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(69, N'REQUESTED_DELIVERY_DATE', N'REQUESTED DELIVERY DATE', N'a3525ccf-dc56-44fd-9253-f67c47c1a489', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(70, N'SHIPMENT_NUMBER', N'SHIPMENT NUMBER', N'553a676d-bdb6-410e-af67-066d15ee2c69', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(71, N'USER_DATA_1', N'USER DATA 1', N'bae55adc-5431-4b14-b5d4-59afce30bd69', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(72, N'USER_DATA_2', N'USER DATA 2', N'e8d4f213-f647-4281-8d83-05920ce36596', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(73, N'USER_DATA_3', N'USER DATA 3', N'116f9c17-cc50-4a7a-92ba-e9eed75e9384', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(74, N'USER_DATA_4', N'USER DATA 4', N'10d19c7e-8c0c-47be-811b-a05a01bad9c9', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(75, N'USER_DATA_5', N'USER DATA 5', N'9aaf34b3-b4a3-4b92-b0f8-95007d9ff739', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(76, N'USER_DATA_6', N'USER DATA 6', N'f936c534-e136-4eb4-9a22-01bf2150faa8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(77, N'USER_DATA_7', N'USER DATA 7', N'5ceae67a-8912-4ff5-8392-8b55d063ddb0', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(78, N'USER_DATA_8', N'USER DATA 8', N'2f573a3a-157a-4cb2-bd2b-e353587cd8c1', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(79, N'USER_DATA_9', N'USER DATA 9', N'7a18694f-321b-42f6-91e8-a80911439ed0', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(80, N'USER_DATA_10', N'USER DATA 10', N'48578405-eddf-4b71-8d3c-5fba06d78d98', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(81, N'USER_DATA_11', N'USER DATA 11', N'49b61767-7fa9-486b-aa90-0c527a738ecf', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(82, N'USER_DATA_12', N'USER DATA 12', N'9851aef5-0a1c-447a-af85-d6da32779bf5', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(83, N'USER_DATA_13', N'USER DATA 13', N'4dadb7b3-c95d-4df1-9ee7-fbf75b221df7', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(84, N'USER_DATA_14', N'USER DATA 14', N'62181926-61ae-458e-bcf1-621d79230a89', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(85, N'USER_DATA_15', N'USER DATA 15', N'919bad45-3be3-4ded-8555-9283f724d1cb', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(86, N'USER_DATA_16', N'USER DATA 16', N'94ddcd5c-91d2-472c-9d1a-700fe39367a2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(87, N'USER_DATA_17', N'USER DATA 17', N'83918fe4-a83d-44a7-8a51-eca8d03747dd', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(88, N'USER_DATA_18', N'USER DATA 18', N'2e10f02b-1aca-44fb-9a0f-d35b93f64084', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(89, N'USER_DATA_19', N'USER DATA 19', N'aa66d965-2ea2-4ec0-a084-dae840483214', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(90, N'USER_DATA_20', N'USER DATA 20', N'1930b086-81f2-46d7-85f4-74486187ad18', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(91, N'USER_DATA_21', N'USER DATA 21', N'7f31ccc9-d183-4ce4-beee-a5c253609260', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(92, N'USER_DATA_22', N'USER DATA 22', N'9de0b70a-eab3-44c2-b544-74efec0134eb', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(93, N'USER_DATA_23', N'USER DATA 23', N'8e43b7e1-176f-4076-8fae-83ae75277e07', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(94, N'USER_DATA_24', N'USER DATA 24', N'fb6bf1fa-5ba3-4823-8f01-0a117c138a0d', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(95, N'OPERATORID', N'OPERATORID', N'2d3422a0-12b9-4f11-ae90-0b4384978176', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(96, N'DESTEQUIPMENT1ID', N'DESTEQUIPMENT1ID', N'36a85570-4c57-4998-85f0-017caf6abc82', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(97, N'DESTEQUIPMENT2ID', N'DESTEQUIPMENT2ID', N'9001cadc-170f-4045-81e3-845f30c4f9a3', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(98, N'DESTEQUIPMENT3ID', N'DESTEQUIPMENT3ID', N'e73bca22-1bf5-44c5-9715-f8468b8761b2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(99, N'EXCISE', N'EXCISE', N'9518e518-f991-4a01-aa85-71c157de1388', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(100, N'COST_CENTRE_CODE', N'COST CENTRE CODE', N'2986d327-b25c-4fea-aedb-47183caba5b8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(101, N'GST', N'GST', N'983cae4f-ceb3-4300-a4c1-3b5302e029cf', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(102, N'INVOICE_NUMBER', N'INVOICE NUMBER', N'f8bffda8-30ed-417f-b6ba-aa12d88cf487', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(103, N'VOUCHER_NUMBER', N'VOUCHER NUMBER', N'31b79c39-b67c-4caf-9eb7-006e40c3d5a2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(104, N'ACCOUNT_CODE', N'ACCOUNT CODE', N'fdaa12d6-c73c-4cd4-bd82-4968048dcd46', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(105, N'LEGACY_NUMBER', N'LEGACY NUMBER', N'88b9e1d5-916a-4598-99f2-71e6be047543', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(106, N'CONTACT_INFO', N'CONTACT INFO', N'564cc0fc-3900-42c1-9339-07106404ea36', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(107, N'CONTACT_SURNAME', N'CONTACT SURNAME', N'5e807c22-f37d-4dca-8b03-b1b93ff40d13', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(108, N'CONTACT_FIRST_NAME', N'CONTACT FIRST NAME', N'2532d10a-238b-4697-b95c-a6eab6753887', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(109, N'PRODUCT_PRICE', N'PRODUCT PRICE', N'bbb5e4b9-33d5-404b-b023-55f08183a313', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(110, N'GROSS_QUANTITY', N'GROSS QUANTITY', N'018ee0ca-00b2-41ab-b9aa-cc9105ea987a', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(111, N'NET_QUANTITY', N'NET QUANTITY', N'6788af23-718d-47c2-a677-b40068733c3d', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(112, N'BATCH_NUMBER', N'BATCH NUMBER', N'a59fbf96-5660-47c3-8704-c83f7e04edc8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(113, N'ORDER_NUMBER', N'ORDER NUMBER', N'c79c58ff-fba2-4f9f-9b28-f729308973eb', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(114, N'PAYMENT_NUMBER', N'PAYMENT NUMBER', N'05b1ae85-880c-4ebf-b530-828ae41f5dab', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(115, N'TOTAL_AMOUNT', N'TOTAL AMOUNT', N'c1c6f992-4bcd-4660-93df-e4bf81c9806c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(116, N'REBATE_FLAG', N'REBATE FLAG', N'c584a36e-c1a2-4b88-a686-6a9f4015f181', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(117, N'LINE_ITEM_USER_DATA_01', N'LINE ITEM USER DATA 01', N'e46db888-828d-40f6-a777-57695fff33e5', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(118, N'LINE_ITEM_USER_DATA_02', N'LINE ITEM USER DATA 02', N'6e0713ff-5329-4628-979d-9163809fc678', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(119, N'LINE_ITEM_USER_DATA_03', N'LINE ITEM USER DATA 03', N'9598756a-93bd-4d94-9390-20b4bf41df96', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(120, N'LINE_ITEM_USER_DATA_04', N'LINE ITEM USER DATA 04', N'eada51d6-7a18-4b16-bc91-358fd45e6efd', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(121, N'LINE_ITEM_USER_DATA_05', N'LINE ITEM USER DATA 05', N'6cf8a604-9956-4578-b3bb-520708848c0b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(122, N'LINE_ITEM_USER_DATA_06', N'LINE ITEM USER DATA 06', N'687d9ecd-3ede-44df-9f7b-f2c88bc0c74f', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(123, N'LINE_ITEM_USER_DATA_07', N'LINE ITEM USER DATA 07', N'88997b66-ddbf-43ae-b112-95717bd835d3', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(124, N'LINE_ITEM_USER_DATA_08', N'LINE ITEM USER DATA 08', N'fcb87775-2436-4df2-9c35-662c3a3a624f', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(125, N'LINE_ITEM_USER_DATA_09', N'LINE ITEM USER DATA 09', N'd91a7491-ed94-4b26-a9e7-da74b22d8d81', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(126, N'LINE_ITEM_USER_DATA_10', N'LINE ITEM USER DATA 10', N'5d7a82b7-5262-4f75-9599-42bac6ac8de2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(127, N'LINE_ITEM_USER_DATA_11', N'LINE ITEM USER DATA 11', N'7380c1ff-53a4-4a82-8817-2e88a5bac2e2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(128, N'LINE_ITEM_USER_DATA_12', N'LINE ITEM USER DATA 12', N'600cbc13-1d6f-480c-8f4b-fbe41921a84c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(129, N'LINE_ITEM_USER_DATA_13', N'LINE ITEM USER DATA 13', N'2856ba21-d0e8-4de5-9823-1d9d1786321c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(130, N'LINE_ITEM_USER_DATA_14', N'LINE ITEM USER DATA 14', N'06ec642d-915d-4d51-88b1-013ce85969f4', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(131, N'LINE_ITEM_USER_DATA_15', N'LINE ITEM USER DATA 15', N'5aee2be6-740b-4436-b40c-f4df6b996dd2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(132, N'LINE_ITEM_USER_DATA_16', N'LINE ITEM USER DATA 16', N'5bdae9ce-0c88-495f-8b45-6481ff1937f6', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(133, N'LINE_ITEM_USER_DATA_17', N'LINE ITEM USER DATA 17', N'93d481fb-ff6d-422d-a7a1-a393eae18b89', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(134, N'LINE_ITEM_USER_DATA_18', N'LINE ITEM USER DATA 18', N'f0b2bb8a-1495-437a-9112-9b552fa81472', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(135, N'LINE_ITEM_USER_DATA_19', N'LINE ITEM USER DATA 19', N'bfa178c7-0522-49b8-88f7-c16d5839aae3', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(136, N'LINE_ITEM_USER_DATA_20', N'LINE ITEM USER DATA 20', N'0ed835de-6f12-47b7-bb5d-225a38370fa7', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(137, N'LINE_ITEM_USER_DATA_21', N'LINE ITEM USER DATA 21', N'3e8f3c7e-e275-465d-a7f2-6e0654f1c0a6', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(138, N'LINE_ITEM_USER_DATA_22', N'LINE ITEM USER DATA 22', N'635c7022-0abb-46a0-ae27-716560f4bd0c', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(139, N'LINE_ITEM_USER_DATA_23', N'LINE ITEM USER DATA 23', N'499a8086-8d72-4aff-a149-9889724268b3', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(140, N'LINE_ITEM_USER_DATA_24', N'LINE ITEM USER DATA 24', N'98492865-64ba-4e5e-a26a-2ae73e2a6eeb', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(141, N'MASSQUANTITY', N'MASSQUANTITY', N'ee607fc2-b8a2-4a1f-a58a-31472be3d7f3', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(142, N'GROSS_MANUALVALUE', N'GROSS MANUALVALUE', N'ef99fb47-1911-4d89-9b8b-b98308a705fe', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(143, N'NET_MANUALVALUE', N'NET MANUALVALUE', N'3c544d13-87d8-42cc-8bea-d8c3fa75e7db', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(144, N'MASS_MANUALVALUE', N'MASS MANUALVALUE', N'117da73d-26a9-4f6c-9d2e-cee2f0e2abd2', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(145, N'VCF_MANUALVALUE', N'VCF MANUALVALUE', N'b7455a66-de8a-416e-8bee-7a593cbb050b', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(146, N'ALTERNATIVE_NET_VOLUME', N'ALTERNATIVE NET VOLUME', N'f86f1e4b-10a9-400c-a4e7-9940673dc311', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(147, N'TOLERANCE', N'TOLERANCE', N'8b6d7bab-40f4-4976-bc80-4712d5d4a9b8', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(148, N'ALLOWED_GAIN_LOSS', N'ALLOWED GAIN LOSS', N'75373c46-e15b-4f08-baa2-faef437e4c6f', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(149, N'VARIANCE_PERCENTAGE', N'VARIANCE PERCENTAGE', N'1196a88a-75a0-46ee-bf45-c5a521f10245', N'6/18/2012 1:02:45 PM +00:00', N'Administrator', N'6/18/2012 1:02:45 PM +00:00', N'Administrator'),
(150, N'ROTATES_BACKWARDS', N'ROTATES BACKWARDS', N'b92c3d62-52a7-49ac-a6fb-32a00dc149b6', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(151, N'METER_TOTAL', N'METER TOTAL', N'6abfef8d-0d00-4fd2-95ae-bc61b03c473d', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(152, N'TRANSACTION_METER_TOTAL', N'TRANSACTION METER TOTAL', N'0b8cfbd1-074e-4e03-b67b-c756d4f969fc', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'03/08/2023 9:06:48 AM -04:00', N'Administrator'),
(153, N'METER_VARIANCE', N'METER VARIANCE', N'a883c840-ca04-4e8f-8371-2717891237b8', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(154, N'METER_RECONCILIATION_ERROR', N'METER RECONCILIATION ERROR', N'942702ee-1624-49f9-9d3e-be49cbadafb3', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(155, N'METER_SKIP', N'METER SKIP', N'b705505c-3d0d-4b77-861b-bc61ce6f525f', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(156, N'FLIGHT_NUMBER', N'FLIGHT NUMBER', N'27400e2b-3d1c-406e-be5c-f8565c572f79', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(157, N'TICKET_NUMBER', N'TICKET NUMBER', N'a3f654bb-786c-4de0-886a-09cd997b6069', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(158, N'STATION', N'STATION', N'7e71e672-adc3-4a5e-b47c-3f996d7f9ca2', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(159, N'METER_ID', N'METER ID', N'159c79c7-74c7-4e8c-9b72-f8b4b6cafec8', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(160, N'VIEW_DETAILS', N'VIEW DETAILS', N'6c049de2-1dd3-48db-a014-7218b8df8827', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator'),
(161, N'AUTO_DISTRIBUTION_RULE_ID', N'AUTO DISTRIBUTION RULE ID', N'c68a55dc-5dd2-4a02-9375-8cf58f4d0757', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(162, N'AUTO_DISTRIBUTION_RULE_DESCRIPTION', N'AUTO DISTRIBUTION RULE DESCRIPTION ', N'ca35cbac-fe8f-4529-b807-f509fe298d02', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(163, N'ENABLED', N'ENABLED', N'0fef6ef2-554f-4ca1-9c4d-f79124ae39b9', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(164, N'AUTO_DISTRIBUTION_DEFAULT_EOM', N'AUTO DISTRIBUTION DEFAULT EOM', N'15bc0a17-a219-4dba-be72-c2f82c446b2a', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(165, N'AUTO_DISTRIBUTION_TRANSACTION_ALIAS', N'AUTO DISTRIBUTION TRANSACTION ALIAS', N'6462cc07-0fd8-475d-a7c6-f84f690b57a1', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(166, N'REASON_CODE', N'REASON CODE', N'a31decb7-e6c2-48a1-975d-9dff9af8d0c6', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(167, N'MANAGERS', N'MANAGERS ', N'22fba402-b1d5-4522-881b-b0a8bc0cc5b0', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(168, N'PRODUCTS', N'PRODUCTS ', N'616aa7e8-392e-41dd-b16e-2551e9f13a97', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(169, N'AUTO_DISTRIBUTION_TRANSACTION_ALIASES', N'AUTO DISTRIBUTION TRANSACTION ALIASES', N'80cd7202-13d9-459b-a110-9c283da8de04', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(170, N'OWNERS', N'OWNERS ', N'1d2b4365-3e26-4792-8b4b-efdbf1c0ec49', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(171, N'SITE_GUID', N'SITE GUID ', N'57461487-51cc-4aa0-95e9-9048c929e8fa', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(172, N'IDENTITY_GUID', N'IDENTITYGUID ', N'598314a5-0235-487c-a2b1-793d22abcaf2', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator'),
(173, N'PACKAGE_MANUALVALUE', N'PACKAGE_MANUALVALUE', N'23daa036-c9b1-4032-83b0-849e43bb2780', N'6/19/2013 5:11:30 PM +00:00', N'Administrator', N'6/19/2013 5:11:30 PM +00:00', N'Administrator'),
(174, N'DELETE_FLAG', N'DELETE_FLAG', N'da537939-09fc-11ed-ab6d-e8f4082b401b', N'7/22/2022 5:11:30 PM +00:00', N'Administrator', N'7/22/2022 5:11:30 PM +00:00', N'Administrator'),
(175, N'REVERSAL_TYPE', N'REVERSAL TYPE', N'da53793a-09fc-11ed-ab6d-e8f4082b401b', N'7/22/2022 5:11:30 PM +00:00', N'Administrator', N'7/22/2022 5:11:30 PM +00:00', N'Administrator'),
(176, N'DESTINATION_SERIAL_NUMBER_1', N'DESTINATION_SERIAL_NUMBER_1', N'1801622A-4F57-49E9-AE70-B1755BB9B75C','2022-10-04','Administrator','2022-10-04','Administrator'), 
(177, N'DESTINATION_SERIAL_NUMBER_2', N'DESTINATION_SERIAL_NUMBER_2', N'DB6B46B2-5F05-48AA-B006-C89969244A33','2022-10-04','Administrator','2022-10-04','Administrator'),
(178, N'DESTINATION_SERIAL_NUMBER_3', N'DESTINATION_SERIAL_NUMBER_3', N'FADF2857-1BE6-48A0-B34B-2D911A77D045','2022-10-04','Administrator','2022-10-04','Administrator'),
(179, N'TRANSACTION_VOLUME_TOTAL', N'TRANSACTION VOLUME TOTAL', N'f86556b3-bdcf-11ed-b7aa-103d1cbd9c45','2023-03-08','Administrator','2023-03-08','Administrator'),
(180, N'VOLUME_VARIANCE', N'VOLUME VARIANCE', N'f86556b4-bdcf-11ed-b7aa-103d1cbd9c45','2023-03-08','Administrator','2023-03-08','Administrator')
) AS Source (	[StandardFieldTypeIndex],
					[StandardFieldTypeCode],
					[StandardFieldTypeName],
					[StandardFieldTypeGuid],
					[CreatedDate],
					[CreatedBy],
					[UpdatedDate],
					[UpdatedBy])
ON (Target.[StandardFieldTypeGuid] = Source.[StandardFieldTypeGuid])
WHEN MATCHED AND (Target.[StandardFieldTypeIndex] <> Source.[StandardFieldTypeIndex] 
					OR Target.[StandardFieldTypeCode] <> Source.[StandardFieldTypeCode]
					OR Target.[StandardFieldTypeName] <> Source.[StandardFieldTypeName]) THEN
	UPDATE SET 
				[StandardFieldTypeIndex] = Source.[StandardFieldTypeIndex]
				, [StandardFieldTypeCode] = Source.[StandardFieldTypeCode]
				, [StandardFieldTypeName] = Source.[StandardFieldTypeName]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT (	[StandardFieldTypeIndex],
				[StandardFieldTypeCode],
				[StandardFieldTypeName],
				[StandardFieldTypeGuid],
				[CreatedDate],
				[CreatedBy],
				[UpdatedDate],
				[UpdatedBy])
		VALUES (	Source.[StandardFieldTypeIndex],
					Source.[StandardFieldTypeCode],
					Source.[StandardFieldTypeName],
					Source.[StandardFieldTypeGuid],
					Source.[CreatedDate],
					Source.[CreatedBy],
					Source.[UpdatedDate],
					Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[StandardFieldTypeIndex],
   inserted.[StandardFieldTypeIndex],
   deleted.[StandardFieldTypeCode],
   inserted.[StandardFieldTypeCode],
   deleted.[StandardFieldTypeName],
   inserted.[StandardFieldTypeName],
   deleted.[StandardFieldTypeGuid],
   inserted.[StandardFieldTypeGuid],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblStandardFieldTypeRefData;

SELECT @StandardFieldTypeRefDataInserted = COUNT(*) FROM @tblStandardFieldTypeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @StandardFieldTypeRefDataUpdated = COUNT(*) FROM @tblStandardFieldTypeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @StandardFieldTypeRefDataDeleted = COUNT(*) FROM @tblStandardFieldTypeRefData WHERE ActionType IN ( 'DELETE' )

IF (@StandardFieldTypeRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @StandardFieldTypeRefDataInserted) + ' NEW RECORDS INSERTED INTO [lookup].[tblStandardFieldType] **'
	PRINT ''
END

IF (@StandardFieldTypeRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @StandardFieldTypeRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [lookup].[tblStandardFieldType] **'
	PRINT ''
	SELECT * FROM @tblStandardFieldTypeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
