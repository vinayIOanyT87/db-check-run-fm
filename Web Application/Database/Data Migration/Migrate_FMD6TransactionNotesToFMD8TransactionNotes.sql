USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6CommonRequestToFMD8FuelCards]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6TransactionNotesToFMD8TransactionNotes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6TransactionNotesToFMD8TransactionNotes]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6TransactionNotesToFMD8TransactionNotes]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/16/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction notes to FuelsManager 8.0 tblTransactionNotes
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6TransactionNotesToFMD8TransactionNotes 2,null

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

declare @count int;


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END



SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1 
ORDER BY S6.SiteIndex

	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION

*/	
		--Insert Notes from t_Acct_Tx1
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx1 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null

		--Insert Notes from t_Acct_Tx3
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx3 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where -- ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)

		--Insert Notes from t_Acct_Tx5
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx5 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		and UPPER(tt.AliasName) NOT IN ('FILLSTAND', 'RETURN TO BULK')
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)

		--Insert Notes from t_Acct_Tx8
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx8 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)

		--Insert Notes from t_Acct_Tx9
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx9 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null

		--Insert Notes from t_Acct_Tx11
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx11 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)
		-- Set Notes for the conjoined
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx11 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.ConjoinTransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null

		--Insert Notes from t_Acct_Tx12
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx12 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)

		--Insert Notes from t_Acct_Tx14
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx14 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)

		--Insert Notes from t_Acct_Tx15
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),'''',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx15 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.TransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null
		-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)
		-- Same for conjoined
		Insert Into ConsolidatedDB.dbo.tblTransactionNotes
		Select 
		SubString(Replace(Replace(isnull(ta.Notes,''),';',''),':',''),1,1000),
		isnull(ta.CreatedBy,'Varec'),
		isnull(ta.CreatedDate,GETDATE()),
		ISNULL(ta.UpdatedBy,'Varec'),
		ISNULL(ta.UpdatedDate,GetDate()),
		tt.TransIndex,
		NULL
		from AccountingDB6.dbo.t_Acct_Tx15 ta
		JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
		JOIN ConsolidatedDB.dbo.tblTransactions tt
		ON tt.ConjoinTransID = ta.TransactionID
		where --ta.SiteIndex = @siteIndex6 and 
		tt.TransIndex is not null



	if(@IsBaseDB <> 2)
	BEGIN
		IF  EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
		BEGIN

			/*Special case for Recirculation.*/

			--Insert Notes from t_Acct_Tx15
			Insert Into ConsolidatedDB.dbo.tblTransactionNotes
			Select 
			SubString(replace(replace(replace(isnull(ta.Memo,''),';',''),':',''),'''',''),1,1000),
			isnull(ta.CreatedBy,'Varec'),
			isnull(ta.CreatedDate,GETDATE()),
			ISNULL(ta.UpdatedBy,'Varec'),
			ISNULL(ta.UpdatedDate,GetDate()),
			tt.TransIndex,
			NULL
			from AviationDB6.dbo.CONTROL_LOG ta
			JOIN #TMPSITES s ON s.SiteID8 = ta.[MANAGER]
			JOIN ConsolidatedDB.dbo.tblTransactions tt
			ON tt.TransID = ta.Transaction_ID
			where --ta.[MANAGER] = @siteID8  AND 
			REQUEST_TYPE IN ('RECIRC', 'RETURN TO BULK', 'FILL STAND') and tt.TransIndex is not null
			and ta.MEMO is not null and LEN(LTRIM(RTRIM(ta.MEMO))) > 0 
			-- and tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionNotes)

		END
	END


DELETE FROM #TMPSITES

--SELECT * FROM ConsolidatedDB.dbo.tblTransactionNotes


/*	
IF @@TRANCOUNT > 0    
BEGIN     
--ROLLBACK TRANSACTION; 
	    
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

