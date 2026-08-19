USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDBTrainingToConsolidatedDB8_3]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDBTrainingToConsolidatedDB8_3') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDBTrainingToConsolidatedDB8_3
GO

CREATE PROCEDURE [dbo].Migration_AviationDBTrainingToConsolidatedDB8_3
 /*=============================================
 Author:			Ali Coker
 Create date:		3/8/2010
 Description:		Migrating AviationDB Training_Items to tblqualification
 Modification History:
	Date		by			Description
	03/02/2010	A. Coker	Added migrating Training Item - Equipment Type association.
	
 =============================================*/
/*

EXEC Migration_AviationDBTrainingToConsolidatedDB8_3 1, null

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

/* Insert into Qualification */
SELECT TRAINING_ITEM_ID,
       5 AS [Type]	--  =PERSON_TRAINING
	  ,[ITEM_NAME] AS [ID]
      ,[DESCRIPTION] AS [Description] 
      ,S.SiteIndex8 AS [siteindex]
      ,[DURATION] AS [Duration]
      ,[RECURRENCE] AS [Reoccurrence]
      ,GETDATE() AS [CreatedDate]
      ,'Varec' AS [CreatedBy]
      ,[UpdatedDate] AS [UpdatedDate]
      ,[UpdatedBy] AS [UpdatedBy]
INTO #TMP
FROM [AviationDB6].[dbo].[TRAINING_ITEMS] TI, #TMPSITES S
WHERE deleteflag = 0  
	AND [ITEM_NAME] NOT IN (SELECT [ID] FROM [ConsolidatedDB].dbo.tblQUALIFICATIONS WHERE SiteIndex=S.SiteIndex8);



INSERT INTO [ConsolidatedDB].dbo.tblQUALIFICATIONS
(
	[Type]
	 ,[ID]
     ,[Description]
     ,[siteindex]  
     ,[Duration]
     ,[Reoccurrence]
	,[CreatedDate]
     ,[CreatedBy]
     ,[UpdatedDate]
     ,[UpdatedBy]
)
SELECT 
	[Type]
	 ,[ID]
     ,[Description]
     ,[siteindex]  
     ,[Duration]
     ,[Reoccurrence]
	 ,[CreatedDate]
     ,[CreatedBy]
     ,[UpdatedDate]
     ,[UpdatedBy]
FROM #TMP;


insert into [ConsolidatedDB].[dbo].[tblEntityToSiteMap] 
(
TypeID,
SiteIndex,
[index],
CreatedDate,
CreatedBy
)
SELECT
'Personnel Training',
#tmp.SiteIndex,
Q.[Index],
#tmp.CreatedDate,
#tmp.CreatedBy
FROM #tmp JOIN [ConsolidatedDB].dbo.tblQUALIFICATIONS Q ON #tmp.[ID] = Q.[ID] ;

/* Insert into QualificationMap */
--
-- Training Item - Person association
-- 
INSERT INTO [ConsolidatedDB].dbo.tblQualificationsMap
(
	[Index]
      ,[AssignedIndex]
      ,[Type]
      ,[Sequence]-----?????
      ,[Instructor]
      ,[DateCompleted]
      ,[DateDue] 
      ,[Rating]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[ExpirationDate]
      ,[HistoricalRecord]--????? ask Brent
)

SELECT p.PersonIndex
      ,qua8.[Index] --8.0.TBLQUALIFICATIONS.iNDEX   
      ,5 -- =PERSON_TRAINING_TO_PERSON
      ,0 -- =Sequence
      ,Qua6.[INSTRUCTOR]       
      ,[DATE_OF_QUALIFICATION]     
      ,[DATE_OF_EXPIRATION]
      ,[RATING]
      ,GETDATE()
      ,'Varec'
      ,Qua6.[UpdatedDate]
      ,Qua6.[UpdatedBy]
      ,[DATE_OF_EXPIRATION]
	  , 0
FROM [AviationDB6].[dbo].[QUALIFICATIONS] Qua6
JOIN #tmp titems6 
ON Qua6.TRAINING_ITEM_ID = titems6.TRAINING_ITEM_ID
JOIN [ConsolidatedDB].dbo.tblQualifications qua8
ON qua8.ID = titems6.ID 
JOIN #TMPSITES S
ON qua8.SiteIndex = S.SiteIndex8
JOIN [ConsolidatedDB6].dbo.tblEmployees emp 
ON emp.employeeindex = qua6.EMPLOYEE_INDEX
JOIN [ConsolidatedDB].dbo.tblPersonnel p
ON p.personid = emp.EmployeeID
WHERE qua6.deleteflag = 0 AND emp.DeleteFlag = 0 ;

--
-- Training Item - Equipment Type association
-- 
INSERT INTO [ConsolidatedDB].dbo.tblQualificationsMap
(
	[Index]
      ,[AssignedIndex]
      ,[Type]
      ,[Sequence]-----?????
      ,[DateCompleted]
      ,[DateDue] 
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[HistoricalRecord]--????? ask Brent
)

SELECT 
		eqt8.EqTypeIndex
      ,qua8.[Index] --8.0.TBLQUALIFICATIONS.iNDEX   
      ,7 -- =PERSON_TRAINING_TO_PERSON
      ,0 -- =Sequence
      ,eqtitems6.[CreatedDate]
      ,eqtitems6.[CreatedDate]
      ,GETDATE()
      ,'Varec'
      ,eqtitems6.[UpdatedDate]
      ,eqtitems6.[UpdatedBy]
	  , 0
FROM [AviationDB6].dbo.EQUIPMENT_TRAINING_ITEMS eqtitems6
JOIN  #tmp titems6 -- [AviationDB6].dbo.TRAINING_ITEMS titems6
ON eqtitems6.TRAINING_ITEM_ID = titems6.TRAINING_ITEM_ID
JOIN [ConsolidatedDB].dbo.tblQualifications qua8
ON qua8.ID = titems6.ID 
JOIN #TMPSITES S 
ON qua8.SiteIndex = S.SiteIndex8
JOIN [ConsolidatedDB6].dbo.tblEquipmentTypes eqt6 
ON eqtitems6.eq_type_index = eqt6.EqTypeIndex
JOIN [ConsolidatedDB].dbo.tblEquipmentTypes eqt8
ON eqt6.EqTypeName = eqt8.EqTypeName
WHERE eqtitems6.deleteflag = 0 AND eqt6.DeleteFlag = 0 ;

DROP TABLE #tmp;
DROP TABLE #tmpsites;

	
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