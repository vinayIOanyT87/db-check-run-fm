USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData 2, null

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

declare @aliasName6 nvarchar(50);
declare @aliasName8 nvarchar(50);

Set @aliasName6 = 'SHIPMENT'
Set @aliasName8 = 'Shipment - Transfer'

declare @fill nvarchar(30);
declare @blank nvarchar(1);
declare @count int;

Set @fill = '?';
Set @blank = '';

IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END




SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1 AND
( Select isnull(COUNT(TransactionID),0) from AccountingDB6.dbo.t_Acct_Tx5 ta WHERE
	ta.SiteIndex = S6.siteIndex and 
	ta.Alias = @aliasName6 and 
	isnull(ta.UserData12,'') = '' and
	ta.TransactionID not in (Select TransactionID from AccountingDB6.dbo.t_Acct_FuelLoad)) > 0
ORDER BY S6.SiteIndex

	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION

*/


	--Insert Notes from t_Acct_Tx1
	Insert Into ConsolidatedDB.dbo.tblTransactionUserData
	(UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,
	 UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,
	 UserData21,UserData22,UserData23,UserData24,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,TransIndex)
	Select 
	/* UserData1 */ (Select top 1 isnull(UnitOfIssue,'U.S. Gallons') from ConsolidatedDB6.dbo.tblProducts where ProductID = ta.ProductID AND ta.SiteIndex = s.siteIndex6),
	/* UserData2 */ta.UserData16,
	/* UserData3 */CASE UPPER(ISNULL(ta.UserData3,'')) -- CHK 4/9/2010 Translate 6.0 abbrev to 8.0 text
	WHEN 'TCM' THEN 'Temperature Compensating Meters'
	WHEN 'NTCM' THEN 'Non-Temperature Compensating Meters'
	WHEN 'WC' THEN 'Weight Conversion'
	WHEN 'CTG' THEN 'Conveyance Tank Gauge'
	WHEN 'STG' THEN 'Shipping Tank Gauge'
	ELSE ''
	END,
	/* UserData4 */ta.UserData4,
	/* UserData5 */@blank,
	/* UserData6 */ta.UserData5,
	/* UserData7 */ta.UserData6,
	/* UserData8 */ta.UserData8,
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
	ELSE ''
	END,
	/* UserData10 */@blank,
	/* UserData11 */@blank,
	/* UserData12 */@blank,
	/* UserData13 */ta.UserData13,
	/* UserData14 */ta.UserData14,
	/* UserData15 */@blank,
	/* UserData16 */ta.TicketNumber,
	/* UserData17 */ta.UserData17,  
	/* UserData18 */@blank,
	/* UserData19 */isnull(tas.TankNumber,''),
	/* UserData20 */convert(nvarchar(20),isnull(tas.Density,0)),
	/* UserData21 */convert(nvarchar(20),isnull(tas.FlashPoint,0)),
	/* UserData22 */isnull(tas.Visual,''),
	/* UserData23 */convert(nvarchar(20),isnull(tas.FSII,0)),
	/* UserData24 */convert(nvarchar(20),isnull(tas.Conductivity,0)),
	ISNULL(ta.CreatedDate,GETDATE()),
	ISNULL(ta.CreatedBy,'Varec'),
	ISNULL(ta.UpdatedDate,getdate()),
	ISNULL(ta.UpdatedBy,'Varec'),
	tt.TransIndex
	from AccountingDB6.dbo.t_Acct_Tx5 ta
	JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
	LEFT JOIN AccountingDB6.dbo.t_Acct_ShippingRelease tas on
	tas.TransactionID = ta.TransactionID
	LEFT JOIN ConsolidatedDB.dbo.tblTransactions tt
	ON tt.TransID = ta.TransactionID
	WHERE
--	ta.SiteIndex = @siteIndex6 and 
	ta.Alias = @aliasName6 and 
	isnull(ta.UserData12,'') = '' and
	ta.TransactionID not in (Select TransactionID from AccountingDB6.dbo.t_Acct_FuelLoad)

drop table #tmpsites

--	SELECT u.* FROM ConsolidatedDB.dbo.tblTransactionUserData u JOIN ConsolidatedDB.dbo.tblTransactions t ON u.TransIndex=t.TransIndex AND aliasname=@aliasname8 order by u.TransIndex;


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