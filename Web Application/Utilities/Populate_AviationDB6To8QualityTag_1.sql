USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Populate_AviationDB6To8QualityTag_1]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Populate_AviationDB6To8QualityTag_1') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Populate_AviationDB6To8QualityTag_1
GO

CREATE PROCEDURE [dbo].Populate_AviationDB6To8QualityTag_1 

 /*=============================================
 Author:			URVI PATEL
 Create date:		2/2/2010
Description:		Migrating AviationDB6.0 Quality Tag Defination to ConsolidatedDB8.0 QualityTags
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Populate_AviationDB6To8QualityTag_1 1

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL

AS 
BEGIN

IF @IsBaseDB = 2
BEGIN
		RETURN
END

SET @SiteID = NULL

DECLARE @SiteIndex6 int
DECLARE @SiteIndex8 int

DECLARE SiteIndexes_Cursor CURSOR FOR SELECT S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex

/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION
	
*/

OPEN SiteIndexes_Cursor 
FETCH NEXT FROM SiteIndexes_Cursor INTO @SiteIndex6, @SiteIndex8 
WHILE @@FETCH_STATUS = 0 
BEGIN 
	
SELECT 
		@SiteIndex8		AS [SiteIndex]
		,[TAG_TYPE]-1	AS [Severity]
		,[TAG_NAME]		AS [Name]
		,[IS_ACTIVATE]	AS [Active]
		,GetDate()		AS [CreatedDate]
		,'Varec'		AS [CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TAG_INDEX]
INTO #TMP_QC_TAGS_DEFINED
FROM [AviationDB6].[dbo].QC_TAGS_DEFINED WHERE DeleteFlag = 0 
AND [TAG_NAME] NOT IN (SELECT [Name] FROM [ConsolidatedDB].[dbo].tblQualityTags WHERE SiteIndex IN (@SiteIndex8, -1)) 

INSERT INTO [ConsolidatedDB].[dbo].tblQualityTags
(
       [SiteIndex]
      ,[Name]
      ,[Severity]
      ,[Active]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
)
SELECT
       [SiteIndex]
      ,[Name]
      ,[Severity]
      ,[Active]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
FROM #TMP_QC_TAGS_DEFINED

DROP TABLE #TMP_QC_TAGS_DEFINED

FETCH NEXT FROM SiteIndexes_Cursor INTO @SiteIndex6 , @SiteIndex8
END 
CLOSE SiteIndexes_Cursor 
DEALLOCATE SiteIndexes_Cursor; 


--------------------------------------------------------------------
-- Populate sites that do not have any QC test results 
-- even though they have equipment
--------------------------------------------------------------------
SELECT s8.SiteIndex AS SiteIndex8, s6.SiteIndex AS SiteIndex6, ID 
INTO #TMPSITES 
FROM ConsolidatedDB.dbo.tblSites s8, ConsolidatedDB6.dbo.tblSites s6
WHERE s6.DeleteFlag=0 AND (s8.SiteIndex > -1 AND s6.SiteIndex > -1)
	AND
	0 = (SELECT COUNT(*) FROM ConsolidatedDB.dbo.tblQualityTags WHERE SiteIndex=s8.SiteIndex) 
	AND 
	0 < (SELECT COUNT(*) FROM ConsolidatedDB.dbo.tblEntityToSiteMap WHERE TypeID='Equipment' AND SiteIndex=s8.SiteIndex)


SELECT 
		S.SiteIndex8	AS [SiteIndex]
		,[TAG_TYPE]-1	AS [Severity]
		,[TAG_NAME]		AS [Name]
		,[IS_ACTIVATE]	AS [Active]
		,GetDate()		AS [CreatedDate]
		,'Varec'		AS [CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TAG_INDEX]
INTO #TMP_QC_TAGS_DEFINED1
FROM [AviationDB6].[dbo].QC_TAGS_DEFINED, #TMPSITES S WHERE DeleteFlag = 0 
AND [TAG_NAME] NOT IN (SELECT [Name] FROM [ConsolidatedDB].[dbo].tblQualityTags 
		WHERE SiteIndex IN (s.SiteIndex8, -1)) 

INSERT INTO [ConsolidatedDB].[dbo].tblQualityTags
(
       [SiteIndex]
      ,[Name]
      ,[Severity]
      ,[Active]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
)
SELECT
       [SiteIndex]
      ,[Name]
      ,[Severity]
      ,[Active]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
FROM #TMP_QC_TAGS_DEFINED1

DROP TABLE #TMPSITES
DROP TABLE #TMP_QC_TAGS_DEFINED1
	
	/*
IF @@TRANCOUNT > 0    
BEGIN     
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
END