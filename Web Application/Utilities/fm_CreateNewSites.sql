--=======================================================================================================
-- 1. Insert x number sites (rows) into the tblSites table.  The new sites should be local (SiteGroupFlag = 0) 
--    sites. These sites shall have the same settings like existing sites. 
-- 2. Assign each of the new local sites to the JFLA group by inserting a row for each new site into the 
--    tblSiteToSiteMap (set ParentSiteIndex)
--
-- Author: Richard Panachida
-- August 16, 2010
--=======================================================================================================

USE ConsolidatedDB
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_CreateNewSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_CreateNewSites]
GO

CREATE PROCEDURE [dbo].[fm_CreateNewSites]
(
	@NumberOfSites INT,
	@GroupSiteID NVARCHAR(30)	-- The Site Group that will be the parent site of children sites
)
AS
SET NOCOUNT ON

BEGIN
	--=================================================================
    -- 1. Create Sites based on the Number Of Sites parameter.
    --=================================================================
	SELECT TOP(1) [ID]
			   ,[Number]
			   ,[SPLCCode]
			   ,[Address1]
			   ,[Address2]
			   ,[City]
			   ,[State]
			   ,[Zip]
			   ,[Country]
			   ,[Phone]
			   ,[FAX]
			   ,[EmailAddress]
			   ,[EmergencyContact]
			   ,[EmergencyPhone]
			   ,[Enabled]
			   ,[SiteGroupFlag]
			   ,[TimeZone]
			   ,[AdjustForDaylightSavings]
			   ,[LevelUnitIndex]
			   ,[TemperatureUnitIndex]
			   ,[DensityUnitIndex]
			   ,[PressureUnitIndex]
			   ,[FlowUnitIndex]
			   ,[VolumeUnitIndex]
			   ,[MassUnitIndex]
			   ,[AdditiveVolumeUnitIndex]
			   ,[AdditiveProfileCycleAmountUnitIndex]
			   ,[AdditiveProfileRateUnitIndex]
			   ,[LevelDecimalPlaces]
			   ,[TemperatureDecimalPlaces]
			   ,[DensityDecimalPlaces]
			   ,[PressureDecimalPlaces]
			   ,[FlowDecimalPlaces]
			   ,[VolumeDecimalPlaces]
			   ,[MassDecimalPlaces]
			   ,[AdditiveVolumeDecimalPlaces]
			   ,[AdditiveProfileCycleAmountDecimalPlaces]
			   ,[AdditiveProfileRateDecimalPlaces]
			   ,[VolumeDisplayDefault]
			   ,[InhibitAccessAfterHours]
			   ,[InhibitMultipleCardIns]
			   ,[AccessCardinRequired]
			   ,[CheckSiteNumber]
			   ,[PromptForCustomerCard]
			   ,[PromptForTractorOrTanker]
			   ,[PromptForFirstTrailer]
			   ,[PromptForSecondTrailer]
			   ,[PromptForCompartment]
			   ,[EnforceDriverEquipmentMatch]
			   ,[EnableAdditiveAccounting]
			   ,[UseCompanyEquipmentIdentifiers]
			   ,[UseLastKnownGoodTankData]
			   ,[MaximumLoadAmount]
			   ,[MaximumLoadTime]
			   ,[MaximumIdleTime]
			   ,[MaximumFlushAmount]
			   ,[MaximumMeterProvingAmount]
			   ,[MaximumReturnsAmount]
			   ,[MaximumNumberOfActiveArms]
			   ,[DriverTimeoutPeriod]
			   ,[DriverWarningPeriod]
			   ,[MaximumPrompts]
			   ,[InventoryTransactionAliasIndex]
			   ,[AdjustmentTransactionAliasIndex]
			   ,[MaximumVehicleWeight]
			   ,[LoadByNet]
			   ,[PromptForShipmentNumber]
			   ,[MaximumProductTemperature]
			   ,[ListEquipment]
			   ,[DeferStationChanges]
			   ,[InhibitBOLWithBrokenBlends]
			   ,[InhibitBOLWithImproperAdditization]
			   ,[InhibitOverweightBOL]
			   ,[ExceptionBOLPrinter]
			   ,[EnableAutomaticBOLPrinting]
			   ,[AutomaticBOLStartNumber]
			   ,[AutomaticBOLEndNumber]
			   ,[AutomaticBOLNextNumber]
			   ,[SeparateManualBOLNumbering]
			   ,[ManualBOLStartNumber]
			   ,[ManualBOLEndNumber]
			   ,[ManualBOLNextNumber]
			   ,[TransactionStartNumber]
			   ,[TransactionEndNumber]
			   ,[TransactionNextNumber]
			   ,[OrderStartNumber]
			   ,[OrderEndNumber]
			   ,[OrderNextNumber]
			   ,[NumberPrefix]
			   ,[OpenTransactionWindow]
			   ,[AdministrativeLockDate]
			   ,[OperationalLockDate]
			   ,[MaximumDaysToRetainLogs]
			   ,[EnableDebugLogging]
			   ,[EnableAuditLogging]
			   ,[AutomaticallyPrintAlarmsAndEvents]
			   ,[AlarmAndEventPrinter]
			   ,[MailServer]
			   ,[MailFrom]
			   ,[MailUserName]
			   ,[MailPassword]
			   ,[MailConnectMode]
			   ,[DialupName]
			   ,[SCADASystem]
			   ,[InhibitTemplateGraphics]
			   ,[RefreshInterval]
			   ,[InhibitEndOfDayOperations]
			   ,[InhibitEndOfMonthOperations]
			   ,[EndOfDayWarningPeriod]
			   ,[InhibitAutomaticPhysicalInventory]
			   ,[InhibitAutomaticMeterCloseout]
			   ,[InhibitAutomaticReportGeneration]
			   ,[InhibitAutomaticAdjustmentDistribution]
			   ,[InhibitAutomaticCloseout]
			   ,[InhibitTankScan]
			   ,[ReportDirectory]
			   ,[ManageReports]
			   ,[ManagedReportDirectory]
			   ,[VRURateLimit]
			   ,[VRUHourlyLimit]
			   ,[VRUDailyLimit]
			   ,[VRUYearlyLimit]
			   ,[VRUCurrentYearLimit]
			   ,[VRURateActual]
			   ,[VRUHourlyActual]
			   ,[VRUDailyActual]
			   ,[VRUYearlyActual]
			   ,[VRUCurrentYearActual]
			   ,[VRURateLimitEnabled]
			   ,[VRUHourlyLimitEnabled]
			   ,[VRUDailyLimitEnabled]
			   ,[VRUYearlyLimitEnabled]
			   ,[VRUCurrentYearLimitEnabled]
			   ,[WatchdogPeriod]
			   ,[WatchdogMode]
			   ,[WatchdogCounterStart]
			   ,[WatchdogCounterEnd]
			   ,[NumberGroupSizesType]
			   ,[NumberDecimalSeparator]
			   ,[NumberGroupSeparator]
			   ,[ListSeparator]
			   ,[TimePattern]
			   ,[TimeSeparator]
			   ,[AMSymbol]
			   ,[PMSymbol]
			   ,[ShortDatePattern]
			   ,[DateSeparator]
			   ,[LongDatePattern]
			   ,[TwoDigitCalendarEndYear]
			   ,[UserData1]
			   ,[UserData2]
			   ,[UserData3]
			   ,[UserData4]
			   ,[UserData5]
			   ,[UserData6]
			   ,[UserData7]
			   ,[UserData8]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   ,[MinTimeAllowedToChangePwd]
			   ,[MinPwdCharacterLength]
			   ,[PwdExpirationInDays]
			   ,[PwdLockoutThreshold]
			   ,[CheckForPreviousPwd]
			   ,[StrongPwdUse]
			   ,[PwdHistoryCount]
			   ,[ApplyToAllSiteMembers]
			   ,[InactivityDisablePeriod]
			   ,[EnforceSingleOwner]
			   ,[InhibitBOLSummaryAutoPopulate]
			   ,[InhibitOrderSummaryAutoPopulate]
			   ,[InhibitSupplyOrderSummaryAutoPopulate]
			   ,[InvoiceStartNumber]
			   ,[InvoiceEndNumber]
			   ,[InvoiceNextNumber]
			   ,[PromptForReturns]
			   ,[PromptForTruckCard]
			   ,[StartingShortCardNumber]
			   ,[UseShortCardNumber]
			   ,[ExcessVarianceCount]
			   ,[ExcessVarianceTolerance]
			   ,[SecondaryStorageFillMethod]
			   ,[DisableArchivePeriod]
			   ,[UseTankReconciliation]
	INTO #SITE_TEMPLATE
	FROM [dbo].[tblSites]
	WHERE SiteGroupFlag = 0
	
	-- Create the new sites
	DECLARE @SiteCount INT
	DECLARE @SiteID NVARCHAR(30)
	DECLARE @SiteSeq INT

	SELECT @SiteSeq = MAX(SiteIndex) FROM tblSites
	SET @SiteCount = 0
	PRINT ''

	WHILE (@SiteCount < @NumberOfSites)
	BEGIN
		SET @SiteSeq = @SiteSeq + 1
		SET @SiteID = 'Site-sn' + CONVERT(NVARCHAR(10), @SiteSeq)
		
		PRINT 'Creating site: ' + @SiteID
		UPDATE #SITE_TEMPLATE SET ID = @SiteID, Number = @SiteID, CreatedBy = 'AutoGen', UpdatedBy = 'AutoGen',
								  CreatedDate = GETDATE(), UpdatedDate = GETDATE()
		
		BEGIN TRY
			INSERT INTO tblSites
				SELECT * FROM #SITE_TEMPLATE
		END TRY
		BEGIN CATCH
			-- Ignore
			PRINT 'Site "' + @SiteID + '" has been already created.'
		END CATCH
			
		SET @SiteCount = @SiteCount + 1
	END

	PRINT 'Completed site creation...'
	PRINT ''

	--=======================================================================
	-- 2. Assign the new sites to a given site group.  The site group value
	--    was passed in.
	--=======================================================================
	DECLARE @GroupSiteIndex INT
	SELECT @GroupSiteIndex = Siteindex FROM tblSites WHERE ID = @GroupSiteID 

	SELECT SiteIndex, ID INTO #SITE_INDEX_LIST
		FROM tblSites
		WHERE ID LIKE ('%Site-sn%')
	
	DECLARE @SiteIndex INT
	DECLARE @SiteIndex_Cursor CURSOR 
	SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM #SITE_INDEX_LIST

	OPEN	@SiteIndex_Cursor
	FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		BEGIN TRY
			SELECT @SiteID = ID FROM #SITE_INDEX_LIST WHERE SiteIndex = @SiteIndex
			PRINT 'Assigning site "' + @SiteID + '" to group site "' + @GroupSiteID + '".'
			
			INSERT INTO tblSiteToSiteMap (ParentSiteIndex
										  ,ChildSiteIndex
										  ,CreatedDate
										  ,CreatedBy)
				   VALUES(@GroupSiteIndex, @SiteIndex, GETDATE(), 'AutoGen')
		END TRY
		BEGIN CATCH
			-- Ignore
			PRINT 'Site "' + @SiteID + '" aready assigned to group site "' + @GroupSiteID + '".'
		END CATCH

		FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex
	END
	
	CLOSE @SiteIndex_Cursor
	DEALLOCATE @SiteIndex_Cursor
END