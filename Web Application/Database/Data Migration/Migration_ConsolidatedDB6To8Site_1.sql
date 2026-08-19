USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6To8Site_1]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Site_1') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8Site_1
GO


CREATE PROCEDURE [dbo].Migration_ConsolidatedDB6To8Site_1
 /*=============================================
 Author:			URVI PATEL
 Create date:		1/8/2010
 Description:		Migrating ConsolidatedDB 6.0 to ConsolidatedDB 8.0 Site Table
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_ConsolidatedDB6To8Site_1 2, null

*/
(
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL
)

AS 

IF @SiteID = 'All Sites' or @IsBaseDB <> 2 SET @SiteID = NULL



SELECT   s.siteindex AS [SITEINDEX], LEFT(s.SiteID,30) AS [ID] --30
INTO #TMP_SITES
FROM     [ConsolidatedDB6].[dbo].[tblSites] s
INNER JOIN [ConsolidatedDB6].dbo.tblContacts c
ON s.contactindex = c.contactIndex
WHERE s.deleteflag = 0 and s.RegionalSiteFlag = 0
AND s.siteindex <> -1
AND s.siteid =  isnull(@SiteID, s.siteid)
AND s.siteid NOT IN (SELECT [ID] FROM [ConsolidatedDB].[dbo].tblSites)


