USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6CustomerAccountsToFMD8FuelCards]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6CustomerAccountsToFMD8FuelCards]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6CustomerAccountsToFMD8FuelCards]
GO


CREATE PROCEDURE [dbo].[Migrate_FMD6CustomerAccountsToFMD8FuelCards]
/*=============================================
 Author:			Eric Simmons
 Create date:		3/16/2010
 Description:		Migrating FuelsManager Defense 6.0 Customer Accounts to FuelsManager 8.0 FuelCards
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6CustomerAccountsToFMD8FuelCards 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 

IF NOT EXISTS(Select * from sys.databases where [name] = 'ConsolidatedDB6')
BEGIN
	Select 'ConsolidatedDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 ConsolidatedDB Database before running this stored procedure';
	return
END

IF NOT EXISTS(Select * from sys.databases where [name] = 'AccountingDB6')
BEGIN
	Select 'AccountingDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Accounting Database before running this stored procedure';
	return
END



if(@IsBaseDB <> 2)
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
	BEGIN
		Select 'AviationDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Aviation Database before running this stored procedure';
		return
	END
	/*if((Select COUNT(SiteIndex) from tblSites) <> 2)
	BEGIN
		Select 'A base level site must have only two sites in the database.  The "SiteAdmin" site and the actual site.';
		return;
	END*/
	/*if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
	IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
		BEGIN
		Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
		return;
		END
		*/
END
/*ELSE
BEGIN
	
	if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
		
	if(isnull(@SiteID,'') <> '')
	BEGIN
		IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
			BEGIN
			Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
			return;
			END
	END
END*/

IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID]
JOIN  AccountingDB6.dbo.t_Acct_CustomerInfo  a ON a.SiteIndex = s6.siteIndex
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex;

	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	
