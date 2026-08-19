USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migration_FMD6ShippersToFMD8Shippers]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migration_FMD6ShippersToFMD8Shippers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Migration_FMD6ShippersToFMD8Shippers
GO

CREATE PROCEDURE [dbo].Migration_FMD6ShippersToFMD8Shippers
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/15/2010
 Description:		Migrating FuelsManager Defense 6.0 Shippers to FuelsManager 8.0 Shippers
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migration_FMD6ShippersToFMD8Shippers 2, null

*/
@IsBaseDb smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
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
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex;


DECLARE @date DATETIME
SET @date = GetDate()





/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION;
	

*/	
	Create Table #tmpTableShippers
	(
		keyField nvarchar(200) NOT NULL,
		SiteID6 nvarchar(30) NOT NULL,
		CompanyID nvarchar(50) NOT NULL,
		CompanyIndex8 int,
		SiteIndex8 int
	);


	--Get Distinct "Shippers" from t_Acct_Tx5
	Insert Into #tmpTableShippers
	Select Distinct ts.SiteID8 + '_' + ta.Manager, ts.SiteID8, ta.Manager,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx5 ta JOIN #TMPSITES ts ON ta.SiteIndex = ts.siteIndex6 where 
	ta.Alias = 'SHIPMENT' AND  
	isnull(ta.Manager,'') <> '' AND
	ts.SiteID8 + '_' + ta.Manager not in
	(Select keyField from #tmpTableShippers);

	Update #tmpTableShippers
	Set SiteIndex8 = SiteIndex
	from [ConsolidatedDB].[dbo].[tblSites]
	where SiteID6 = ID;

	Insert Into [ConsolidatedDB].[dbo].[tblCompanies]
	(
	SiteIndex,ID,Code,Name,Address1,Address2,City,
	[State],Zip,Country,Phone,FAX,EmergencyContact,
	EmergencyPhone,FlightPrefix,EffectiveDate,ExpirationDate,
	IATAIndex,OnHold,PickupFLights,StockTrack,SufferLossGain,
	LowStockWarning,LockedOut,LockedOutReason,LockedOutDate,
	ShipperTypeIndex,CustomerBillToTypeIndex,CustomerShipToTypeIndex,
	ReceivableAccount,RefinerCode,LastActivityDate,CreditOK,
	AdditiveAccounting,PurchaseOrderRequired,EPANumber,FederalID,
	TaxNumber,FlushPermitted,PumpOffPermitted,DeliveryToTerminalPermitted,
	LicenseNumber,LicenseExpiration,InsuranceCompany,InsurancePolicy,LiabilityAmount,
	HazardousMaterialExclusion,InsuranceExpiration,AllowDriverEntry,PINRequired,
	MaximumVehicleWeight,WeightUnits,AccountNumber,SCACCode,NotesIndex,
	DisableOwnerAllocationsCheck,DisableShipperAllocationsCheck,DisableBillToAllocationsCheck,
	DisableShipToAllocationsCheck,LoadRackDisplayText,
	UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,
	CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
	SELECT

	CASE @IsBaseDB 
	when 0 then t.siteIndex8 
	else -1
	END,

	CompanyID,
	'DOD',
	'','','','','','','','','','','','',
	CONVERT(date,@date),'2030-12-31',
	NULL,0,0,0,0,0,0,'',
	CONVERT(date,@date),
	NULL,NULL,NULL,'','',@date,1,0,0,
	'','','',0,0,0,'',
	CONVERT(date,@date),
	'','',0.00,0,
	CONVERT(date,@date),
	0,0,0,64,'','',0,0,0,0,0,'','','',
	'','','','','','',@date,
	'Varec',@date,'Varec'
	from #tmpTableShippers t where CompanyID not in
	(Select Distinct [ID] from [ConsolidatedDB].[dbo].[tblCompanies]) ;

	Update #tmpTableShippers
	Set CompanyIndex8 = CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies]
	where CompanyID = ID;

	--Assign companies to appropriate sites
	if(@IsBaseDB <> 0)
	BEGIN
	Insert Into [ConsolidatedDB].[dbo].[tblEntityToSiteMap]
	Select distinct 'Companies',-1,CompanyIndex8,@date,'Varec'
	from #tmpTableShippers
	where CONVERT(nvarchar(20),-1) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
	(Select CONVERT(nvarchar(20),-1) + '_' + CONVERT(nvarchar(20),[Index]) from [ConsolidatedDB].[dbo].[tblEntityToSiteMap] where TypeID = 'Companies')
	And SiteIndex8 > -2;
	END
	Insert Into [ConsolidatedDB].[dbo].[tblEntityToSiteMap]
	Select 'Companies',SiteIndex8,CompanyIndex8,@date,'Varec'
	from #tmpTableShippers
	where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
	(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[Index]) from [ConsolidatedDB].[dbo].[tblEntityToSiteMap] where TypeID = 'Companies')
	and SiteIndex8 > -2;

	--Assign companies to Shipper To Role
	Insert Into [ConsolidatedDB].[dbo].[tblCompanyRoleMap]
	Select CompanyIndex8,2,@date,'Varec',SiteIndex8
	from #tmpTableShippers
	where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
	(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[CompanyIndex]) from [ConsolidatedDB].[dbo].[tblCompanyRoleMap] where [Role] = 2)
	and SiteIndex8 > -2;
	
	/**/
	
	drop table #tmpTableShippers;
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