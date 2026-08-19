USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6To8QualityTag_1]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8QualityTag_1') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6To8QualityTag_1
GO

CREATE PROCEDURE [dbo].Migration_AviationDB6To8QualityTag_1 

 /*=============================================
 Author:			URVI PATEL
 Create date:		2/2/2010
Description:		Migrating AviationDB6.0 Quality Tag Defination to ConsolidatedDB8.0 QualityTags
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_AviationDB6To8QualityTag_1 1, null

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

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex;


/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION;
	
*/

	
SELECT 
		S.SiteIndex8		AS [SiteIndex]
		,[TAG_TYPE]-1	AS [Severity]
		,[TAG_NAME]		AS [Name]
		,[IS_ACTIVATE]	AS [Active]
		,GetDate()		AS [CreatedDate]
		,'Varec'		AS [CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TAG_INDEX]
INTO #TMP_QC_TAGS_DEFINED 
FROM [AviationDB6].[dbo].QC_TAGS_DEFINED, #TMPSITES S WHERE DeleteFlag = 0 
AND [TAG_NAME] NOT IN (SELECT [Name] FROM [ConsolidatedDB].[dbo].tblQualityTags WHERE SiteIndex IN (S.SiteIndex8, -1)); 

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
FROM #TMP_QC_TAGS_DEFINED;

DROP TABLE #TMP_QC_TAGS_DEFINED;
DROP TABLE #TMPSITES;



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