*/
	
	CREATE TABLE #tblFuelCards(
		[FuelCardID] [nvarchar] (100) NOT NULL,
		[SiteIndex] [int] NOT NULL,
		[SiteID] [nvarchar] (50) NOT NULL,
		[ID] [nvarchar](50) NOT NULL,
		[Provider] [nvarchar](50) NULL,
		[ActivationStatus] [int] NOT NULL,
		[ManagerIndex] [int] NULL,
		[ManagerID] [nvarchar] (50) NULL,
		[OwnerIndex] [int] NULL,
		[OwnerID] [nvarchar] (50) NULL,
		[ShipperIndex] [int] NULL,
		[ShipperID] [nvarchar] (50) NULL,
		[ShipToIndex] [int] NULL,
		[ShipToID] [nvarchar] (50) NULL,
		[BillToIndex] [int] NULL,
		[BillToID] [nvarchar] (50) NULL,
		[InactivityPeriod] [int] NULL,
		[Notes] [nvarchar](max) NULL,
		[StatusModifiedDate] [datetime] NOT NULL,
		[StatusModifiedBy] [nvarchar](50) NOT NULL,
		[UserData1] [nvarchar](60) NULL,
		[UserData2] [nvarchar](60) NULL,
		[UserData3] [nvarchar](60) NULL,
		[UserData4] [nvarchar](60) NULL,
		[UserData5] [nvarchar](60) NULL,
		[UserData6] [nvarchar](60) NULL,
		[UserData7] [nvarchar](60) NULL,
		[UserData8] [nvarchar](60) NULL,
		[CreatedDate] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](100) NOT NULL,
		[UpdatedDate] [datetime] NOT NULL,
		[UpdatedBy] [nvarchar](100) NOT NULL);
	
	Insert Into #tblFuelCards
	Select
	tac.FuelCardID,
	SiteIndex8,
	SiteID8,
	tac.FuelCardID,
	tac.Activity,
	0,
	-1,
	'DESC',
	NULL,
	'DESC',
	NULL,
	SiteID8,
	NULL,
	tac.AccountID,
	NULL,
	tac.SubAccountID,
	4,
	NULL,
	getDate(),
	'Varec',
	tac.UserData3,
	'1',
	tac.UserData4,
	tac.Activity,
	tac.UserData5,
	tac.DocumentID,
	NULL,
	NULL,
	getDate(),
	'Varec',
	getDate(),
	'Varec'
	from 
	(
		Select distinct 
			SiteIndex8,
			SiteID8,
			CASE 
			WHEN
				RTrim(LTrim(isnull(AccountID,'') + 
				isnull(SubAccountID,'') + 
				isnull(DocumentID,'') + 
				isnull(UserData3,'') + 
				isnull(UserData4,'') + 
				isnull(UserData5,'') )) = '' 
			THEN
				'Miscellaneous' 
			ELSE
				RTrim(LTrim(isnull(AccountID,'') + 
				isnull(SubAccountID,'') + 
				isnull(DocumentID,'') + 
				isnull(UserData3,'') + 
				isnull(UserData4,'') + 
				isnull(UserData5,'') )) 
			END as FuelCardID,
			isnull(AccountID,'') as AccountID,
			isnull(SubAccountID,'') as SubAccountID,
			isnull(DocumentID,'') as DocumentID,
			isnull(UserData3,'') as UserData3,
			isnull(UserData4,'') as UserData4,
			isnull(UserData5,'') as UserData5,
			'' as Activity 
		from AccountingDB6.dbo.t_Acct_CustomerInfo ta
		join #TMPSITES s ON  ta.SiteIndex = s.SiteIndex6) tac;
		
	
	Update #tblFuelCards Set #tblFuelCards.Provider = tac.Activity
	from 
	(Select	CASE WHEN
			RTrim(LTrim(isnull(AccountID,'') + 
			isnull(SubAccountID,'') + 
			isnull(DocumentID,'') + 
			isnull(UserData3,'') + 
			isnull(UserData4,'') + 
			isnull(UserData5,'') )) = '' THEN
			'Miscellaneous' ELSE
			RTrim(LTrim(isnull(AccountID,'') + 
			isnull(SubAccountID,'') + 
			isnull(DocumentID,'') + 
			isnull(UserData3,'') + 
			isnull(UserData4,'') + 
			isnull(UserData5,'') )) END as FuelCardID,
		CASE WHEN
			isnull(UserData2,'') = ''
			THEN
			RTrim(LTrim(isnull(AccountID,'') + 
			isnull(SubAccountID,'') + 
			isnull(DocumentID,'') + 
			isnull(UserData3,'') + 
			isnull(UserData4,'') + 
			isnull(UserData5,'') ))
			ELSE
			UserData2 END as Activity 
	from AccountingDB6.dbo.t_Acct_CustomerInfo) tac
	WHERE #tblFuelCards.FuelCardID = tac.FuelCardID;
		

	Update #tblFuelCards 
	Set ManagerIndex = tc.CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies] tc
	where ManagerID = tc.ID;
	
	Update #tblFuelCards 
	Set OwnerIndex = tc.CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies] tc
	where OwnerID = tc.ID;
	
	Update #tblFuelCards 
	Set ShipperIndex = tc.CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies] tc
	where ShipperID = tc.ID;
	
	Update #tblFuelCards 
	Set ShipToIndex = tc.CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies] tc
	where ShipToID = tc.ID;
	
	Update #tblFuelCards 
	Set BillToIndex = tc.CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies] tc
	where BillToID = tc.ID;
	
	Insert Into tblFuelCards
	Select SiteIndex,ID,Provider,ActivationStatus,ManagerIndex,OwnerIndex,ShipperIndex,ShipToIndex,
		   BillToIndex,InactivityPeriod,Notes,StatusModifiedDate,StatusModifiedBy,
		   UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,
		   CreatedDate,CreatedBy,UpdatedDate,UpdatedBy
		   from #tblFuelCards;
	       
	declare @createDate dateTime;
	Set @createDate = GETDATE();
	
	Insert Into tblEntityToSiteMap
	Select Distinct 'Fuel Card',SiteIndex,FuelCardIndex,@createDate,'Varec' from tblFuelCards 
	JOIN #TMPSITES s ON SiteIndex = s.SiteIndex8;
	
	--- CREATE EQUIPMENT
	Create Table #tempEquipment
	(
		SiteIndex int NOT NULL,
		EquipmentID nvarchar(200) NOT NULL,
		EquipmentIndex int NULL,
		XRef nvarchar(30) NULL,
		EqTypeName nvarchar(30),
		EqTypeIndex int NULL,
		ProductCode nvarchar(30),
		ProductIndex int NULL,
		Make nvarchar(30) NULL,
		Model nvarchar(30) NULL,
		Company nvarchar(30) NULL,
		CompanyIndex int NULL,
		UseCode nvarchar(2) NULL,
		FuelCardIndex int NULL
	);
	
	Insert Into
	#tempEquipment
	Select
		SiteIndex8,
		tce.RegistrationID,
		NULL,
		RIGHT(tce.RegistrationID,4),
		'Vehicle',
		NULL,
		tce.ProductCode,
		NULL,
		'',
		tce.Model,
		tce.AccountID,
		NULL,
		'',
		tfc.FuelCardIndex
		from
		(
			Select distinct 
				s.siteindex8,
				s.siteid8,
				CASE WHEN
				RTrim(LTrim(isnull(AccountID,'') + 
				isnull(SubAccountID,'') + 
				isnull(DocumentID,'') + 
				isnull(UserData3,'') + 
				isnull(UserData4,'') + 
				isnull(UserData5,'') )) = '' THEN
				'Miscellaneous' ELSE
				RTrim(LTrim(isnull(AccountID,'') + 
				isnull(SubAccountID,'') + 
				isnull(DocumentID,'') + 
				isnull(UserData3,'') + 
				isnull(UserData4,'') + 
				isnull(UserData5,'') )) END as FuelCardID,
				AccountID,
				SubAccountID,
				DocumentID,
				CASE WHEN
				isnull(UserData2,'') = ''
				THEN
				RTrim(LTrim(isnull(AccountID,'') + 
				isnull(SubAccountID,'') + 
				isnull(DocumentID,'') + 
				isnull(UserData3,'') + 
				isnull(UserData4,'') + 
				isnull(UserData5,'') ))
				ELSE
				UserData2 END as Activity,
				CustomerID as RegistrationID,
				ProductID as ProductCode,
				UserData1 as Model
			from AccountingDB6.dbo.t_Acct_CustomerInfo ta join #TMPSITES s on SiteIndex = s.siteIndex6
		  ) tce
		  INNER JOIN ConsolidatedDB.dbo.tblFuelCards tfc ON
		  tfc.ID = tce.FuelCardID and tfc.SiteIndex = tce.siteIndex8;
		  
		
	Update #tempEquipment
	Set 
	EqTypeIndex = (Select top 1 tet.EqTypeIndex from ConsolidatedDB.dbo.tblEquipmentTypes tet where tet.EqTypeName = #tempEquipment.EqTypeName and tet.SiteIndex in(s.SiteIndex8,-1) order by tet.SiteIndex DESC),
	ProductIndex = (Select top 1 tp.ProductIndex from ConsolidatedDB.dbo.tblProducts tp where tp.ProductCode = #tempEquipment.ProductCode and tp.SiteIndex in (s.SiteIndex8,-1) order by tp.SiteIndex DESC),
	CompanyIndex = (Select top 1 tc.CompanyIndex from ConsolidatedDB.dbo.tblCompanies tc where tc.ID = #tempEquipment.Company)
	from #TMPSITES s where s.SiteIndex8 = SiteIndex;
	
	Insert Into ConsolidatedDB.dbo.tblEquipment
	Select 
	/* SiteIndex*/ te.SiteIndex,
	/* ID*/	te.EquipmentID,
	/* Description*/ '',							
	/* EqTypeIndex*/ te.EqTypeIndex,
	/* Make*/ te.Make,
	/* Model*/ te.Model,
	/* Year*/ 0,
	/* IssPtNum*/ '',
	/* CompanyIndex*/ te.CompanyIndex,
	/* Fixed*/ 0,
	/* StorageType*/ '',
	/* InUse*/ 0,
	/* ProductIndex*/ te.ProductIndex,
	/* FuelCardIndex*/ te.FuelCardIndex,
	/* FixedVolume*/ 0,
	/* IntoPlane*/ 0,
	/* Mobile*/ 0,
	/* AttachedTo*/ '',
	/* MediaType*/ '',
	/* Meters*/ 0,
	/* DefuelMeterForwards*/ 0,
	/* PulseRatio*/ 1,
	/* Round*/ 0,
	/* Xref*/ te.XRef,
	/* LowStockWarning*/ 0,
	/* StockTrack*/ 0,
	/* Totalisor1*/ '',
	/* Totalisor2*/ '',
	/* FuelingState*/ '',
	/* Volume*/ 0,
	/* MeterReading*/ 0,
	/* Consecutive_OOS_Variance*/ 0,
	/* Notes*/ '',
	/* Capacity*/ 0,
	/* SafeFill*/ 0, 
	/* VolumeUnitIndex*/ 46,
	/* TemperatureUnitIndex*/ 2,
	/* DensityUnitIndex*/ 191,
	/* MassUnitIndex*/ 64,
	/* VolumeDecimalPlaces*/ 0,
	/* TemperatureDecimalPlaces*/ 0,
	/* DensityDecimalPlaces*/ 0,
	/* MassDecimalPlaces*/ 0,
	/* EquipmentIndex*/ NULL,
	/* EquipmentSequence*/ 0,
	/* LockedOut*/ 0,
	/* LockedOutReason*/ '',
	/* LockedOutDate*/ CONVERT(date,getDate()),
	/* SerialNumber*/ '',
	/* CompanyEquipmentID*/ NULL,
	/* TruckCardNumber*/ '',
	/* CreatedDate*/ GETDATE(),
	/* CreatedBy*/ 'Varec',
	/* UpdatedDate*/ GETDATE(),
	/* UpdatedBy*/ 'Varec',
	/* RatedGPM*/ 0,
	/* ActualGPM*/ 0,
	/* FuelAdditiveFlag*/ 0,
	/* ManufactureDate*/ NULL,
	/* InstallationDate*/ NULL,
	/* InspectionDate*/ NULL,
	/* CalibrationDate*/ NULL,
	/* QCDate*/ NULL,
	/* SecondaryStorageFlag*/ 0,
	/* ManagedEquipmentFlag*/ 0,
	/* FuelingType*/ 0,
	/* UserData1*/ '',
	/* UserData2*/ '',
	/* UserData3*/ '',
	/* UserData4*/ '',
	/* UserData5*/ '',
	/* UserData6*/ '',
	/* UserData7*/ '',
	/* UserData8*/ '',
	/* UserData9*/ '',
	/* UserData10*/ '',
	/* UserData11*/ '',
	/* UserData12*/ '',
	/* UserData13*/ '',
	/* UserData14*/ '',
	/* UserData15*/ '',
	/* UserData16*/ '',
	/* UserData17*/ '',
	/* UserData18*/ '',
	/* UserData19*/ '',
	/* UserData20*/ '',
	/* UserData21*/ '',
	/* UserData22*/ '',
	/* UserData23*/ '',
	/* UserData24*/ ''
	from #tempEquipment te;
	
	Update #tempEquipment Set EquipmentIndex = teq.[Index]
	from tblEquipment teq
	where teq.ID = #tempEquipment.EquipmentID;
	
	Set @createDate = GETDATE();
	
	Insert Into tblEntityToSiteMap
	Select Distinct
	'Equipment',
	te.siteIndex,
	te.EquipmentIndex,  
	@createDate,
	'Varec'
	from #tempEquipment te
	where 
	'Equipment_' +  convert(nvarchar(50),te.siteIndex) + '_' + convert(nvarchar(50),te.EquipmentIndex) not in
	(Select TypeID + '_' +  convert(nvarchar(50),SiteIndex) + '_' + convert(nvarchar(50),[Index]) from tblEntityToSiteMap);

	drop table #tblFuelCards;
	drop table #tempEquipment;
	drop table #tmpsites;
	

	 
	/*
IF @@TRANCOUNT > 0    
BEGIN     
--	ROLLBACK TRANSACTION;     
	COMMIT TRANSACTION  
END   
 
END TRY

BEGIN CATCH
IF @@TRANCOUNT > 0    
BEGIN     
	ROLLBACK TRANSACTION; 
	--SELECT  'ERROR: ' + ISNULL(@MSG,'Unknown Error')  as [Status]; 
	DECLARE @MSG nvarchar(MAX)
	SET @MSG = ERROR_MESSAGE()    
	RAISERROR  (@MSG,0,1)  
END  
END CATCH
	*/