USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migration_FMD6ConsumersToFMD8ShipTo]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migration_FMD6ConsumersToFMD8ShipTo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migration_FMD6ConsumersToFMD8ShipTo]
GO

CREATE PROCEDURE [dbo].Migration_FMD6ConsumersToFMD8ShipTo
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/15/2010
 Description:		Migrating FuelsManager Defense 6.0 Consumers to FuelsManager 8.0 ShipTo
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migration_FMD6ConsumersToFMD8ShipTo 1, 'FP5518'

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
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
AND NOT(isnull(@SiteID,'') <> '' and @SiteID <> S8.[ID])
ORDER BY S6.SiteIndex

DECLARE @date DATETIME
SET @date = GETDATE()




/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
	BEGIN TRANSACTION
*/
	
	Create Table #tmpTableConsumers
	(
		keyField nvarchar(200) NOT NULL,
		SiteID6 nvarchar(30) NOT NULL,
		CompanyID nvarchar(50) NOT NULL,
		CompanyIndex8 int,
		SiteIndex8 int
	);



	--Get Distinct "Consumers" from t_Acct_Tx3
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.Consumer, ts.SiteID8, tac.Consumer,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx3 tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6 
	where
	ISNULL(tac.Consumer,'') <> '' AND 
	ts.SiteID8 + '_' + tac.Consumer not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_Tx5
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.Consumer, ts.SiteID8, tac.Consumer,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx5 tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6  where 
	ISNULL(tac.Consumer,'') <> '' AND
	ts.SiteID8 + '_' + tac.Consumer not in
	(Select keyField from #tmpTableConsumers)
	
	--Get Distinct "UserData1 (SuppDoDAAC)" from t_Acct_Tx5
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.UserData1, ts.SiteID8, tac.UserData1,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx5 tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6 where 
	ISNULL(tac.UserData1,'') <> '' AND
	ts.SiteID8 + '_' + tac.UserData1 not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_Tx11
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.FromConsumer, ts.SiteID8, tac.FromConsumer,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx11 tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6 where 
	ISNULL(tac.FromConsumer,'') <> '' AND
	ts.SiteID8 + '_' + tac.FromConsumer not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_Tx11
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.ToConsumer, ts.SiteID8, tac.ToConsumer,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx11 tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6 where 
	ISNULL(tac.ToConsumer,'') <> '' AND
	ts.SiteID8 + '_' + tac.ToConsumer not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_Tx12
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.Consumer, ts.SiteID8, tac.Consumer,-1,-2 
	from AccountingDB6.dbo.t_Acct_Tx12 tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6  where 
	ISNULL(tac.Consumer,'') <> '' AND
	ts.SiteID8 + '_' + tac.Consumer not in
	(Select keyField from #tmpTableConsumers)


	--Get Distinct "Consumers" from t_Acct_CustomerInfo.AccountID
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.AccountID, ts.SiteID8, tac.AccountID,-1,-2 
	from AccountingDB6.dbo.t_Acct_CustomerInfo tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6  where 
	ISNULL(tac.AccountID,'') <> '' AND
	ts.SiteID8 + '_' + tac.AccountID not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_CustomerInfo.SubAccountID
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.SubAccountID, ts.SiteID8, tac.SubAccountID,-1,-2 
	from AccountingDB6.dbo.t_Acct_CustomerInfo tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6  where 
	ISNULL(tac.SubAccountID,'') <> '' AND
	ts.SiteID8 + '_' + tac.SubAccountID not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_Contracts.BuyerAccount
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.BuyerAccount, ts.SiteID8, tac.BuyerAccount,-1,-2 
	from AccountingDB6.dbo.t_Acct_Contracts tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6  where 
	ISNULL(tac.BuyerAccount,'') <> '' AND
	ts.SiteID8 + '_' + tac.BuyerAccount not in
	(Select keyField from #tmpTableConsumers)

	--Get Distinct "Consumers" from t_Acct_Contracts.Sub
	Insert Into #tmpTableConsumers
	Select Distinct ts.SiteID8 + '_' + tac.Sub, ts.SiteID8, tac.Sub,-1,-2 
	from AccountingDB6.dbo.t_Acct_Contracts tac JOIN #TMPSITES ts ON tac.SiteIndex = ts.siteIndex6  where 
	ISNULL(tac.Sub,'') <> '' AND
	ts.SiteID8 + '_' + tac.Sub not in
	(Select keyField from #tmpTableConsumers)

	if(@IsBaseDB <> 2)
		BEGIN
		
		--Get Distinct "Consumers" from tbl_CommonRequest_BillingInfo.Buyer_Account_No
		Insert Into #tmpTableConsumers
		Select Distinct s.SiteID8 + '_' + tac.Buyer_Account_No, s.SiteID8, tac.Buyer_Account_No,-1,-2 
		from AviationDB6.dbo.tbl_CommonRequest_BillingInfo tac, #TMPSITES S where 
		ISNULL(tac.Buyer_Account_No,'') <> '' AND
	s.SiteID8 + '_' + tac.Buyer_Account_No not in
		(Select keyField from #tmpTableConsumers)
		
		--Get Distinct "Consumers" from tbl_CommonRequest_BillingInfo.Buyer_SubAccount_No
		Insert Into #tmpTableConsumers
		Select Distinct s.SiteID8 + '_' + tac.Buyer_SubAccount_No, s.SiteID8, tac.Buyer_SubAccount_No,-1,-2 
		from AviationDB6.dbo.tbl_CommonRequest_BillingInfo tac, #TMPSITES S where 
		ISNULL(tac.Buyer_SubAccount_No,'') <> '' AND
		s.SiteID8 + '_' + tac.Buyer_SubAccount_No not in
		(Select keyField from #tmpTableConsumers)
		
		END


	--Update #tmpTableConsumers with the appropriate SiteIndex
	Update #tmpTableConsumers
	Set SiteIndex8 = SiteIndex
	from [ConsolidatedDB].[dbo].[tblSites]
	where SiteID6 = ID

	--Add missing companies to tblCompanies table.
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

	SiteIndex,

	ltrim(rtrim(CompanyID)),
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
	from (SELECT DISTINCT CompanyID, 
	CASE @IsBaseDB 
	when 0 then siteIndex8 
	else (Select MIN(SiteIndex) from [ConsolidatedDB].[dbo].[tblSites] where ID = 'SiteAdmin')
	END AS SiteIndex
	 FROM #tmpTableConsumers) t where CompanyID not in
	(Select Distinct [ID] from [ConsolidatedDB].[dbo].[tblCompanies]) 

	--Update #tmpTableConsumers with company indexes that match added records
	Update #tmpTableConsumers
	Set CompanyIndex8 = CompanyIndex
	from [ConsolidatedDB].[dbo].[tblCompanies]
	where CompanyID = ID

	--Assign companies to appropriate sites
	if(@IsBaseDB <> 0)
	BEGIN
		Insert Into [ConsolidatedDB].[dbo].[tblEntityToSiteMap]
		Select distinct 'Companies' as TypeID,-1 as xy,CompanyIndex8 as [Index],@date as CreatedDate,'Varec' as CreatedBy
		from #tmpTableConsumers
		where CONVERT(nvarchar(20),-1) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
		(Select CONVERT(nvarchar(20),-1) + '_' + CONVERT(nvarchar(20),[Index]) from [ConsolidatedDB].[dbo].[tblEntityToSiteMap] where TypeID = 'Companies')
		And SiteIndex8 > -2
		
	END
	Insert Into [ConsolidatedDB].[dbo].[tblEntityToSiteMap]
	Select 'Companies',SiteIndex8,CompanyIndex8,@date,'Varec'
	from #tmpTableConsumers
	where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
	(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[Index]) from [ConsolidatedDB].[dbo].[tblEntityToSiteMap] where TypeID = 'Companies')
	And SiteIndex8 > -2

	--Assign companies to Ship To Role
	Insert Into [ConsolidatedDB].[dbo].[tblCompanyRoleMap]
	Select CompanyIndex8,4,@date,'Varec',SiteIndex8
	from #tmpTableConsumers
	where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
	(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[CompanyIndex]) from [ConsolidatedDB].[dbo].[tblCompanyRoleMap] where [Role] = 4)
	And SiteIndex8 > -2

	--Assign companies to Bill To Role
	Insert Into [ConsolidatedDB].[dbo].[tblCompanyRoleMap]
	Select CompanyIndex8,3,@date,'Varec',SiteIndex8
	from #tmpTableConsumers
	where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
	(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[CompanyIndex]) from [ConsolidatedDB].[dbo].[tblCompanyRoleMap] where [Role] = 3)
	And SiteIndex8 > -2
	
	/*
	Insert Into [ConsolidatedDB].[dbo].[tblProductMap]
	(AssignedToIndex, AssignedIndex, [Type], createdby, UpdatedBy )
	Select CompanyIndex8, ProductIndex, 6, 'Varec', 'Varec' 
	from #tmpTableConsumers, [ConsolidatedDB].[dbo].[tblProducts] p join [ConsolidatedDB].[dbo].[tblEntityToSiteMap] m 
	on p.ProductIndex = m.[index] and m.SiteIndex=@siteIndex6 and TypeID='Products'
	where not exists (select * from [ConsolidatedDB].[dbo].[tblProductMap] 
	where AssignedToIndex=CompanyIndex8 and AssignedIndex=ProductIndex and [Type]=6)
	*/
	drop table #tmpTableConsumers;
	DROP TABLE #TMPSITES;	
	
	
	

/*	
IF @@TRANCOUNT > 0    
BEGIN     
--	   ROLLBACK TRANSACTION;  
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