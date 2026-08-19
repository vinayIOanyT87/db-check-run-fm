USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_SetTransactionDocumentNumbers]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_SetTransactionDocumentNumbers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_SetTransactionDocumentNumbers]
GO

CREATE PROCEDURE [dbo].[Migrate_SetTransactionDocumentNumbers]
 /*=============================================
 Author:			A. Coker
 Create date:		4/16/2010
 Description:		Sets documentNumbers of transactions that don't have a documentNumber

 Modification History:
	Date		by			Description

 =============================================*/
/*

EXEC Migrate_SetTransactionDocumentNumbers 2, null 

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 


declare @count int


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END



SELECT S8.[ID], S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 ,
S8.TransactionNextNumber, S8.TransactionStartNumber, S8.TransactionEndNumber, S8.NumberPrefix AS Prefix
INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1;




/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION;
	
*/


UPDATE #TMPSITES SET Prefix=CONVERT(NVARCHAR, Consolidateddb.dbo.GetLocalTime(SiteIndex8, GETUTCDATE()), 112)
	WHERE Prefix = '%DATE%'
	
	SELECT TransIndex, Prefix + RIGHT('0000000000' + CAST((ROW_NUMBER() OVER(ORDER BY TransIndex)+S.TransactionNextNumber-1) - 
				floor((ROW_NUMBER() OVER(ORDER BY TransIndex) +S.TransactionNextNumber-1-S.TransactionStartNumber)/( S.TransactionEndNumber-S.TransactionStartNumber + 1)) 
				* (S.TransactionEndNumber-S.TransactionStartNumber + 1) AS NVARCHAR),10) AS DocumentNumber 
				INTO #TMP_TRANSACTIONS 
				FROM Consolidateddb.dbo.tblTransactions JOIN #TMPSITES S ON SiteIndex=S.siteIndex8
				WHERE  ISNULL(LTRIM(RTRIM(DocumentNumber)),'')=''
	UPDATE Consolidateddb.dbo.tblTransactions SET DocumentNumber = T.DocumentNumber
			FROM #TMP_TRANSACTIONS T WHERE Consolidateddb.dbo.tblTransactions.TransIndex=T.TransIndex
	DECLARE @TransCount bigint
	SELECT @TransCount = COUNT(*) FROM #TMP_TRANSACTIONS

	UPDATE [ConsolidatedDB].[dbo].tblSites SET TransactionNextNumber = 
				(@TransCount+[ConsolidatedDB].[dbo].tblSites.TransactionNextNumber) - 
					floor((@TransCount+[ConsolidatedDB].[dbo].tblSites.TransactionNextNumber-
					[ConsolidatedDB].[dbo].tblSites.TransactionStartNumber)/( 
					[ConsolidatedDB].[dbo].tblSites.TransactionEndNumber-
					[ConsolidatedDB].[dbo].tblSites.TransactionStartNumber + 1)) 
					* ([ConsolidatedDB].[dbo].tblSites.TransactionEndNumber-[ConsolidatedDB].[dbo].tblSites.TransactionStartNumber + 1)
		FROM #TMPSITES S WHERE SiteIndex=S.SiteIndex8


	DROP TABLE #TMP_TRANSACTIONS;
	
	DROP TABLE #TMPSITES;
 
 
 



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
