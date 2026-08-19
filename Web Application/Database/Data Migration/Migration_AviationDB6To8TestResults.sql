USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6To8TestResults]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8TestResults') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6To8TestResults
GO


CREATE PROCEDURE [dbo].Migration_AviationDB6To8TestResults
 /*===============================================================================
 Author:			Ali Coker
 Create date:		3/5/2010
 Description:		Migrating AviationDB 6.0 Test Results to ConsolidatedDB 8.0
					
					Source tables in AviationDB 6.0:
					QC_TEST_DEFINITION
					QC_TESTSET_DEFINITION
					QC_TS_TEST_MAP
					QC_TEST_RESULT
					QC_TESTSET_RESULT
					QC_TEST_RESULT_MAP
					
					Destination tables in ConsolidatedDB 8.0:
					tblTestDefinitions
					tblTestSetDefinitions
					tblTestToTestSetMap
					tblTestEquipmentResults
					tblTestSetEquipmentResults
					tblEntityToSiteMap
					
 
 Modification History:
	Date		by			Description

	
 =================================================================================*/
/*

EXEC Migration_AviationDB6To8TestResults 1, null

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
IF @IsBaseDB = 0
BEGIN
	UPDATE [ConsolidatedDB].[dbo].[tblTestSetDefinitions] SET OwnerSiteIndex = S.SiteIndex8
		FROM #TMPSITES S WHERE [TestSetName] IN (SELECT NAME FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION  
		WHERE DeleteFlag = 0 AND Class='Test') AND OwnerSiteIndex=-1
		
	UPDATE [ConsolidatedDB].[dbo].[tblTestDefinitions] SET OwnerSiteIndex= S.SiteIndex8
		FROM #TMPSITES S WHERE [TestName] IN (SELECT NAME FROM [AviationDB6].[dbo].QC_TEST_DEFINITION  
		WHERE DeleteFlag = 0 AND Class='Test') AND OwnerSiteIndex=-1 
		
END

---------------------------------------------------------
-- Test Set Definitions to migrate
---------------------------------------------------------
SELECT 
	TESTSET_ID,
	S.SiteIndex8	AS [OwnerSiteIndex],
	NAME			AS [TestSetName],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_TESTSETDEFINITIONS 
FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION A, #TMPSITES S 
WHERE DeleteFlag = 0 AND Class='Test' 


---------------------------------------------------------
-- Test Definitions to migrate
---------------------------------------------------------
SELECT 
	TEST_ID,
	S.SiteIndex8	AS [OwnerSiteIndex],
	[NAME]			AS [TestName],
	[UNIT]			AS [MeasurementUnit],
	[RULE]			AS [ValidationRule],
	[SAMPLE_SIZE]	AS [SampleSize],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_TESTDEFINITIONS
FROM [AviationDB6].[dbo].QC_TEST_DEFINITION  A, #TMPSITES S 
WHERE DeleteFlag = 0 AND Class='Test' 


---------------------------------------------------------------
-- Migrate QC test results
---------------------------------------------------------------
INSERT INTO [ConsolidatedDB].[dbo].[tblTestDefinitions]
(
	OwnerSiteIndex,
	TestName,
	MeasurementUnit,
	ValidationRule,
	SampleSize,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy	
)
SELECT 
	OwnerSiteIndex,
	TestName,
	MeasurementUnit,
	ValidationRule,
	SampleSize,
	CreatedDate,
	'Varec',
	UpdatedDate,
	UpdatedBy	
FROM #TMP_TESTDEFINITIONS t
WHERE TestName NOT IN (SELECT TestName FROM [ConsolidatedDB].[dbo].[tblTestDefinitions] WHERE OwnerSiteIndex IN (-1,t.OwnerSiteIndex ))



INSERT INTO [ConsolidatedDB].[dbo].[tblTestSetDefinitions]
(
	OwnerSiteIndex,
	TestSetName,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy	
)
SELECT 
	OwnerSiteIndex,
	TestSetName,
	CreatedDate,
	'Varec',
	UpdatedDate,
	UpdatedBy	
FROM #TMP_TESTSETDEFINITIONS t
WHERE TestSetName NOT IN (SELECT TestSetName FROM [ConsolidatedDB].[dbo].[tblTestSetDefinitions] WHERE OwnerSiteIndex IN (-1,t.OwnerSiteIndex ))


---------------------------------------------------------------------
-- Set tblEntitySiteMap for Test
---------------------------------------------------------------------	
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	[TypeID],
	[SiteIndex],
	[Index],
	[CreatedDate],
	[CreatedBy]
)
SELECT
	'Test',
	B.OwnerSiteIndex,
	[TestDefinitionIndex],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblTestDefinitions A JOIN #TMP_TESTDEFINITIONS B 
ON A.[TestName] = B.[TestName] 
WHERE A.OwnerSiteIndex IN (-1, B.OwnerSiteIndex) AND
   [TestDefinitionIndex] NOT IN(SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Test' AND SiteIndex=B.OwnerSiteIndex) 

---------------------------------------------------------------------
-- Set tblEntitySiteMap for TestSet
---------------------------------------------------------------------	
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	[TypeID],
	[SiteIndex],
	[Index],
	[CreatedDate],
	[CreatedBy]
)
SELECT
	'Test Set',
	B.OwnerSiteIndex,
	[TestSetDefinitionIndex],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions A JOIN #TMP_TESTSETDEFINITIONS B 
ON A.[TestSetName] = B.[TestSetName] 
WHERE A.OwnerSiteIndex IN (-1, B.OwnerSiteIndex) AND
	[TestSetDefinitionIndex] NOT IN(SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Test Set' AND SiteIndex=B.OwnerSiteIndex) 

DROP TABLE #TMP_TESTDEFINITIONS
DROP TABLE #TMP_TESTSETDEFINITIONS

SELECT TEST_ID AS TestDefinitionIndex6, T8.TestDefinitionIndex AS TestDefinitionIndex8
INTO #TMP_TEST_DEFINITION_MAP
FROM [AviationDB6].[dbo].QC_TEST_DEFINITION T6 JOIN [ConsolidatedDB].[dbo].tblTestDefinitions T8 
ON T6.NAME = T8.TestName 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=T8.TestDefinitionIndex AND TypeID='Test'
JOIN #TMPSITES S ON M.SiteIndex = S.SiteIndex8
WHERE T6.DeleteFlag = 0 

SELECT TESTSET_ID AS TestSetDefinitionIndex6, T8.TestSetDefinitionIndex AS TestSetDefinitionIndex8
INTO #TMP_TESTSET_DEFINITION_MAP
FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION T6 JOIN [ConsolidatedDB].[dbo].tblTestSetDefinitions T8 
ON T6.NAME = T8.TestSetName 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=T8.TestSetDefinitionIndex AND TypeID='Test Set'
JOIN #TMPSITES S ON M.SiteIndex = S.SiteIndex8
WHERE T6.DeleteFlag = 0   

INSERT INTO [ConsolidatedDB].[dbo].[tblTestToTestSetMap]
(
	TestDefinitionIndex,
	TestSetDefinitionIndex,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy	
)
SELECT 
	D.TestDefinitionIndex,
	E.TestSetDefinitionIndex,
	A.CreatedDate,
	'Varec',
	A.UpdatedDate,
	A.UpdatedBy	
FROM [AviationDB6].[dbo].QC_TS_TEST_MAP A JOIN #TMP_TEST_DEFINITION_MAP B ON A.TEST_ID = B.TestDefinitionIndex6 
INNER JOIN #TMP_TESTSET_DEFINITION_MAP C ON A.TESTSET_ID = C.TestSeTDefinitionIndex6 
INNER JOIN [ConsolidatedDB].[dbo].[tblTestDefinitions] D ON D.TestDefinitionIndex = B.TestDefinitionIndex8
INNER JOIN [ConsolidatedDB].[dbo].[tblTestSetDefinitions] E ON E.TestSetDefinitionIndex = C.TestSetDefinitionIndex8
WHERE  A.DeleteFlag = 0  
AND NOT EXISTS(SELECT TestDefinitionIndex, TestSetDefinitionIndex FROM [ConsolidatedDB].[dbo].[tblTestToTestSetMap]
WHERE TestDefinitionIndex = D.TestDefinitionIndex AND TestSetDefinitionIndex = E.TestSetDefinitionIndex)


DROP TABLE #TMP_TESTSET_DEFINITION_MAP
DROP TABLE #TMP_TEST_DEFINITION_MAP
------------------------------------------------------------------------------
-- First determine TestSet Results that need to be migrated so that we know 
-- which Test Results,Test definitions and TestSet definitions need to be 
-- migrated.
------------------------------------------------------------------------------
SELECT
	S.SiteIndex8							AS SiteIndex,
	[TS_RESULT_INDEX],
	[TS_RESULT_TIMESTAMP]				AS [ResultTimeStamp],
	[NAME]								AS [TestSetName],--testset definition name
	[TEST_ITEM]							AS [EquipmentID],
	[SAMPLE_NUMBER]						AS [SampleNumber],
	[SAMPLE_SIZE]						AS [SampleSize],
	SUBSTRING(replace(replace(replace(isnull([MEMO],''),';',''),':',''),'''',''),1,1000)					AS [Memo],
	(CASE	WHEN [PASSED] = 1 
				THEN 1
			WHEN [PASSED] = 0 
				THEN 2 
			END)						AS [Status],
	[GALLON_REP]						AS [GallonsRepresented],
	[IS_RETEST]							AS [IsRetest],
	ISNULL([PREVIOUS_SAMPLE_NUMBER],0)	AS  [PreviousSampleNumber],
	''									AS [Inspector],
	''									AS [Supervisor],
	''									AS [DocumentNumber],
	0									AS [Override],
	CreatedDate,
	'Varec' AS CreatedBy,
	UpdatedDate,
	UpdatedBy	
INTO #TMP_QC_TESTSET_RESULT 
FROM [AviationDB6].[dbo].QC_TESTSET_RESULT Q JOIN #TMPSITES S ON [SiteIndex] = S.SiteIndex6
WHERE DeleteFlag = 0 AND Class='Test' 

---------------------------------------------------------------------
-- Determine Test Results to migrate based on Test Results referenced
-- in Test Set Results.
---------------------------------------------------------------------
SELECT
	A.[TEST_RESULT_INDEX],
	B.[TESTSET_RESULT_INDEX],
	C.[NAME]						AS [TestName],
	[MEASUREMENT]					AS [Measurement],
	[TEST_DATE]						AS [TestDate],
	(CASE 
		WHEN A.[PASSED] = 1 
			THEN 1 
		WHEN A.[PASSED] = 0 
			THEN 2 
		END)						AS [Status],
	[Inspector],
	[Supervisor],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_QC_TEST_RESULT 
FROM [AviationDB6].[dbo].QC_TEST_RESULT A LEFT JOIN [AviationDB6].[dbo].QC_TS_TEST_RESULT_MAP B 
ON A.[TEST_RESULT_INDEX] = B.[TEST_RESULT_INDEX] JOIN [AviationDB6].[dbo].QC_TEST_DEFINITION  C 
ON A.TEST_ID = C.TEST_ID JOIN [AviationDB6].[dbo].QC_TESTSET_RESULT D ON B.[TESTSET_RESULT_INDEX] = D.[TS_RESULT_INDEX]
JOIN #TMPSITES S ON D.[SiteIndex] = S.SiteIndex6
WHERE A.DeleteFlag = 0 AND B.DeleteFlag = 0 AND C.DeleteFlag = 0 


INSERT INTO [ConsolidatedDB].[dbo].tblTestSetEquipmentResults
(
	[SiteIndex],
	[ResultTimeStamp],
	[TestSetName],
	Inspector,
	Supervisor,
	DocumentNumber,
	[EquipmentIndex],
	[EquipmentID],
	[SampleNumber],
	[SampleSize],
	[Status],
	[IsRetest],
	[PreviousSampleNumber],
	[Memo],
	[GallonsRepresented],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy	
)
SELECT distinct
	A.[SiteIndex],
	[ResultTimeStamp],
	[TestSetName],
	ISNULL((select TOP 1 LTRIM(RTRIM(Inspector)) from #TMP_QC_TEST_RESULT T 
		WHERE T.[TESTSET_RESULT_INDEX] = A.[TS_RESULT_INDEX] AND Inspector IS NOT NULL),'') AS Inspector,
	ISNULL((select TOP 1 LTRIM(RTRIM(supervisor)) from #TMP_QC_TEST_RESULT T   
		WHERE T.[TESTSET_RESULT_INDEX] = A.[TS_RESULT_INDEX] AND Supervisor IS NOT NULL ),'') AS Supervisor,
	DocumentNumber,
	B.[Index],
	A.[EquipmentID],
	[SampleNumber],
	[SampleSize],
	[Status],
	[IsRetest],
	[PreviousSampleNumber],
	[Memo],
	[GallonsRepresented],
	A.CreatedDate,
	'Varec',
	A.UpdatedDate,
	A.UpdatedBy	
FROM 
 #TMPSITES S JOIN 
[ConsolidatedDB].[dbo].tblEntityToSiteMap C 
ON TypeID='Equipment'AND C.SiteIndex=S.SiteIndex8 
JOIN [ConsolidatedDB].[dbo].tblEquipment B 
ON B.[Index] = C.[Index]
JOIN #TMP_QC_TESTSET_RESULT A 
ON A.EquipmentID = B.[ID] 
WHERE
[TestSetName] IN 
(SELECT [TestSetName] FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions WHERE OwnerSiteIndex=C.SiteIndex AND DeleteFlag = 0)
 
 --select * from #TMP_PERSONNEL
--select * from #TMP_QC_TEST_RESULT
 --select * FROM [ConsolidatedDB].[dbo].tblTestSetEquipmentResults
----

INSERT INTO [ConsolidatedDB].[dbo].tblTestEquipmentResults
(
	[TestSetEquipmentResultIndex],
	[TestName],
	[Measurement],
	[TestDate],
	[Status],
--	[PerformedBy],
--	[Supervisor],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy
)
SELECT
	[TestSetEquipmentResultIndex],
	A.[TestName],
	A.[Measurement],
	A.[TestDate],
	A.[Status],
--	A.[Inspector],
--	A.[Supervisor],
	A.CreatedDate,
	'Varec',
	A.UpdatedDate,
	A.UpdatedBy
FROM 
 #TMPSITES S
JOIN [ConsolidatedDB].[dbo].tblTestSetEquipmentResults C  
ON C.SiteIndex=S.SiteIndex8 
JOIN #TMP_QC_TESTSET_RESULT B 
ON B.[SampleNumber] = C.[SampleNumber] AND B.[TestSetName]=C.[TestSetName]
JOIN #TMP_QC_TEST_RESULT A 
ON [TESTSET_RESULT_INDEX] = [TS_RESULT_INDEX]
WHERE A.[TestName] IN (SELECT [TestName] FROM [ConsolidatedDB].[dbo].tblTestDefinitions WHERE [OwnerSiteIndex] = C.SiteIndex)



DROP TABLE #TMP_QC_TESTSET_RESULT;
DROP TABLE #TMP_QC_TEST_RESULT;
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