INSERT INTO [ConsolidatedDB].[dbo].[tblSites]
(

tblSites.ID,
tblSites.VolumeUnitIndex,
tblSites.TemperatureUnitIndex,
tblSites.DensityUnitIndex,
tblSites.VolumeDecimalPlaces,
tblSites.TemperatureDecimalPlaces,
tblSites.DensityDecimalPlaces,
tblSites.CreatedDate,
tblSites.CreatedBy,
tblSites.UpdatedDate,
tblSites.UpdatedBy,
tblSites.siteGroupFlag, --regionalSiteFlag
tblSites.Number, --Description
tblSites.MailServer,
tblSites.MailUserName,
tblSites.MailPassword,
tblSites.EmailAddress,
tblSites.MailFrom,
tblSites.Address1,
tblSites.Address2,
tblSites.City,
tblSites.State,
tblSites.Country,
tblSites.Zip,
tblSites.Phone,
tblSites.EmergencyContact,
tblSites.Enabled,
SPLCCode,
FAX,
EmergencyPhone,
TimeZone,
AdjustForDaylightSavings,
LevelUnitIndex,
PressureUnitIndex, 
      FlowUnitIndex, 
      MassUnitIndex, 
      AdditiveVolumeUnitIndex,
      AdditiveProfileCycleAmountUnitIndex, 
      AdditiveProfileRateUnitIndex, 
      LevelDecimalPlaces, 
     
      PressureDecimalPlaces, 
      FlowDecimalPlaces, 
      
      MassDecimalPlaces, 
      AdditiveVolumeDecimalPlaces, 
      AdditiveProfileCycleAmountDecimalPlaces, 
      AdditiveProfileRateDecimalPlaces, 
      QuantityDisplayDefault, --VolumeDisplayDefault, 
      InhibitAccessAfterHours, 
      InhibitMultipleCardIns, 
      AccessCardinRequired, 
      CheckSiteNumber, 
      PromptForCustomerCard, 
      PromptForTractorOrTanker, 
      PromptForFirstTrailer, 
      PromptForSecondTrailer, 
      PromptForCompartment, 
      EnforceDriverEquipmentMatch, 
      EnableAdditiveAccounting, 
      UseCompanyEquipmentIdentifiers, 
      UseLastKnownGoodTankData, 
      MaximumLoadAmount, 
      MaximumLoadTime, 
      MaximumIdleTime, 
      MaximumFlushAmount, 
      MaximumMeterProvingAmount, 
      MaximumReturnsAmount, 
      MaximumNumberOfActiveArms, 
      DriverTimeoutPeriod, 
      DriverWarningPeriod, 
      MaximumPrompts, 
      InventoryTransactionAliasIndex, 
      AdjustmentTransactionAliasIndex, 
      MaximumVehicleWeight, 
      LoadByNet, 
      PromptForShipmentNumber, 
      MaximumProductTemperature, 
      ListEquipment, 
      DeferStationChanges, 
      InhibitBOLWithBrokenBlends, 
      InhibitBOLWithImproperAdditization, 
      InhibitOverweightBOL, 
      ExceptionBOLPrinter, 
      EnableAutomaticBOLPrinting, 
      AutomaticBOLStartNumber, 
      AutomaticBOLEndNumber, 
      AutomaticBOLNextNumber, 
      SeparateManualBOLNumbering, 
      ManualBOLStartNumber, 
      ManualBOLEndNumber, 
      ManualBOLNextNumber, 
      TransactionStartNumber, 
      TransactionEndNumber, 
      TransactionNextNumber, 
      OrderStartNumber, 
      OrderEndNumber, 
      OrderNextNumber, 
      NumberPrefix, 
      OpenTransactionWindow, 
      AdministrativeLockDate, 
      OperationalLockDate, 
      MaximumDaysToRetainLogs, 
      EnableDebugLogging, 
      EnableAuditLogging, 
      AutomaticallyPrintAlarmsAndEvents, 
      AlarmAndEventPrinter, 
      
      MailConnectMode, 
      DialupName, 
      SCADASystem, 
      InhibitTemplateGraphics, 
      RefreshInterval, 
      InhibitEndOfDayOperations,
      InhibitEndOfMonthOperations, 
      EndOfDayWarningPeriod, 
      InhibitAutomaticPhysicalInventory, 
      InhibitAutomaticMeterCloseout,
      InhibitAutomaticReportGeneration, 
      InhibitAutomaticAdjustmentDistribution, 
      InhibitAutomaticCloseout, 
      InhibitTankScan, 
      ReportDirectory, 
      ManageReports, 
      ManagedReportDirectory, 
      VRURateLimit, 
      VRUHourlyLimit, 
      VRUDailyLimit, 
      VRUYearlyLimit, 
      VRUCurrentYearLimit, 
      VRURateActual, 
      VRUHourlyActual, 
      VRUDailyActual, 
      VRUYearlyActual, 
      VRUCurrentYearActual, 
      VRURateLimitEnabled, 
      VRUHourlyLimitEnabled, 
      VRUDailyLimitEnabled, 
      VRUYearlyLimitEnabled, 
      VRUCurrentYearLimitEnabled, 
      WatchdogPeriod, 
      WatchdogMode, 
      WatchdogCounterStart, 
      WatchdogCounterEnd, 
      NumberGroupSizesType, 
      NumberDecimalSeparator, 
      NumberGroupSeparator, 
      ListSeparator, 
      TimePattern, 
      TimeSeparator, 
      AMSymbol, 
      PMSymbol, 
      ShortDatePattern, 
      DateSeparator, 
      LongDatePattern, 
      TwoDigitCalendarEndYear, 
      UserData1, 
      UserData2, 
      UserData3, 
      UserData4, 
      UserData5, 
      UserData6, 
      UserData7, 
      UserData8, 
      
      MinTimeAllowedToChangePwd, 
      MinPwdCharacterLength, 
      PwdExpirationInDays, 
      PwdLockoutThreshold, 
      CheckForPreviousPwd, 
      StrongPwdUse, 
      PwdHistoryCount, 
      ApplyToAllSiteMembers, 
      InactivityDisablePeriod, 
      EnforceSingleOwner, 
      InhibitBOLSummaryAutoPopulate, 
      InhibitOrderSummaryAutoPopulate, 
      InhibitSupplyOrderSummaryAutoPopulate, 
      InvoiceStartNumber, 
      InvoiceEndNumber, 
      InvoiceNextNumber, 
      PromptForReturns, 
      PromptForTruckCard, 
      StartingShortCardNumber, 
      UseShortCardNumber, 
      ExcessVarianceCount, 
      ExcessVarianceTolerance, 
      SecondaryStorageFillMethod, 
      DisableArchivePeriod
)
SELECT   -- s.SiteIndex,
		 LEFT(s.SiteID,30), --30
		 s.VolumeUnitIndex,
		 s.TemperatureUnitIndex, 
		 s.DensityUnitIndex, 
		 s.VolumeDecimalPlaces, 
		 s.TemperatureDecimalPlaces, 
		 s.DensityDecimalPlaces, 
		 s.CreatedDate, 
		 s.CreatedBy,
		 getdate(),
		 'Varec', --s.UpdatedBy, 
		 s.RegionalSiteFlag, 
		 LEFT(s.Description,30),
		 s.EmailServer,
		 s.EmailUserID, 
		 s.EmailPassword,
		 s.EmailFromAddress, 
		 s.EmailSenderName, 
		 CASE WHEN c.Address1 = '' OR c.Address1 is NULL THEN isnull('Not Found',LEFT(ltrim(rtrim(c.Address1)),30)) ELSE LEFT(ltrim(rtrim(c.Address1)),30) END,
		 LEFT(c.Address2,30),
		 CASE WHEN c.City = '' OR c.City is null THEN isnull('Not Found',LEFT(ltrim(rtrim(c.City)),20)) ELSE LEFT(ltrim(rtrim(c.City)),20) END,
		 CASE WHEN c.State = '' OR c.State is null THEN isnull('Not Found',LEFT(ltrim(rtrim(c.State)),30)) ELSE LEFT(ltrim(rtrim(c.State)),30) END,
		 LEFT(c.Country,20),
		 CASE WHEN c.PostalCode = '' OR c.PostalCode is null THEN isnull('Not Found',LEFT(ltrim(rtrim(c.PostalCode)),30)) ELSE LEFT(ltrim(rtrim(c.PostalCode)),30) END,
		 CASE WHEN c.Phone = '' OR c.Phone  is null THEN isnull('Not Found',LEFT(ltrim(rtrim(c.Phone)),20)) ELSE LEFT(ltrim(rtrim(c.Phone)),20) END ,
		 CASE WHEN c.ContactID = ''OR c.ContactID is null THEN isnull('Not Found',LEFT(ltrim(rtrim(c.ContactID)),30)) ELSE LEFT(ltrim(rtrim(c.ContactID)),30) END,
		 1,
		 '', --SPLCCode
