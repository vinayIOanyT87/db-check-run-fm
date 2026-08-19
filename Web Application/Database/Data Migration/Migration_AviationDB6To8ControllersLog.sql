USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6To8ControllersLog]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8ControllersLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6To8ControllersLog
GO

CREATE PROCEDURE [dbo].Migration_AviationDB6To8ControllersLog 

 /*=============================================
 Author:			A. Coker
 Create date:		3/14/2010
Description:		Migrating AviationDB6.0 Controllers event log to ConsolidatedDB8.0 Controllers log
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_AviationDB6To8ControllersLog 1,null

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
ORDER BY S6.SiteIndex




/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION
	
*/

	

INSERT INTO [ConsolidatedDB].[dbo].[tblControllersLog]
(
       [SiteIndex]
      ,[EventTime]
      ,[Controller]
      ,[Memo]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
)
SELECT 
		S.SiteIndex8 
      ,[EVENT_TIME]
      ,[CONTROLLER_NAME]
      ,ISNULL(LEFT(replace(replace(isnull([MEMO],''),';',''),':',''),150),'')
      ,GETDATE()
      ,'Varec'
      ,[UpdatedDate]
      ,[UpdatedBy]
  FROM [AviationDB6].[dbo].[CONTROLLERS_EVENTLOG], #TMPSITES S 
  WHERE [DeleteFlag] = 0
 
 INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
TypeID,
SiteIndex,
[Index],
CreatedDate,
CreatedBy
)
SELECT
'ControllerLog',
SiteIndex,
[Index],
GETDATE(),
'Varec'
FROM [ConsolidatedDB].[dbo].[tblControllersLog] C JOIN #TMPSITES S 
ON SiteIndex = S.SiteIndex8 
AND [Index] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE SiteIndex=S.SiteIndex8 AND TypeID='ControllerLog' )

	
	
	
	   

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