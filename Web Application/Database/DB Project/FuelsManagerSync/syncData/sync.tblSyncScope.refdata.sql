SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [sync].[tblSyncScope]'
PRINT ''

DECLARE @SyncScopeInserted bigint
DECLARE @SyncScopeUpdated bigint
DECLARE @SyncScopeDeleted bigint

SET @SyncScopeInserted = 0
SET @SyncScopeUpdated = 0
SET @SyncScopeDeleted = 0

DECLARE @tblSyncScopeRefData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldID] NVARCHAR(80)
	,[ID] NVARCHAR(80)
	,[OldSyncScopeTypeIndex] BIGINT
	,[SyncScopeTypeIndex] BIGINT
	,[OldFriendlyName] NVARCHAR(100)
	,[FriendlyName] NVARCHAR(100)
	,[OldLongDescription] NVARCHAR(1024)
	,[LongDescription] NVARCHAR(1024)
	,[OldSyncProfileGuid] UNIQUEIDENTIFIER
	,[SyncProfileGuid] UNIQUEIDENTIFIER
	,[OldSyncOrder] INT
	,[SyncOrder] INT
	,[OldCreatedDate] DATETIMEOFFSET(7)
	,[CreatedDate] DATETIMEOFFSET(7)
	,[OldCreatedBy] [dbo].[udtUserID]
	,[CreatedBy] [dbo].[udtUserID]
	,[OldUpdatedDate] DATETIMEOFFSET(7)
	,[UpdatedDate] DATETIMEOFFSET(7)
	,[OldUpdatedBy] [dbo].[udtUserID]
	,[UpdatedBy] [dbo].[udtUserID]
	,[OldSyncSinglePass] BIT
	,[SyncSinglePass] BIT
);