'', --Fax
'',--EmergencyPhone,
'Eastern Standard Time',--TimeZone
1, --AdjustForDaylightSavings
27, --LevelUnitIndex
73, --PressureunitIndex
109, --FlowUnitIndex
64,
40,
40,
46, --AdditiveprofilerateunitIndex
2, --leveldecimalPlaces
2, --pressuredecimalplaces
1,--flowdecimalplaces
0,--MassDecimalPlaces
0,--AdditiveVolumeDecimalPlaces
0,--AdditiveProfileCycleAmountDecimalPlaces
0,--AdditiveProfileRateDecimalPlaces
1,--VolumeDisplayDefault
0,--InhibitAccessAfterHours
1,--InhibitMultipleCardIns
1,--AccessCardinRequired
0,--CheckSiteNumber
1,--PromptForCustomerCard
0,--PromptForTractorOrTanker
0,--PromptForFirstTrailer
0,--PromptForSecondTrailer
0,--PromptForCompartment
1,--EnforceDriverEquipmentMatch
1,--EnableAdditiveAccounting
0,--UseCompanyEquipmentIdentifiers
0,--UseLastKnownGoodTankData
1.321108788,--MaximumLoadAmount
720,--MaximumLoadTime
10,--MaximumIdleTime
0.13248942,--MaximumFlushAmount
0.13248942,--MaximumMeterProvingAmount
0.13248942,--MaximumReturnsAmount
10,--MaximumNumberOfActiveArms
90,--DriverTimeoutPeriod
5,--DriverWarningPeriod
3,--MaximumPrompts
NULL,--InventoryTransactionAliasIndex
NULL,--AdjustmentTransactionAliasIndex
36287.392,--MaximumVehicleWeight
0,--LoadByNet
0,--PromptForShipmentNumber
15.5555555555556,--MaximumProductTemperature
0,--ListEquipment
0,--DeferStationChanges
1,--InhibitBOLWithBrokenBlends
1,--InhibitBOLWithImproperAdditization
1,--InhibitOverweightBOL
'<None>',--ExceptionBOLPrinter
1,--EnableAutomaticBOLPrinting
0,--AutomaticBOLStartNumber
10000000,--AutomaticBOLEndNumber
0,--AutomaticBOLNextNumber
0,--SeparateManualBOLNumbering
0,--ManualBOLStartNumber
10000000,--ManualBOLEndNumber
0,--ManualBOLNextNumber
0,--TransactionStartNumber
10000000,--TransactionEndNumber
0,--TransactionNextNumber
0,--OrderStartNumber
10000000,--OrderEndNumber
0,--OrderNextNumber
'%Date%',--NumberPrefix
2,--OpenTransactionWindow
'2010-01-05',--AdministrativeLockDate
'2010-01-05',--OperationalLockDate
60,--MaximumDaysToRetainLogs
0,--EnableDebugLogging
1,--EnableAuditLogging
0,--AutomaticallyPrintAlarmsAndEvents
'<None>',--AlarmAndEventPrinter
0,--MailConnectMode
'',--DialupName
'localhost',--SCADASystem
0,--InhibitTemplateGraphics
5,--RefreshInterval
0,--InhibitEndOfDayOperations
0,--InhibitEndOfMonthOperations
30,--EndOfDayWarningPeriod
0,--InhibitAutomaticPhysicalInventory
1,--InhibitAutomaticMeterCloseout
1,--InhibitAutomaticReportGeneration
1,--InhibitAutomaticAdjustmentDistribution
1,--InhibitAutomaticCloseout
0,--InhibitTankScan
'/Standard Reports',--ReportDirectory
0,--ManageReports
'',--ManagedReportDirectory
0,--VRURateLimit
0,--VRUHourlyLimit
0,--VRUDailyLimit
0,--VRUYearlyLimit
0,--VRUCurrentYearLimit
0,--VRURateActual
0,--VRUHourlyActual
0,--VRUDailyActual
0,--VRUYearlyActual
0,--VRUCurrentYearActual
0,--VRURateLimitEnabled
0,--VRUHourlyLimitEnabled
0,--VRUDailyLimitEnabled
0,--VRUYearlyLimitEnabled
0,--VRUCurrentYearLimitEnabled
10,--WatchdogPeriod
0,--WatchdogMode
0,--WatchdogCounterStart
1000,--WatchdogCounterEnd
1,--NumberGroupSizesType
'.',--NumberDecimalSeparator
',',--NumberGroupSeparator
',',--ListSeparator
'hh:mm:ss tt',--TimePattern
':',--TimeSeparator
'AM',--AMSymbol
'PM',--PMSymbol
'M/d/yyyy',--ShortDatePattern
'/',--DateSeparator
'ddddd, MMMMM dd, yyyy',--LongDatePattern
2029,--TwoDigitCalendarEndYear
'',--UserData1
'',--UserData2
'',--UserData3
'',--UserData4
'',--UserData5
'',--UserData6
'',--UserData7
'',--UserData8
0,--MinTimeAllowedToChangePwd
0,--MinPwdCharacterLength
999,--PwdExpirationInDays
0,--PwdLockoutThreshold
0,--CheckForPreviousPwd
0,--StrongPwdUse
0,--PwdHistoryCount
0,--ApplyToAllSiteMembers
0,--InactivityDisablePeriod
0,--EnforceSingleOwner
0,--InhibitBOLSummaryAutoPopulate
0,--InhibitOrderSummaryAutoPopulate
0,--InhibitSupplyOrderSummaryAutoPopulate
0,--InvoiceStartNumber
10000000,--InvoiceEndNumber
0,--InvoiceNextNumber
0,--PromptForReturns
0,--PromptForTruckCard
1,--StartingShortCardNumber
0,--UseShortCardNumber
2,--ExcessVarianceCount
2,--ExcessVarianceTolerance
1,--SecondaryStorageFillMethod
0 --DisableArchivePeriod

