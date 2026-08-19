USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[Migration_FMD6OwnersToFMD8Owners]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migration_FMD6OwnersToFMD8Owners]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Migration_FMD6OwnersToFMD8Owners
GO

USE [ConsolidatedDB]
GO

CREATE PROCEDURE [dbo].Migration_FMD6OwnersToFMD8Owners
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/15/2010
 Description:		Migrating FuelsManager Defense 6.0 Owners to FuelsManager 8.0 Owners
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migration_FMD6OwnersToFMD8Owners ''

*/
@IsBaseDB bit,
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

if(@IsBaseDB = 1)
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
	BEGIN
		Select 'AviationDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Aviation Database before running this stored procedure';
		return
	END
	if((Select COUNT(SiteIndex) from tblSites) <> 2)
	BEGIN
		Select 'A base level site must have only two sites in the database.  The "SiteAdmin" site and the actual site.';
		return;
	END
	if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
	IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
		BEGIN
		Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
		return;
		END
END
ELSE
BEGIN
	if(isnull(@SiteID,'') <> '')
	BEGIN
		IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
			BEGIN
			Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
			return;
			END
	END
END

Create Table #tmpTableOwners
(
	keyField nvarchar(200) NOT NULL,
	SiteID6 nvarchar(30) NOT NULL,
	CompanyID nvarchar(50) NOT NULL,
	CompanyIndex8 int,
	SiteIndex8 int
);

--Get Distinct "Owners" from t_Acct_Tx1	
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx1 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--Get Distinct "Owners" from t_Acct_Tx3
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx3 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--Get Distinct "Owners" from t_Acct_Tx5
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx5 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--Get Distinct "Owners" from t_Acct_Tx8
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx8 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--Get Distinct "Owners" from t_Acct_Tx9
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2
from AccountingDB6.dbo.t_Acct_Tx9 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--Get Distinct "Owners" from t_Acct_Tx11
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx11 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--Get Distinct "Owners" from t_Acct_Tx12
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx12 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

--NO Distinct "Owners" from t_Acct_Tx14 because this table does not have an owner field.

--Get Distinct "Owners" from t_Acct_Tx15
Insert Into #tmpTableOwners
Select Distinct ts.SiteID + '_' + ta.Owner, ts.SiteID, ta.Owner,-1,-2 
from AccountingDB6.dbo.t_Acct_Tx15 ta,ConsolidatedDB6.dbo.tblSites ts where 
ts.SiteIndex = ta.SiteIndex AND
ts.SiteID + '_' + ta.Owner not in
(Select keyField from #tmpTableOwners)

Update #tmpTableOwners
Set SiteIndex8 = SiteIndex
from [ConsolidatedDB].[dbo].[tblSites]
where SiteID6 = ID


declare @siteIndex int;

if(@IsBaseDB = 1 or isnull(@SiteID,'') <> '')
	BEGIN
	Set @siteIndex = (Select MIN(SiteIndex) from [ConsolidatedDB].[dbo].[tblSites] where ID = @SiteID)
	END
ELSE
	BEGIN
	Set @siteIndex = (Select MIN(SiteIndex) from [ConsolidatedDB].[dbo].[tblSites] where ID = 'SiteAdmin');
	END

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
when 0 then -1 
else @siteIndex
END,

CompanyID,
'DOD',
'','','','','','','','','','','','',
CONVERT(date,getDate()),'2030-12-31',
NULL,0,0,0,0,0,0,'',
CONVERT(date,getDate()),
NULL,NULL,NULL,'','',getDate(),1,0,0,
'','','',0,0,0,'',
CONVERT(date,getDate()),
'','',0.00,0,
CONVERT(date,getDate()),
0,0,0,64,'','',0,0,0,0,0,'','','',
'','','','','','',getDate(),
'Varec',getDate(),'Varec'
from #tmpTableOwners where CompanyID not in
(Select Distinct [ID] from [ConsolidatedDB].[dbo].[tblCompanies]) 

Update #tmpTableOwners
Set CompanyIndex8 = CompanyIndex
from [ConsolidatedDB].[dbo].[tblCompanies]
where CompanyID = ID

--Assign companies to appropriate sites
if(@IsBaseDB = 0)
BEGIN
Insert Into [ConsolidatedDB].[dbo].[tblEntityToSiteMap]
Select 'Companies',-1,CompanyIndex8,GETDATE(),'Varec'
from #tmpTableOwners
where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[Index]) from [ConsolidatedDB].[dbo].[tblEntityToSiteMap] where TypeID = 'Companies')
And SiteIndex8 > -2
END
Insert Into [ConsolidatedDB].[dbo].[tblEntityToSiteMap]
Select 'Companies',SiteIndex8,CompanyIndex8,GETDATE(),'Varec'
from #tmpTableOwners
where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[Index]) from [ConsolidatedDB].[dbo].[tblEntityToSiteMap] where TypeID = 'Companies')
and SiteIndex8 > -2

--Assign companies to Owner To Role
Insert Into [ConsolidatedDB].[dbo].[tblCompanyRoleMap]
Select CompanyIndex8,1,GETDATE(),'Varec',SiteIndex8
from #tmpTableOwners
where CONVERT(nvarchar(20),SiteIndex8) + '_' + CONVERT(nvarchar(20),CompanyIndex8) not in
(Select CONVERT(nvarchar(20),SiteIndex) + '_' + CONVERT(nvarchar(20),[CompanyIndex]) from [ConsolidatedDB].[dbo].[tblCompanyRoleMap] where [Role] = 1)
and SiteIndex8 > -2
GO

