USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6CommonRequestToFMD8FuelCards]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6AirForceSALEUserDataToFMD8UserData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6AirForceSALEUserDataToFMD8UserData]
GO

USE [ConsolidatedDB]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6AirForceSALEUserDataToFMD8UserData]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6AirForceSALEUserDataToFMD8UserData ''

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
END

declare @siteIndex6 int;
declare @siteIndex8 int;

Set @siteIndex6 = (Select Min(Isnull(SiteIndex,0)) from ConsolidatedDB6.dbo.tblSites where SiteID = @SiteID);
Set @siteIndex8 = (Select Min(Isnull(SiteIndex,0)) from ConsolidatedDB.dbo.tblSites where ID = @SiteID);

declare @fill nvarchar(30);
declare @blank nvarchar(1);


Set @fill = '?';
Set @blank = '';

--Insert Notes from t_Acct_Tx1
Insert Into ConsolidatedDB.dbo.tblTransactionUserData
(UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,
 UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,
 UserData21,UserData22,UserData23,UserData24,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,TransIndex)
Select 
/* UserData1 */ @fill,
/* UserData2 */ta.UserData16,
/* UserData3 */ta.UserData2,
/* UserData4 */@blank,
/* UserData5 */ta.UserData4,
/* UserData6 */ta.UserData5,
/* UserData7 */@blank,
/* UserData8 */@blank,
/* UserData9 */CASE UPPER(ISNULL(ta.UserData11,'')) 
WHEN '2' THEN '2 (GOV BARGE)'
WHEN '8' THEN '8 (PIPELINE)'
WHEN '9' THEN '9 (LOCAL)'
WHEN 'A' THEN 'A (TRUCK)'
WHEN 'B' THEN 'B (MOTOR)'
WHEN 'I' THEN 'I (GOV TRUCK)'
WHEN 'K' THEN 'K (RAIL)'
WHEN 'V' THEN 'V (SEAVAN)'
WHEN 'W' THEN 'W (COM BARGE)'
WHEN 'Z' THEN 'Z (MSC TANKER)'
END,
/* UserData10 */@blank,
/* UserData11 */@blank,
/* UserData12 */@blank,
/* UserData13 */ta.UserData13,
/* UserData14 */ta.UserData14,
/* UserData15 */@blank,
/* UserData16 */ta.TicketNumber,
/* UserData17 */ta.UserData17,  --ODOMERTER READING
/* UserData18 */@blank,
/* UserData19 */ta.SubtypeCode1,
/* UserData20 */ta.SubtypeCode2,
/* UserData21 */ta.SubtypeCode3,
/* UserData22 */@blank,
/* UserData23 */@blank,
/* UserData24 */@blank,
ISNULL(ta.CreatedDate,GETDATE()),
ISNULL(ta.CreatedBy,'Varec'),
ISNULL(ta.UpdatedDate,getdate()),
ISNULL(ta.UpdatedBy,'Varec'),
tt.TransIndex
from AccountingDB6.dbo.t_Acct_Tx5 ta
LEFT JOIN ConsolidatedDB.dbo.tblTransactions tt
ON tt.TransID = ta.TransactionID
where 
ta.SiteIndex = @siteIndex6 and tt.TransIndex is not null and ta.Alias = 'SALE' and
tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionUserData)