FROM    
[ConsolidatedDB6].[dbo].[tblSites] s INNER JOIN [ConsolidatedDB6].dbo.tblContacts c
ON s.contactindex = c.contactIndex
WHERE s.deleteflag = 0 and s.RegionalSiteFlag = 0
AND s.siteindex <> -1
AND s.siteid =  isnull(@SiteID, s.siteid)
AND s.siteid IN (SELECT [ID] FROM #TMP_SITES)


--Eric Simmons (4-20-2010)
--Added to resolve Change Request 14392
Update ConsolidatedDB.dbo.tblSites SET
ConsolidatedDB.dbo.tblSites.SPLCCode = ts.SPLCCode,
ConsolidatedDB.dbo.tblSites.TimeZone = ts.TimeZone,
ConsolidatedDB.dbo.tblSites.AdjustForDaylightSavings = ts.AdjustForDaylightSavings,
ConsolidatedDB.dbo.tblSites.LevelUnitIndex = ts.LevelUnitIndex,
ConsolidatedDB.dbo.tblSites.PressureUnitIndex  = ts.PressureUnitIndex ,
ConsolidatedDB.dbo.tblSites.FlowUnitIndex  = ts.FlowUnitIndex ,
ConsolidatedDB.dbo.tblSites.MassUnitIndex  = ts.MassUnitIndex ,
ConsolidatedDB.dbo.tblSites.AdditiveVolumeUnitIndex = ts.AdditiveVolumeUnitIndex,
ConsolidatedDB.dbo.tblSites.AdditiveProfileCycleAmountUnitIndex  = ts.AdditiveProfileCycleAmountUnitIndex ,
ConsolidatedDB.dbo.tblSites.AdditiveProfileRateUnitIndex  = ts.AdditiveProfileRateUnitIndex ,
ConsolidatedDB.dbo.tblSites.LevelDecimalPlaces  = ts.LevelDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.PressureDecimalPlaces  = ts.PressureDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.FlowDecimalPlaces  = ts.FlowDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.MassDecimalPlaces  = ts.MassDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.AdditiveVolumeDecimalPlaces  = ts.AdditiveVolumeDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.AdditiveProfileCycleAmountDecimalPlaces  = ts.AdditiveProfileCycleAmountDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.AdditiveProfileRateDecimalPlaces  = ts.AdditiveProfileRateDecimalPlaces ,
ConsolidatedDB.dbo.tblSites.QuantityDisplayDefault  = ts.QuantityDisplayDefault ,--ConsolidatedDB.dbo.tblSites.VolumeDisplayDefault  = ts.VolumeDisplayDefault ,
ConsolidatedDB.dbo.tblSites.InhibitAccessAfterHours  = ts.InhibitAccessAfterHours ,
ConsolidatedDB.dbo.tblSites.InhibitMultipleCardIns  = ts.InhibitMultipleCardIns ,
ConsolidatedDB.dbo.tblSites.AccessCardinRequired  = ts.AccessCardinRequired ,
ConsolidatedDB.dbo.tblSites.CheckSiteNumber  = ts.CheckSiteNumber ,
ConsolidatedDB.dbo.tblSites.PromptForCustomerCard  = ts.PromptForCustomerCard ,
ConsolidatedDB.dbo.tblSites.PromptForTractorOrTanker  = ts.PromptForTractorOrTanker ,
ConsolidatedDB.dbo.tblSites.PromptForFirstTrailer  = ts.PromptForFirstTrailer ,
ConsolidatedDB.dbo.tblSites.PromptForSecondTrailer  = ts.PromptForSecondTrailer ,
ConsolidatedDB.dbo.tblSites.PromptForCompartment  = ts.PromptForCompartment ,
ConsolidatedDB.dbo.tblSites.EnforceDriverEquipmentMatch  = ts.EnforceDriverEquipmentMatch ,
ConsolidatedDB.dbo.tblSites.EnableAdditiveAccounting  = ts.EnableAdditiveAccounting ,
ConsolidatedDB.dbo.tblSites.UseCompanyEquipmentIdentifiers  = ts.UseCompanyEquipmentIdentifiers ,
ConsolidatedDB.dbo.tblSites.UseLastKnownGoodTankData  = ts.UseLastKnownGoodTankData ,
ConsolidatedDB.dbo.tblSites.MaximumLoadAmount  = ts.MaximumLoadAmount ,
ConsolidatedDB.dbo.tblSites.MaximumLoadTime  = ts.MaximumLoadTime ,
ConsolidatedDB.dbo.tblSites.MaximumIdleTime  = ts.MaximumIdleTime ,
ConsolidatedDB.dbo.tblSites.MaximumFlushAmount  = ts.MaximumFlushAmount ,
ConsolidatedDB.dbo.tblSites.MaximumMeterProvingAmount  = ts.MaximumMeterProvingAmount ,
ConsolidatedDB.dbo.tblSites.MaximumReturnsAmount  = ts.MaximumReturnsAmount ,
ConsolidatedDB.dbo.tblSites.MaximumNumberOfActiveArms  = ts.MaximumNumberOfActiveArms ,
ConsolidatedDB.dbo.tblSites.DriverTimeoutPeriod  = ts.DriverTimeoutPeriod ,
ConsolidatedDB.dbo.tblSites.DriverWarningPeriod  = ts.DriverWarningPeriod ,
ConsolidatedDB.dbo.tblSites.MaximumPrompts  = ts.MaximumPrompts ,
ConsolidatedDB.dbo.tblSites.InventoryTransactionAliasIndex  = ts.InventoryTransactionAliasIndex ,
ConsolidatedDB.dbo.tblSites.AdjustmentTransactionAliasIndex  = ts.AdjustmentTransactionAliasIndex ,
ConsolidatedDB.dbo.tblSites.MaximumVehicleWeight  = ts.MaximumVehicleWeight ,
ConsolidatedDB.dbo.tblSites.LoadByNet  = ts.LoadByNet ,
ConsolidatedDB.dbo.tblSites.PromptForShipmentNumber  = ts.PromptForShipmentNumber ,
ConsolidatedDB.dbo.tblSites.MaximumProductTemperature  = ts.MaximumProductTemperature ,
ConsolidatedDB.dbo.tblSites.ListEquipment  = ts.ListEquipment ,
ConsolidatedDB.dbo.tblSites.DeferStationChanges  = ts.DeferStationChanges ,
ConsolidatedDB.dbo.tblSites.InhibitBOLWithBrokenBlends  = ts.InhibitBOLWithBrokenBlends ,
ConsolidatedDB.dbo.tblSites.InhibitBOLWithImproperAdditization  = ts.InhibitBOLWithImproperAdditization ,
ConsolidatedDB.dbo.tblSites.InhibitOverweightBOL  = ts.InhibitOverweightBOL ,
ConsolidatedDB.dbo.tblSites.ExceptionBOLPrinter  = ts.ExceptionBOLPrinter ,
ConsolidatedDB.dbo.tblSites.EnableAutomaticBOLPrinting  = ts.EnableAutomaticBOLPrinting ,
ConsolidatedDB.dbo.tblSites.AutomaticBOLStartNumber  = ts.AutomaticBOLStartNumber ,
ConsolidatedDB.dbo.tblSites.AutomaticBOLEndNumber  = ts.AutomaticBOLEndNumber ,
ConsolidatedDB.dbo.tblSites.AutomaticBOLNextNumber  = ts.AutomaticBOLNextNumber ,
ConsolidatedDB.dbo.tblSites.SeparateManualBOLNumbering  = ts.SeparateManualBOLNumbering ,
ConsolidatedDB.dbo.tblSites.ManualBOLStartNumber  = ts.ManualBOLStartNumber ,
ConsolidatedDB.dbo.tblSites.ManualBOLEndNumber  = ts.ManualBOLEndNumber ,
ConsolidatedDB.dbo.tblSites.ManualBOLNextNumber  = ts.ManualBOLNextNumber ,
ConsolidatedDB.dbo.tblSites.TransactionStartNumber  = ts.TransactionStartNumber ,
ConsolidatedDB.dbo.tblSites.TransactionEndNumber  = ts.TransactionEndNumber ,
ConsolidatedDB.dbo.tblSites.TransactionNextNumber  = ts.TransactionNextNumber ,
ConsolidatedDB.dbo.tblSites.OrderStartNumber  = ts.OrderStartNumber ,
ConsolidatedDB.dbo.tblSites.OrderEndNumber  = ts.OrderEndNumber ,
ConsolidatedDB.dbo.tblSites.OrderNextNumber  = ts.OrderNextNumber ,
ConsolidatedDB.dbo.tblSites.NumberPrefix  = ts.NumberPrefix ,
ConsolidatedDB.dbo.tblSites.OpenTransactionWindow  = ts.OpenTransactionWindow ,
ConsolidatedDB.dbo.tblSites.AdministrativeLockDate  = ts.AdministrativeLockDate ,
ConsolidatedDB.dbo.tblSites.OperationalLockDate  = ts.OperationalLockDate ,
ConsolidatedDB.dbo.tblSites.MaximumDaysToRetainLogs  = ts.MaximumDaysToRetainLogs ,
ConsolidatedDB.dbo.tblSites.EnableDebugLogging  = ts.EnableDebugLogging ,
ConsolidatedDB.dbo.tblSites.EnableAuditLogging  = ts.EnableAuditLogging ,
ConsolidatedDB.dbo.tblSites.AutomaticallyPrintAlarmsAndEvents  = ts.AutomaticallyPrintAlarmsAndEvents ,
ConsolidatedDB.dbo.tblSites.AlarmAndEventPrinter  = ts.AlarmAndEventPrinter ,
ConsolidatedDB.dbo.tblSites.MailConnectMode  = ts.MailConnectMode ,
ConsolidatedDB.dbo.tblSites.DialupName  = ts.DialupName ,
ConsolidatedDB.dbo.tblSites.SCADASystem  = ts.SCADASystem ,
ConsolidatedDB.dbo.tblSites.InhibitTemplateGraphics  = ts.InhibitTemplateGraphics ,
ConsolidatedDB.dbo.tblSites.RefreshInterval  = ts.RefreshInterval ,
ConsolidatedDB.dbo.tblSites.InhibitEndOfDayOperations = ts.InhibitEndOfDayOperations,
ConsolidatedDB.dbo.tblSites.InhibitEndOfMonthOperations  = ts.InhibitEndOfMonthOperations ,
ConsolidatedDB.dbo.tblSites.EndOfDayWarningPeriod  = ts.EndOfDayWarningPeriod ,
ConsolidatedDB.dbo.tblSites.InhibitAutomaticPhysicalInventory  = ts.InhibitAutomaticPhysicalInventory ,
ConsolidatedDB.dbo.tblSites.InhibitAutomaticMeterCloseout = ts.InhibitAutomaticMeterCloseout,
ConsolidatedDB.dbo.tblSites.InhibitAutomaticReportGeneration  = ts.InhibitAutomaticReportGeneration ,
ConsolidatedDB.dbo.tblSites.InhibitAutomaticAdjustmentDistribution  = ts.InhibitAutomaticAdjustmentDistribution ,
ConsolidatedDB.dbo.tblSites.InhibitAutomaticCloseout  = ts.InhibitAutomaticCloseout ,
ConsolidatedDB.dbo.tblSites.InhibitTankScan  = ts.InhibitTankScan ,
ConsolidatedDB.dbo.tblSites.ReportDirectory  = ts.ReportDirectory ,
ConsolidatedDB.dbo.tblSites.ManageReports  = ts.ManageReports ,
ConsolidatedDB.dbo.tblSites.ManagedReportDirectory  = ts.ManagedReportDirectory ,
ConsolidatedDB.dbo.tblSites.VRURateLimit  = ts.VRURateLimit ,
ConsolidatedDB.dbo.tblSites.VRUHourlyLimit  = ts.VRUHourlyLimit ,
ConsolidatedDB.dbo.tblSites.VRUDailyLimit  = ts.VRUDailyLimit ,
ConsolidatedDB.dbo.tblSites.VRUYearlyLimit  = ts.VRUYearlyLimit ,
ConsolidatedDB.dbo.tblSites.VRUCurrentYearLimit  = ts.VRUCurrentYearLimit ,
ConsolidatedDB.dbo.tblSites.VRURateActual  = ts.VRURateActual ,
ConsolidatedDB.dbo.tblSites.VRUHourlyActual  = ts.VRUHourlyActual ,
ConsolidatedDB.dbo.tblSites.VRUDailyActual  = ts.VRUDailyActual ,
ConsolidatedDB.dbo.tblSites.VRUYearlyActual  = ts.VRUYearlyActual ,
ConsolidatedDB.dbo.tblSites.VRUCurrentYearActual  = ts.VRUCurrentYearActual ,
ConsolidatedDB.dbo.tblSites.VRURateLimitEnabled  = ts.VRURateLimitEnabled ,
ConsolidatedDB.dbo.tblSites.VRUHourlyLimitEnabled  = ts.VRUHourlyLimitEnabled ,
ConsolidatedDB.dbo.tblSites.VRUDailyLimitEnabled  = ts.VRUDailyLimitEnabled ,
ConsolidatedDB.dbo.tblSites.VRUYearlyLimitEnabled  = ts.VRUYearlyLimitEnabled ,
ConsolidatedDB.dbo.tblSites.VRUCurrentYearLimitEnabled  = ts.VRUCurrentYearLimitEnabled ,
ConsolidatedDB.dbo.tblSites.WatchdogPeriod  = ts.WatchdogPeriod ,
ConsolidatedDB.dbo.tblSites.WatchdogMode  = ts.WatchdogMode ,
ConsolidatedDB.dbo.tblSites.WatchdogCounterStart  = ts.WatchdogCounterStart ,
ConsolidatedDB.dbo.tblSites.WatchdogCounterEnd  = ts.WatchdogCounterEnd ,
ConsolidatedDB.dbo.tblSites.NumberGroupSizesType  = ts.NumberGroupSizesType ,
ConsolidatedDB.dbo.tblSites.NumberDecimalSeparator  = ts.NumberDecimalSeparator ,
ConsolidatedDB.dbo.tblSites.NumberGroupSeparator  = ts.NumberGroupSeparator ,
ConsolidatedDB.dbo.tblSites.ListSeparator  = ts.ListSeparator ,
ConsolidatedDB.dbo.tblSites.TimePattern  = ts.TimePattern ,
ConsolidatedDB.dbo.tblSites.TimeSeparator  = ts.TimeSeparator ,
ConsolidatedDB.dbo.tblSites.AMSymbol  = ts.AMSymbol ,
ConsolidatedDB.dbo.tblSites.PMSymbol  = ts.PMSymbol ,
ConsolidatedDB.dbo.tblSites.ShortDatePattern  = ts.ShortDatePattern ,
ConsolidatedDB.dbo.tblSites.DateSeparator  = ts.DateSeparator ,
ConsolidatedDB.dbo.tblSites.LongDatePattern  = ts.LongDatePattern ,
ConsolidatedDB.dbo.tblSites.TwoDigitCalendarEndYear  = ts.TwoDigitCalendarEndYear ,
ConsolidatedDB.dbo.tblSites.UserData1  = ts.UserData1 ,
ConsolidatedDB.dbo.tblSites.UserData2  = ts.UserData2 ,
ConsolidatedDB.dbo.tblSites.UserData3  = ts.UserData3 ,
ConsolidatedDB.dbo.tblSites.UserData4  = ts.UserData4 ,
ConsolidatedDB.dbo.tblSites.UserData5  = ts.UserData5 ,
ConsolidatedDB.dbo.tblSites.UserData6  = ts.UserData6 ,
ConsolidatedDB.dbo.tblSites.UserData7  = ts.UserData7 ,
ConsolidatedDB.dbo.tblSites.UserData8  = ts.UserData8 ,
ConsolidatedDB.dbo.tblSites.MinTimeAllowedToChangePwd  = ts.MinTimeAllowedToChangePwd ,
ConsolidatedDB.dbo.tblSites.MinPwdCharacterLength  = ts.MinPwdCharacterLength ,
ConsolidatedDB.dbo.tblSites.PwdExpirationInDays  = ts.PwdExpirationInDays ,
ConsolidatedDB.dbo.tblSites.PwdLockoutThreshold  = ts.PwdLockoutThreshold ,
ConsolidatedDB.dbo.tblSites.CheckForPreviousPwd  = ts.CheckForPreviousPwd ,
ConsolidatedDB.dbo.tblSites.StrongPwdUse  = ts.StrongPwdUse ,
ConsolidatedDB.dbo.tblSites.PwdHistoryCount  = ts.PwdHistoryCount ,
ConsolidatedDB.dbo.tblSites.ApplyToAllSiteMembers  = ts.ApplyToAllSiteMembers ,
ConsolidatedDB.dbo.tblSites.InactivityDisablePeriod  = ts.InactivityDisablePeriod ,
ConsolidatedDB.dbo.tblSites.EnforceSingleOwner  = ts.EnforceSingleOwner ,
ConsolidatedDB.dbo.tblSites.InhibitBOLSummaryAutoPopulate  = ts.InhibitBOLSummaryAutoPopulate ,
ConsolidatedDB.dbo.tblSites.InhibitOrderSummaryAutoPopulate  = ts.InhibitOrderSummaryAutoPopulate ,
ConsolidatedDB.dbo.tblSites.InhibitSupplyOrderSummaryAutoPopulate  = ts.InhibitSupplyOrderSummaryAutoPopulate ,
ConsolidatedDB.dbo.tblSites.InvoiceStartNumber  = ts.InvoiceStartNumber ,
ConsolidatedDB.dbo.tblSites.InvoiceEndNumber  = ts.InvoiceEndNumber ,
ConsolidatedDB.dbo.tblSites.InvoiceNextNumber  = ts.InvoiceNextNumber ,
ConsolidatedDB.dbo.tblSites.PromptForReturns  = ts.PromptForReturns ,
ConsolidatedDB.dbo.tblSites.PromptForTruckCard  = ts.PromptForTruckCard ,
ConsolidatedDB.dbo.tblSites.StartingShortCardNumber  = ts.StartingShortCardNumber ,
ConsolidatedDB.dbo.tblSites.UseShortCardNumber  = ts.UseShortCardNumber ,
ConsolidatedDB.dbo.tblSites.ExcessVarianceCount  = ts.ExcessVarianceCount ,
ConsolidatedDB.dbo.tblSites.ExcessVarianceTolerance  = ts.ExcessVarianceTolerance ,
ConsolidatedDB.dbo.tblSites.SecondaryStorageFillMethod  = ts.SecondaryStorageFillMethod ,
ConsolidatedDB.dbo.tblSites.DisableArchivePeriod = ts.DisableArchivePeriod
from
( SELECT
SPLCCode,
TimeZone,
AdjustForDaylightSavings,
LevelUnitIndex,
PressureUnitIndex ,
FlowUnitIndex ,
MassUnitIndex ,
AdditiveVolumeUnitIndex,
AdditiveProfileCycleAmountUnitIndex ,
AdditiveProfileRateUnitIndex ,
LevelDecimalPlaces ,
PressureDecimalPlaces ,
FlowDecimalPlaces ,
MassDecimalPlaces ,
AdditiveVolumeDecimalPlaces ,
AdditiveProfileCycleAmountDecimalPlaces ,
AdditiveProfileRateDecimalPlaces ,
QuantityDisplayDefault ,--QuantityDisplayDefault ,
InhibitAccessAfterHours ,
InhibitMultipleCardIns ,
AccessCardinRequired ,
CheckSiteNumber ,
PromptForCustomerCard ,
PromptForTractorOrTanker ,
PromptForFirstTrailer ,
PromptForSecondTrailer ,
PromptForCompartment ,
EnforceDriverEquipmentMatch ,
EnableAdditiveAccounting ,
UseCompanyEquipmentIdentifiers ,
UseLastKnownGoodTankData ,
MaximumLoadAmount ,
MaximumLoadTime ,
MaximumIdleTime ,
MaximumFlushAmount ,
MaximumMeterProvingAmount ,
MaximumReturnsAmount ,
MaximumNumberOfActiveArms ,
DriverTimeoutPeriod ,
DriverWarningPeriod ,
MaximumPrompts ,
InventoryTransactionAliasIndex ,
AdjustmentTransactionAliasIndex ,
MaximumVehicleWeight ,
LoadByNet ,
PromptForShipmentNumber ,
MaximumProductTemperature ,
ListEquipment ,
DeferStationChanges ,
InhibitBOLWithBrokenBlends ,
InhibitBOLWithImproperAdditization ,
InhibitOverweightBOL ,
ExceptionBOLPrinter ,
EnableAutomaticBOLPrinting ,
AutomaticBOLStartNumber ,
AutomaticBOLEndNumber ,
AutomaticBOLNextNumber ,
SeparateManualBOLNumbering ,
ManualBOLStartNumber ,
ManualBOLEndNumber ,
ManualBOLNextNumber ,
TransactionStartNumber ,
TransactionEndNumber ,
TransactionNextNumber ,
OrderStartNumber ,
OrderEndNumber ,
OrderNextNumber ,
NumberPrefix ,
OpenTransactionWindow ,
AdministrativeLockDate ,
OperationalLockDate ,
MaximumDaysToRetainLogs ,
EnableDebugLogging ,
EnableAuditLogging ,
AutomaticallyPrintAlarmsAndEvents ,
AlarmAndEventPrinter ,
MailConnectMode ,
DialupName ,
SCADASystem ,
InhibitTemplateGraphics ,
RefreshInterval ,
InhibitEndOfDayOperations,
InhibitEndOfMonthOperations ,
EndOfDayWarningPeriod ,
InhibitAutomaticPhysicalInventory ,
InhibitAutomaticMeterCloseout,
InhibitAutomaticReportGeneration ,
InhibitAutomaticAdjustmentDistribution ,
InhibitAutomaticCloseout ,
InhibitTankScan ,
ReportDirectory ,
ManageReports ,
ManagedReportDirectory ,
VRURateLimit ,
VRUHourlyLimit ,
VRUDailyLimit ,
VRUYearlyLimit ,
VRUCurrentYearLimit ,
VRURateActual ,
VRUHourlyActual ,
VRUDailyActual ,
VRUYearlyActual ,
VRUCurrentYearActual ,
VRURateLimitEnabled ,
VRUHourlyLimitEnabled ,
VRUDailyLimitEnabled ,
VRUYearlyLimitEnabled ,
VRUCurrentYearLimitEnabled ,
WatchdogPeriod ,
WatchdogMode ,
WatchdogCounterStart ,
WatchdogCounterEnd ,
NumberGroupSizesType ,
NumberDecimalSeparator ,
NumberGroupSeparator ,
ListSeparator ,
TimePattern ,
TimeSeparator ,
AMSymbol ,
PMSymbol ,
ShortDatePattern ,
DateSeparator ,
LongDatePattern ,
TwoDigitCalendarEndYear ,
UserData1 ,
UserData2 ,
UserData3 ,
UserData4 ,
UserData5 ,
UserData6 ,
UserData7 ,
UserData8 ,
MinTimeAllowedToChangePwd ,
MinPwdCharacterLength ,
PwdExpirationInDays ,
PwdLockoutThreshold ,
CheckForPreviousPwd ,
StrongPwdUse ,
PwdHistoryCount ,
ApplyToAllSiteMembers ,
InactivityDisablePeriod ,
EnforceSingleOwner ,
InhibitBOLSummaryAutoPopulate ,
InhibitOrderSummaryAutoPopulate ,
InhibitSupplyOrderSummaryAutoPopulate ,
InvoiceStartNumber ,
InvoiceEndNumber ,
InvoiceNextNumber ,
PromptForReturns ,
PromptForTruckCard ,
StartingShortCardNumber ,
UseShortCardNumber ,
ExcessVarianceCount ,
ExcessVarianceTolerance ,
SecondaryStorageFillMethod ,
DisableArchivePeriod from ConsolidatedDB.dbo.tblSites where SiteIndex = 1) ts
where ConsolidatedDB.dbo.tblSites.ID in (Select [ID] from #TMP_SITES)



INSERT INTO  [ConsolidatedDB].[dbo].[tblSiteToSiteMap]
(ParentSiteIndex,ChildSiteIndex,CreatedDate,CreatedBy)
SELECT -1,s.Siteindex,getdate(),'Varec' FROM [ConsolidatedDB].[dbo].tblSites s
JOIN #TMP_SITES ON s.ID = #TMP_SITES.ID
WHERE NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].[tblSiteToSiteMap] WHERE
 -1 = ParentSiteIndex AND s.SiteIndex = ChildSiteIndex)
 