; MERGE INTO [sync].[tblSyncScope] AS Target
USING (VALUES
	-- Create List of SyncScopes (Groups) Associated to the Default Profile
	(N'6039393B-8DC0-48C9-AF2C-C4089FB68A69', N'Level1a', 1, N'Lookup Data', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 1, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'58EE5F26-5596-4A90-A4AA-8D7F05A80E57', N'Level1b', 1, N'Lookup Data Part 2', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 2, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'A63E2EAC-A590-4A36-9EB5-ECD21DE6868B', N'Level1c', 1, N'Lookup Data Part 3', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 3, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'4BC3A6A9-5A8A-4C33-BEB9-CAC7A2C72000', N'Level1d', 1, N'Lookup Data Part 4', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 4, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'33EBE22B-C6FF-4D70-9D17-95E79B9D9FE0', N'Level1e', 1, N'Lookup Data Part 5', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 5, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'7167BD49-019E-4784-8228-D2D89549BE29', N'Level1f', 1, N'Lookup Data Part 6', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 6, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'1FB3DFE9-26F6-4691-93A4-CE46C429B7FD', N'Level2a', 1, N'Filtered Lookup Data', N'Lookup tables that are filtered/partitioned based on values in the AppCore Lookup Data.  Tables in this group must be able to have ALL foreign key relationships satisified by the AppCore reference tables.', N'83912BBD-113C-4824-9406-6DC3FED36590', 7, N'2012-11-15 08:25:06.0000000 -05:00', NULL, N'2012-11-15 08:25:06.0000000 -05:00', NULL, 0) 
	,(N'61CD9CB7-76BA-4650-AD01-6E07C345D6FF', N'Level2b', 4, N'Site List', N'Only the Site table because this scope type applies to all Sites in the Site Hierarchy.', N'83912BBD-113C-4824-9406-6DC3FED36590', 8, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'2E9572FA-6788-4407-B52B-7815EA78F3B7', N'Level2c', 4, N'Field Level Control Config', N'Only the Field Level Control Field Config table because this scope type applies to all Sites in the Site Hierarchy.', N'83912BBD-113C-4824-9406-6DC3FED36590', 9, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'ADCF0A1F-F14B-4215-992E-0A09846C688D', N'Level3a', 3, N'AppString/DataDictionaries', N'Additive profiles, alarm priorities, application strings, audit log, data dictionaries, email groups, etc...', N'83912BBD-113C-4824-9406-6DC3FED36590', 10, N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', 0) 
	,(N'A4F7984C-BD4B-457C-8851-6C841B9591E9', N'Level3b', 3, N'Gates/General/FuelCardLimit', N'Gates, General Configuration and Fuel Card Limits', N'83912BBD-113C-4824-9406-6DC3FED36590', 11, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'4BFAC150-B743-491F-8D7D-C5BD06A2E8C1', N'Level3c', 3, N'UserGroups/Meters/Products', N'User groups, IATA codes, meters, notes, products, etc..', N'83912BBD-113C-4824-9406-6DC3FED36590', 12, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'19C42ABD-FD11-45BF-8341-8D2D1BAAF1B8', N'Level3d', 3, N'Site Mappings/Users/Tests', N'Site Mappings, users list, test definitions, transaction aliases, etc..', N'83912BBD-113C-4824-9406-6DC3FED36590', 13, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'4A89A33F-8619-4BC3-B1A9-D94F4D492E1E', N'Level3e', 3, N'Mobile Device Profile', N'Only the mobile device profile table because of the large number of columns.', N'83912BBD-113C-4824-9406-6DC3FED36590', 14, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'354BAC9B-9B88-4366-956F-B33C78574AEF', N'Level4a', 3, N'AppStrings/Alarms', N'Alarm and Events, application string mappings', N'83912BBD-113C-4824-9406-6DC3FED36590', 15, N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', 0) 
	,(N'CF101EF6-03E2-469F-B874-D1F8DBE33551', N'Level4b', 3, N'Archived Users/Companies', N'Archived users, auto dist rules, companies, entity to site mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 16, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'14504467-BB63-4EE8-A004-A5D16965D31B', N'Level4c', 3, N'Entity to Site Maps', N'Entity to site mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 17, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'50093FC6-1086-425A-8C9C-81A4B369E32B', N'Level4d', 3, N'Entity to Site Maps', N'Entity to site mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 18, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'79C95D34-4AE0-4A0B-B412-C1819603D7C1', N'Level4e', 3, N'Entity to Site Maps', N'Entity to site mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 19, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'9229004A-E5ED-422D-9041-278E8F19E6D5', N'Level4f', 3, N'EntitySiteMap/UserDataConfig', N'Group Mappings / Entity Groups to Site Maps / Entity To Site Maps for Entities directly referenced by the Site.', N'83912BBD-113C-4824-9406-6DC3FED36590', 20, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'AFBAB0C3-A494-4EDC-A271-653D743235B5', N'Level4g', 3, N'ListViews/Group Rights', N'List views, group rights, equipment types, process variable to site mappings, etc.', N'83912BBD-113C-4824-9406-6DC3FED36590', 21, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'99BAB581-5BED-488A-B4F4-32E1CA6CEBDE', N'Level4h', 3, N'User Data Fields/User To Group Map', N'User data field mappings, user group mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 22, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'BABDC000-7176-465D-89C3-5B9D73EB4C7C', N'Level4i', 3, N'Export Results', N'Export Results and Details', N'83912BBD-113C-4824-9406-6DC3FED36590', 23, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 1) 
	,(N'B677EA0E-56D1-4D11-9041-B63D326544DB', N'Level5a', 3, N'Airplane/CompanyRoles', N'Airplane tanks, application string to footnote mappings, company to entity mappings, company roles.', N'83912BBD-113C-4824-9406-6DC3FED36590', 24, N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', 0) 
	,(N'5BC70BCF-0753-4E1A-907E-09139BAAB98B', N'Level5b', 3, N'Entity to Site Maps', N'Entity to site maps for alarms, company, equipment types, user data, fuel cards group to ledger mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 25, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'72332B71-FF76-4CD5-90C0-C625356F4C5C', N'Level5c', 3, N'Auto Dist Maps', N'List view fields, auto distribution mappings, qualification mappings', N'83912BBD-113C-4824-9406-6DC3FED36590', 26, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'C320FFCE-18F6-47A5-93CA-676B78E18FEA', N'Level5d', 3, N'Tanks/User/Site Data/Trend/PointGroup', N'Tank list, user data list config, site ancillary data.', N'83912BBD-113C-4824-9406-6DC3FED36590', 27, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'50F37270-FB36-4546-A7CE-D5B79D688158', N'Level5e', 3, N'PointTemplateAlarmStatus/PointGroupColumns/PointGroupRows', N'PointTemplateAlarmStatus, PointGroupColumns, PointGroupRows', N'83912BBD-113C-4824-9406-6DC3FED36590', 28, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'D473A0A5-B26C-41D1-B9C3-98A8E2CE68A7', N'Level6a', 3, N'ApptTank/CompanyOwner', N'Appointment tank, company to owner and entity to site mappings, meter to tank mapping.', N'83912BBD-113C-4824-9406-6DC3FED36590', 29, N'2013-02-19 18:09:33.0000000 -05:00', N'Administrator', N'2013-02-19 18:09:33.0000000 -05:00', N'Administrator', 0) 
	,(N'99C455D7-0124-43E4-B6DC-B9A5D30D01ED', N'Level6b', 3, N'Product mappings', N'Product to ... mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 30, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'AC172A6D-9C1D-4D84-8481-31649021B771', N'Level6c', 3, N'Stations/Tank Data', N'Stations, tank group mapping, tank tests and quality log.', N'83912BBD-113C-4824-9406-6DC3FED36590', 31, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'184BDC35-FAB9-49C0-BC53-F5475696879A', N'Level6d', 3, N'Equipment', N'Only the equipment table.', N'83912BBD-113C-4824-9406-6DC3FED36590', 32, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'4A00ACEE-A597-4C8C-9C32-ED31A359777F', N'Level7a', 3, N'ApptEq/Maps/Personnel', N'Appointment equipment, Eqiupment to site mapping, load arms, personnel and some process variable data.', N'83912BBD-113C-4824-9406-6DC3FED36590', 33, N'2013-02-19 18:17:16.0000000 -05:00', N'Administrator', N'2013-02-19 18:17:16.0000000 -05:00', N'Administrator', 0) 
	,(N'50C4B179-CF13-4671-BACF-94C7D5FFF9C8', N'Level7b', 3, N'Qualifications/Test Results', N'Equipment and person qualification mappings, test result mappings, etc...', N'83912BBD-113C-4824-9406-6DC3FED36590', 34, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'F1DC1219-F62D-469A-BCA0-A6351BB52E17', N'Level8a', 3, N'Personnel Appt/Sites', N'Personnel appointments, ship to mapping bill to, personnel site mapping, house cards, personnel roles, messages, etc.', N'83912BBD-113C-4824-9406-6DC3FED36590', 35, N'2013-02-19 18:25:26.0000000 -05:00', N'Administrator', N'2013-02-19 18:25:26.0000000 -05:00', N'Administrator', 0) 
	,(N'0E46670C-99B4-4D1A-A72A-7B42902C041C', N'Level8b', 3, N'ProcessVars/ProductPresets', N'Process variables, product to preset mappings, etc...', N'83912BBD-113C-4824-9406-6DC3FED36590', 36, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'FFBB5A46-C410-4D88-8421-EA586B61AAC8', N'Level8c', 3, N'ProductPresets/Qualifications', N'Additional product to preset mappings, person qualifications, test equipment results, etc.', N'83912BBD-113C-4824-9406-6DC3FED36590', 37, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'475C6EB1-8427-4CE2-94C0-C87336C448C8', N'Level8d', 3, N'Allocations', N'Allocations and Allocation Line items', N'83912BBD-113C-4824-9406-6DC3FED36590', 38, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 1) 
	,(N'4BEABD46-E00F-4B71-BDA2-1852E4496691', N'Level9a', 3, N'Message Log/Bulk Payment Links', N'Message log, bulk payment links, etc...', N'83912BBD-113C-4824-9406-6DC3FED36590', 39, N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:07.0000000 -05:00', N'Administrator', 0) 
	,(N'A97A5644-2F2F-4F54-9A41-97E31F8AA748', N'Level9b', 3, N'Additional Process Variables', N'Additional process variable mappings.', N'83912BBD-113C-4824-9406-6DC3FED36590', 40, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 0) 
	,(N'159E6491-AB99-4742-BDF1-29A012B56084', N'Level9c', 3, N'Transactions', N'Transactions and Transaction Releated Date', N'83912BBD-113C-4824-9406-6DC3FED36590', 41, N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', N'2012-11-15 08:25:06.0000000 -05:00', N'Administrator', 1) 
	,(N'E1EDBF11-908A-4CA6-B1C4-0B769BBB136C', N'Level9d', 3, N'Last Export Result Detail', N'Additional transaction data related with 3rd party exports', N'83912BBD-113C-4824-9406-6DC3FED36590', 42, N'2015-09-11 10:41:16.1678647 -04:00', N'Administrator', N'2015-09-11 10:41:16.1678647 -04:00', N'Administrator', 0)
	,(N'53d460ca-059c-4b39-b30b-14c972b881e0', N'Level10a', 3, N'Allocation Items/Transaction Data', N'Allocation line items, transaction line item user data, transaction links and transaction sub line items', N'83912bbd-113c-4824-9406-6dc3fed36590', 43, N'2015-09-11 10:41:16.1678647 -04:00', N'Administrator', N'2015-09-11 10:41:16.1678647 -04:00', N'Administrator', 0)
	,(N'B1967175-CB6E-4A77-8109-46F4297ACACD', N'Level10b', 3, N'Points', N'Points', N'83912bbd-113c-4824-9406-6dc3fed36590', 44, N'2019-07-02 10:45:16.1678647 -04:00', N'Administrator', N'2019-07-02 10:45:16.1678647 -04:00', N'Administrator', 1)
	,(N'5E1747A2-FF7D-46D7-866A-551EF3949D9E', N'Level10c', 3, N'Movement Summary', N'Movement Summary', N'83912bbd-113c-4824-9406-6dc3fed36590', 45, N'2025-11-10 10:45:16.1678647 -04:00', N'Administrator', N'2025-11-10 10:45:16.1678647 -04:00', N'Administrator', 0)
 ) AS Source ([SyncScopeGuid], [ID], [SyncScopeTypeIndex], [FriendlyName], [LongDescription], [SyncProfileGuid], [SyncOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SyncSinglePass])
ON (Target.[SyncScopeGuid] = Source.[SyncScopeGuid])
WHEN MATCHED AND EXISTS (SELECT Target.[ID]
						, Target.[SyncScopeTypeIndex]
						, Target.[FriendlyName]
						, Target.[LongDescription]
						, Target.[SyncProfileGuid]
						, Target.[SyncOrder]
						, Target.[CreatedDate]
						, Target.[CreatedBy]
						, Target.[UpdatedDate]
						, Target.[UpdatedBy] 
						, Target.[SyncSinglePass]
						EXCEPT 
						SELECT Source.[ID]
						, Source.[SyncScopeTypeIndex]
						, Source.[FriendlyName]
						, Source.[LongDescription]
						, Source.[SyncProfileGuid]
						, Source.[SyncOrder]
						, Source.[CreatedDate]
						, Source.[CreatedBy]
						, Source.[UpdatedDate]
						, Source.[UpdatedBy]
						, Source.[SyncSinglePass] ) THEN
	UPDATE SET [ID] = Source.[ID]
				, [SyncScopeTypeIndex] = Source.[SyncScopeTypeIndex]
				, [FriendlyName] = Source.[FriendlyName]
				, [LongDescription] = Source.[LongDescription]
				, [SyncProfileGuid] = Source.[SyncProfileGuid]
				, [SyncOrder] = Source.[SyncOrder]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
				, [SyncSinglePass] = Source.[SyncSinglePass]
WHEN NOT MATCHED THEN
	INSERT ([SyncScopeGuid], [ID], [SyncScopeTypeIndex], [FriendlyName], [LongDescription], [SyncProfileGuid], [SyncOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SyncSinglePass])
		VALUES (Source.[SyncScopeGuid], Source.[ID], Source.[SyncScopeTypeIndex], Source.[FriendlyName], Source.[LongDescription], Source.[SyncProfileGuid], Source.[SyncOrder], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy], Source.[SyncSinglePass])
OUTPUT
   $action AS ActionType,
   deleted.[ID],
   inserted.[ID],
   deleted.[SyncScopeTypeIndex],
   inserted.[SyncScopeTypeIndex],
   deleted.[FriendlyName],
   inserted.[FriendlyName],
   deleted.[LongDescription],
   inserted.[LongDescription],
   deleted.[SyncProfileGuid],
   inserted.[SyncProfileGuid],
   deleted.[SyncOrder],
   inserted.[SyncOrder],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy],
   deleted.[SyncSinglePass],
   inserted.[SyncSinglePass]
INTO @tblSyncScopeRefData;

SELECT @SyncScopeInserted = COUNT(*) FROM @tblSyncScopeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @SyncScopeUpdated = COUNT(*) FROM @tblSyncScopeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @SyncScopeDeleted = COUNT(*) FROM @tblSyncScopeRefData WHERE ActionType IN ( 'DELETE' )

IF (@SyncScopeInserted = 0 AND @SyncScopeUpdated = 0)
BEGIN
	PRINT '** No Changes Detected for [sync].[tblSyncScope] **'
	PRINT ''
END

IF (@SyncScopeInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncScopeInserted) + ' NEW RECORDS INSERTED INTO [sync].[tblSyncScope] **'
	PRINT ''
END

IF (@SyncScopeUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncScopeUpdated) + ' EXISTING RECORDS UPDATED IN [sync].[tblSyncScope] **'
	PRINT ''
	SELECT * FROM @tblSyncScopeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
