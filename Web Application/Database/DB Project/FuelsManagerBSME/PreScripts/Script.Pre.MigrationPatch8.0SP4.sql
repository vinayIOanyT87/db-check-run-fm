IF EXISTS (SELECT 1 FROM [dbo].[tblVersion] WHERE [VersionIndex] = (SELECT MAX(v1.VersionIndex) FROM [dbo].[tblVersion] v1) AND [Version] LIKE '8.0.%')
BEGIN
	SET XACT_ABORT ON;
	SET NOCOUNT ON;

	DECLARE @SchemaName NVARCHAR(200)
		,	@TableName NVARCHAR(500)
		,	@Sql NVARCHAR(max)


	PRINT '------------------------------------------'
	PRINT '-- PRE-SCRIPT <START>'
	PRINT '------------------------------------------'
	PRINT ''
	PRINT 'Executing script .\PreScript\Script.PreDeploymentSub.00001.sql'
	PRINT '*** Fixing Nullable CreatedBy columns:'
	PRINT ''

	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_CATALOG='tblSites')
	BEGIN
		IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_BOLSummary]'))
		DROP VIEW [dbo].[vw_BOLSummary]


		DECLARE FixColumnCursor CURSOR FOR
			SELECT TABLE_SCHEMA,TABLE_NAME 
			FROM INFORMATION_SCHEMA.COLUMNS
			WHERE COLUMN_NAME = 'CreatedBy'
			AND IS_NULLABLE = 'YES'
			AND LEFT(TABLE_NAME,3) != 'vw_'
			ORDER BY TABLE_SCHEMA,TABLE_NAME

		OPEN FixColumnCursor
		FETCH NEXT FROM FixColumnCursor
		INTO @SchemaName,@TableName
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @Sql = 'UPDATE ['+@SchemaName+'].['+@TableName+'] SET [CreatedBy]=''Administrator'' WHERE [CreatedBy] IS NULL'
			PRINT @Sql
			EXEC sp_executesql @Sql
	
			SET @Sql = 'ALTER TABLE ['+@SchemaName+'].['+@TableName+'] ALTER COLUMN [CreatedBy] udtUserId NOT NULL '
			PRINT @Sql
			EXEC sp_executesql @Sql
	
			FETCH NEXT FROM FixColumnCursor
			INTO @SchemaName,@TableName
		END
		CLOSE FixColumnCursor
		DEALLOCATE FixColumnCursor

		PRINT ''
		PRINT '*** Fixing Nullable UpdatedBy columns:'
		PRINT ''

		DECLARE FixColumnCursor CURSOR FOR
			SELECT TABLE_SCHEMA,TABLE_NAME 
			FROM INFORMATION_SCHEMA.COLUMNS
			WHERE COLUMN_NAME = 'UpdatedBy'
			AND IS_NULLABLE = 'YES'
			AND LEFT(TABLE_NAME,3) != 'vw_'	
			ORDER BY TABLE_SCHEMA,TABLE_NAME

		OPEN FixColumnCursor
		FETCH NEXT FROM FixColumnCursor
		INTO @SchemaName,@TableName
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @Sql = 'UPDATE ['+@SchemaName+'].['+@TableName+'] SET [UpdatedBy]=''Administrator'' WHERE [UpdatedBy] IS NULL'
			PRINT @Sql
			EXEC sp_executesql @Sql
	
			SET @Sql = 'ALTER TABLE ['+@SchemaName+'].['+@TableName+'] ALTER COLUMN [UpdatedBy] udtUserId NOT NULL '
			PRINT @Sql
			EXEC sp_executesql @Sql
	
			FETCH NEXT FROM FixColumnCursor
			INTO @SchemaName,@TableName
		END
		CLOSE FixColumnCursor
		DEALLOCATE FixColumnCursor

		/*
			CLEANUP tblTransactionTransportationLineItems	
		*/

		PRINT ''
		PRINT '-- CLEANING UP tblTransactionTransportLineItems WHERE TransportOrderNumber IS NULL OR Empty String...'
		PRINT ''

		DELETE 
		FROM tblTransactionTransportLineItems
		WHERE TransportOrderNumber IS NULL
		OR TransportOrderNumber = ''

		/*
			UPDATING UNMATCHED SITEGUID	
		*/

		PRINT ''
		PRINT '-- Updating unmached StieGuid on tblAlarmAndEvents...'
		PRINT ''

		UPDATE t1
		SET SiteGuid='00000000-0000-0000-0000-000000000001'
		FROM tblAlarmAndEventLog t1
		WHERE NOT EXISTS(
			SELECT 1 FROM tblSites t2
			WHERE t2.SiteGuid=t1.SiteGuid)

		UPDATE t1
		SET SiteGuid='00000000-0000-0000-0000-000000000001'
		FROM tblAlarmAndEvents t1
		WHERE NOT EXISTS(
			SELECT 1 FROM tblSites t2
			WHERE t2.SiteGuid=t1.SiteGuid)

		EXEC sp_executesql N'CREATE VIEW [dbo].[vw_BOLSummary]
		--WITH SCHEMABINDING
		AS
		SELECT
				TransID,
				AliasName,
				TransactionAliasGuid,
				LookupTransTypeIndex,
				SubType,
				Site,
				SiteGuid,
				TransReferenceID,
				InventoryDate,
				ShipToID,
				ShipToCode,
				ShipToCompanyGuid,
				SupplierID,
				SupplierCode,
				SupplierCompanyGuid,
				CreatedDate,
				CreatedBy,
				RequestedDeliveryDate,
				UpdatedDate,
				UpdatedBy,
				TransDateTime,
				TransVersion,
				SCACCode,
				CardNumber,
				ShipmentNumber,
				ShipperID,
				ShipperCode,
				ShipperCompanyGuid,
				OwnerID,
				OwnerCode,
				OwnerCompanyGuid,
				ManagerID,
				ManagerCode,
				ManagerCompanyGuid,
				CarrierID,
				CarrierCode,
				CarrierCompanyGuid,
				ConjoinTransID,
				ReversedTransID,
				LinkedDocumentNumber,
				ReversalType,
				PONumber,
				TimeIn,
				TimeOut,
				TimeEnd,
				RoutingID,
				TicketSource,
				LoadID,
				LookupTransactionStatusIndex,
				BillToID,
				BillToCode,
				BillToCompanyGuid,
				DriverIdentificationNumber,
				CreditAmount,
				CardExpiration,
				CardName,
				CardType,
				CashAmount,
				RouteOriginationDate,
				InternationalRouteIndicator,
				PreviousRoutingID,
				FinalStationIATAGuid,
				FinalStationIATAID,
				PreviousStationIATAGuid,
				PreviousStationIATAID,
				NextStationIATAGuid,
				NextStationIATAID,
				OriginStationIATAGuid,
				OriginStationIATAID,
				ShippingDocumentNumber,
				DocumentNumber,
				STD,
				ETD,
				STA,
				ETA,
				SFT,
				FST,
				EstimatedFuelingDuration,
				DeleteFlag,
				TicketMode,
				DestinationRegistrationID1,
				DestinationSerialNumber1,
				DestinationEquipmentType1,
				DestinationEquipmentModel1,
				DestinationCompanyEquipmentID1,
				Destination1EquipmentGuid,
				DestinationRegistrationID2,
				DestinationSerialNumber2,
				DestinationEquipmentType2,
				DestinationEquipmentModel2,
				DestinationCompanyEquipmentID2,
				Destination2EquipmentGuid,
				DestinationRegistrationID3,
				DestinationSerialNumber3,
				DestinationEquipmentType3,
				DestinationEquipmentModel3,
				DestinationCompanyEquipmentID3,
				Destination3EquipmentGuid,
				SourceRegistrationID1,
				SourceSerialNumber1,
				SourceEquipmentType1,
				SourceEquipmentModel1,
				SourceCompanyEquipmentID1,
				Source1EquipmentGuid,
				SourceRegistrationID2,
				SourceSerialNumber2,
				SourceEquipmentType2,
				SourceEquipmentModel2,
				SourceCompanyEquipmentID2,
				Source2EquipmentGuid,
				SourceRegistrationID3,
				SourceSerialNumber3,
				SourceEquipmentType3,
				SourceEquipmentModel3,
				SourceCompanyEquipmentID3,
				Source3EquipmentGuid,
				OperatorID,
				OperatorPersonnelGuid,
				EffectiveDate,
				ExpirationDate,
				ScheduledDate,
				AutoComplete
			  FROM dbo.tblTransactions
			 WHERE ISNULL(DeleteFlag, 0) = 0'

	END
	PRINT '------------------------------------------'
	PRINT '-- PRE-SCRIPT <END>'
	PRINT '------------------------------------------'
	PRINT ''
END
ELSE
BEGIN
	PRINT '------------------------------------------'
	PRINT '-- PRE-SCRIPT SKIPPED'
	PRINT '-- DATABASE VERSION WAS NOT v8.0.x'
	PRINT '------------------------------------------'
	PRINT ''
END