INSERT INTO  [ConsolidatedDB].[dbo].[tblSiteToSiteMap]
(ParentSiteIndex,ChildSiteIndex,CreatedDate,CreatedBy)
SELECT s.Siteindex,s.Siteindex,getdate(),'Varec' FROM [ConsolidatedDB].[dbo].tblSites s
JOIN #TMP_SITES ON s.ID = #TMP_SITES.ID
WHERE NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].[tblSiteToSiteMap] WHERE
 s.SiteIndex = ParentSiteIndex AND s.SiteIndex = ChildSiteIndex)

--Index	SiteIndex	Type	TypeIndex	CreatedDate	CreatedBy	UpdatedDate	UpdatedBy	ID	DefaultView
--133	-1	2	1	2010-02-10 02:14:26.000	administrator	2010-03-03 18:17:18.000	administrator	DOD Standard	1

  INSERT INTO  [ConsolidatedDB].[dbo].tblEntityToSiteMap (
	[Index]
	,[SiteIndex]
	,[TypeId]
	,CreatedBy
	,CreatedDate
	)
  SELECT 
	L.[Index]
	, S.SiteIndex
	, 'Ledger Views'
	, 'Varec'
	, GETDATE() 
  FROM [ConsolidatedDB].[dbo].tblListViews L, [ConsolidatedDB].[dbo].tblSites S JOIN #TMP_SITES T ON T.[ID] = S.[ID] 
  WHERE L.SiteIndex=-1 AND L.[ID]='DOD Standard'
    AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap 
		WHERE TypeID='Ledger Views' AND SiteIndex=S.SiteIndex AND [Index]=L.[Index]);
		
		

GO

