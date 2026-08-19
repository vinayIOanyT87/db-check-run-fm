USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6To8TestResults]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.PopulateTestResults') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.PopulateTestResults
GO


CREATE PROCEDURE [dbo].PopulateTestResults
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

EXEC PopulateTestResults 2, null --'SB3100'

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL

AS 
BEGIN
/*
IF @IsBaseDB = 2
BEGIN
		RETURN
END
*/
--SET @SiteID = NULL

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
	
IF @IsBaseDB = 0
BEGIN
	UPDATE [ConsolidatedDB].[dbo].[tblTestSetDefinitions] SET OwnerSiteIndex= @SiteIndex8
		WHERE [TestSetName] IN (SELECT NAME FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION  
		WHERE DeleteFlag = 0 AND Class='Test') AND OwnerSiteIndex=-1
		
	UPDATE [ConsolidatedDB].[dbo].[tblTestDefinitions] SET OwnerSiteIndex= @SiteIndex8
		WHERE [TestName] IN (SELECT NAME FROM [AviationDB6].[dbo].QC_TEST_DEFINITION  
		WHERE DeleteFlag = 0 AND Class='Test') AND OwnerSiteIndex=-1 
		
END

---------------------------------------------------------
-- Test Set Definitions to migrate
---------------------------------------------------------
SELECT 
	TESTSET_ID,
	@SiteIndex8		AS [OwnerSiteIndex],
	NAME			AS [TestSetName],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_TESTSETDEFINITIONS 
FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION A 
WHERE DeleteFlag = 0 AND Class='Test' 


---------------------------------------------------------
-- Test Definitions to migrate
---------------------------------------------------------
SELECT 
	TEST_ID,
	@SiteIndex8		AS [OwnerSiteIndex],
	[NAME]			AS [TestName],
	[UNIT]			AS [MeasurementUnit],
	[RULE]			AS [ValidationRule],
	[SAMPLE_SIZE]	AS [SampleSize],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_TESTDEFINITIONS
FROM [AviationDB6].[dbo].QC_TEST_DEFINITION  A 
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
FROM #TMP_TESTDEFINITIONS
WHERE TestName NOT IN (SELECT TestName FROM [ConsolidatedDB].[dbo].[tblTestDefinitions] WHERE OwnerSiteIndex IN (-1,@SiteIndex8 ))



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
FROM #TMP_TESTSETDEFINITIONS
WHERE TestSetName NOT IN (SELECT TestSetName FROM [ConsolidatedDB].[dbo].[tblTestSetDefinitions] WHERE OwnerSiteIndex IN (-1,@SiteIndex8 ))


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
	@SiteIndex8,
	[TestDefinitionIndex],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblTestDefinitions A JOIN #TMP_TESTDEFINITIONS B 
ON A.[TestName] = B.[TestName] 
WHERE A.OwnerSiteIndex IN (-1, @SiteIndex8) AND
   [TestDefinitionIndex] NOT IN(SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Test' AND SiteIndex=@SiteIndex8) 

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
	@SiteIndex8,
	[TestSetDefinitionIndex],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions A JOIN #TMP_TESTSETDEFINITIONS B 
ON A.[TestSetName] = B.[TestSetName] 
WHERE A.OwnerSiteIndex IN (-1, @SiteIndex8) AND
	[TestSetDefinitionIndex] NOT IN(SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Test Set' AND SiteIndex=@SiteIndex8) 

DROP TABLE #TMP_TESTDEFINITIONS
DROP TABLE #TMP_TESTSETDEFINITIONS

SELECT TEST_ID AS TestDefinitionIndex6, T8.TestDefinitionIndex AS TestDefinitionIndex8
INTO #TMP_TEST_DEFINITION_MAP
FROM [AviationDB6].[dbo].QC_TEST_DEFINITION T6 JOIN [ConsolidatedDB].[dbo].tblTestDefinitions T8 
ON T6.NAME = T8.TestName 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=T8.TestDefinitionIndex AND TypeID='Test'
WHERE T6.DeleteFlag = 0 AND  M.SiteIndex = @SiteIndex8

SELECT TESTSET_ID AS TestSetDefinitionIndex6, T8.TestSetDefinitionIndex AS TestSetDefinitionIndex8
INTO #TMP_TESTSET_DEFINITION_MAP
FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION T6 JOIN [ConsolidatedDB].[dbo].tblTestSetDefinitions T8 
ON T6.NAME = T8.TestSetName 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=T8.TestSetDefinitionIndex AND TypeID='Test Set'
WHERE T6.DeleteFlag = 0 AND  M.SiteIndex = @SiteIndex8

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
	@SiteIndex8							AS SiteIndex,
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
FROM [AviationDB6].[dbo].QC_TESTSET_RESULT 
WHERE DeleteFlag = 0 AND Class='Test'  AND [SiteIndex] = @SiteIndex6

--select '+',* from #TMP_QC_TESTSET_RESULT 

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
WHERE A.DeleteFlag = 0 AND B.DeleteFlag = 0 AND C.DeleteFlag = 0 AND D.[SiteIndex] = @SiteIndex6



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
FROM #TMP_QC_TESTSET_RESULT A JOIN [ConsolidatedDB].[dbo].tblEquipment B ON A.EquipmentID = B.[ID] JOIN
[ConsolidatedDB].[dbo].tblEntityToSiteMap C ON B.[Index] = C.[Index]
WHERE C.SiteIndex=@SiteIndex8 AND TypeID='Equipment' AND [TestSetName] IN 
(SELECT [TestSetName] FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions WHERE OwnerSiteIndex=C.SiteIndex AND DeleteFlag = 0)
 
INSERT INTO [ConsolidatedDB].[dbo].tblTestEquipmentResults
(
	[TestSetEquipmentResultIndex],
	[TestName],
	[Measurement],
	[TestDate],
	[Status],
	[PerformedBy],
	[Supervisor],
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
	A.[Inspector],
	A.[Supervisor],
	A.CreatedDate,
	'Varec',
	A.UpdatedDate,
	A.UpdatedBy
FROM #TMP_QC_TEST_RESULT A JOIN #TMP_QC_TESTSET_RESULT B ON [TESTSET_RESULT_INDEX] = [TS_RESULT_INDEX]
JOIN [ConsolidatedDB].[dbo].tblTestSetEquipmentResults C ON B.[SampleNumber] = C.[SampleNumber] AND B.[TestSetName]=C.[TestSetName] AND C.SiteIndex=@SiteIndex8
WHERE A.[TestName] IN (SELECT [TestName] FROM [ConsolidatedDB].[dbo].tblTestDefinitions WHERE [OwnerSiteIndex] = C.SiteIndex)



DROP TABLE #TMP_QC_TESTSET_RESULT
DROP TABLE #TMP_QC_TEST_RESULT 

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
	0 = (SELECT COUNT(*) FROM ConsolidatedDB.dbo.tblTestSetEquipmentResults WHERE SiteIndex=s8.SiteIndex) 
	AND 
	0 < (SELECT COUNT(*) FROM ConsolidatedDB.dbo.tblEntityToSiteMap WHERE TypeID='Equipment' AND SiteIndex=s8.SiteIndex)

SELECT s.SiteIndex8, e.ID, m.[Index], q.* 
	INTO #tmp_qc
	FROM 
		ConsolidatedDB.dbo.tblEquipment e 
		JOIN ConsolidatedDB.dbo.tblEntityToSiteMap m ON e.[Index] = m.[Index]  AND TypeID='Equipment'
		JOIN #TMPSITES s ON m.SiteIndex = s.SiteIndex8,
	(SELECT * FROM [AviationDB6].[dbo].QC_TESTSET_RESULT WHERE TS_RESULT_INDEX IN
		(SELECT MAX(TS_RESULT_INDEX) FROM [AviationDB6].[dbo].QC_TESTSET_RESULT
		GROUP BY NAME, SAMPLE_SIZE, PASSED, IS_RETEST, PREVIOUS_SAMPLE_NUMBER,MEMO,GALLON_REP)) q
	WHERE q.DeleteFlag = 0 AND q.Class='Test'

DECLARE @MaxSampleNumber int
DECLARE @MinSampleNumber int
SELECT @MaxSampleNumber = MAX(SampleNumber) FROM ConsolidatedDB.dbo.tblTestSetEquipmentResults
SELECT @MinSampleNumber = MIN(Sample_Number) FROM [AviationDB6].[dbo].QC_TESTSET_RESULT

SELECT
	t.SiteIndex8						AS SiteIndex,
	[TS_RESULT_INDEX],
	[TS_RESULT_TIMESTAMP]				AS [ResultTimeStamp],
	[NAME]								AS [TestSetName],--testset definition name
	[ID]								AS [EquipmentID],
	[Index]								AS [EquipmentIndex],
	[SAMPLE_NUMBER] - @MinSampleNumber + @MaxSampleNumber + 1	AS [SampleNumber],
	[SAMPLE_SIZE]						AS [SampleSize],
	SUBSTRING(replace(replace(replace(isnull([MEMO],''),';',''),':',''),'''',''),1,1000)					AS [Memo],
	(CASE	WHEN [PASSED] = 1 
				THEN 1
			WHEN [PASSED] = 0 
				THEN 2 
			END)						AS [Status],
	[GALLON_REP]						AS [GallonsRepresented],
	[IS_RETEST]							AS [IsRetest],
	ISNULL([PREVIOUS_SAMPLE_NUMBER] - @MinSampleNumber + @MaxSampleNumber + 1,0)	AS [PreviousSampleNumber],
	''									AS [Inspector],
	''									AS [Supervisor],
	''									AS [DocumentNumber],
	0									AS [Override],
	CreatedDate,
	'Varec' AS CreatedBy,
	UpdatedDate,
	UpdatedBy	
INTO #TMP_QC_TESTSET_RESULT1 
FROM #tmp_qc t
	
SELECT 
	TESTSET_ID,
	s.SiteIndex8	AS [OwnerSiteIndex],
	NAME			AS [TestSetName],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_TESTSETDEFINITIONS1 
FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION A, #TMPSITES s
WHERE DeleteFlag = 0 AND Class='Test' 


---------------------------------------------------------
-- Test Definitions to migrate
---------------------------------------------------------
SELECT 
	TEST_ID,
	s.SiteIndex8	AS [OwnerSiteIndex],
	[NAME]			AS [TestName],
	[UNIT]			AS [MeasurementUnit],
	[RULE]			AS [ValidationRule],
	[SAMPLE_SIZE]	AS [SampleSize],
	A.CreatedDate,
	'Varec' AS CreatedBy,
	A.UpdatedDate,
	A.UpdatedBy	
INTO #TMP_TESTDEFINITIONS1
FROM [AviationDB6].[dbo].QC_TEST_DEFINITION  A , #TMPSITES s
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
FROM #TMP_TESTDEFINITIONS1 t
WHERE TestName NOT IN (SELECT TestName FROM [ConsolidatedDB].[dbo].[tblTestDefinitions] 
WHERE OwnerSiteIndex IN (-1,t.OwnerSiteIndex ))



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
FROM #TMP_TESTSETDEFINITIONS1 t
WHERE TestSetName NOT IN (SELECT TestSetName FROM [ConsolidatedDB].[dbo].[tblTestSetDefinitions] 
WHERE OwnerSiteIndex IN (-1,t.OwnerSiteIndex ))


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
FROM [ConsolidatedDB].[dbo].tblTestDefinitions A JOIN #TMP_TESTDEFINITIONS1 B 
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
FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions A JOIN #TMP_TESTSETDEFINITIONS1 B 
ON A.[TestSetName] = B.[TestSetName] 
WHERE A.OwnerSiteIndex IN (-1, B.OwnerSiteIndex) AND
	[TestSetDefinitionIndex] NOT IN(SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Test Set' AND SiteIndex=B.OwnerSiteIndex) 

DROP TABLE #TMP_TESTDEFINITIONS1
DROP TABLE #TMP_TESTSETDEFINITIONS1

SELECT TEST_ID AS TestDefinitionIndex6, T8.TestDefinitionIndex AS TestDefinitionIndex8
INTO #TMP_TEST_DEFINITION_MAP1
FROM [AviationDB6].[dbo].QC_TEST_DEFINITION T6 JOIN [ConsolidatedDB].[dbo].tblTestDefinitions T8 
ON T6.NAME = T8.TestName 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=T8.TestDefinitionIndex AND TypeID='Test'
JOIN #TMPSITES s ON s.SiteIndex8=M.SiteIndex
WHERE T6.DeleteFlag = 0

SELECT TESTSET_ID AS TestSetDefinitionIndex6, T8.TestSetDefinitionIndex AS TestSetDefinitionIndex8
INTO #TMP_TESTSET_DEFINITION_MAP1
FROM [AviationDB6].[dbo].QC_TESTSET_DEFINITION T6 JOIN [ConsolidatedDB].[dbo].tblTestSetDefinitions T8 
ON T6.NAME = T8.TestSetName 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=T8.TestSetDefinitionIndex AND TypeID='Test Set'
JOIN #TMPSITES s ON s.SiteIndex8=M.SiteIndex
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
FROM [AviationDB6].[dbo].QC_TS_TEST_MAP A JOIN #TMP_TEST_DEFINITION_MAP1 B ON A.TEST_ID = B.TestDefinitionIndex6 
INNER JOIN #TMP_TESTSET_DEFINITION_MAP1 C ON A.TESTSET_ID = C.TestSeTDefinitionIndex6 
INNER JOIN [ConsolidatedDB].[dbo].[tblTestDefinitions] D ON D.TestDefinitionIndex = B.TestDefinitionIndex8
INNER JOIN [ConsolidatedDB].[dbo].[tblTestSetDefinitions] E ON E.TestSetDefinitionIndex = C.TestSetDefinitionIndex8
WHERE  A.DeleteFlag = 0  
AND NOT EXISTS(SELECT TestDefinitionIndex, TestSetDefinitionIndex FROM [ConsolidatedDB].[dbo].[tblTestToTestSetMap]
WHERE TestDefinitionIndex = D.TestDefinitionIndex AND TestSetDefinitionIndex = E.TestSetDefinitionIndex)


DROP TABLE #TMP_TESTSET_DEFINITION_MAP1
DROP TABLE #TMP_TEST_DEFINITION_MAP1

---------------------------------------
	---------------------------------------------------------------------
	-- Determine Test Results to migrate based on Test Results referenced
	-- in Test Set Results.
	---------------------------------------------------------------------
	SELECT
		--D.SiteIndex,
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
	INTO #TMP_QC_TEST_RESULT1 
	FROM  
	[AviationDB6].[dbo].QC_TEST_RESULT A LEFT JOIN [AviationDB6].[dbo].QC_TS_TEST_RESULT_MAP B 
	ON A.[TEST_RESULT_INDEX] = B.[TEST_RESULT_INDEX] JOIN [AviationDB6].[dbo].QC_TEST_DEFINITION  C 
	ON A.TEST_ID = C.TEST_ID JOIN #tmp_qc D ON B.[TESTSET_RESULT_INDEX] = D.[TS_RESULT_INDEX]
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
		ISNULL((select TOP 1 LTRIM(RTRIM(Inspector)) from #TMP_QC_TEST_RESULT1 T 
			WHERE T.[TESTSET_RESULT_INDEX] = A.[TS_RESULT_INDEX] AND Inspector IS NOT NULL),'') AS Inspector,
		ISNULL((select TOP 1 LTRIM(RTRIM(supervisor)) from #TMP_QC_TEST_RESULT1 T   
			WHERE T.[TESTSET_RESULT_INDEX] = A.[TS_RESULT_INDEX] AND Supervisor IS NOT NULL ),'') AS Supervisor,
		DocumentNumber,
		EQUIPMENTINDEX,
		[EQUIPMENTID],
		[SampleNumber],
		[SampleSize],
		[Status],
		[IsRetest],
		[PreviousSampleNumber],
		[Memo],
		[GallonsRepresented],
		A.CreatedDate,
		'abc',
		A.UpdatedDate,
		A.UpdatedBy	
	FROM #TMP_QC_TESTSET_RESULT1 A 
	WHERE 
	[TestSetName] IN 
	(SELECT [TestSetName] FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions WHERE OwnerSiteIndex=A.SiteIndex AND DeleteFlag = 0)
	SELECT * FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions
	select * from #TMP_QC_TESTSET_RESULT1
-- EXEC PopulateTestResults 2, null --'SB3100'	
	INSERT INTO [ConsolidatedDB].[dbo].tblTestEquipmentResults
	(
		[TestSetEquipmentResultIndex],
		[TestName],
		[Measurement],
		[TestDate],
		[Status],
		[PerformedBy],
		[Supervisor],
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
		A.[Inspector],
		A.[Supervisor],
		A.CreatedDate,
		'abc',
		A.UpdatedDate,
		A.UpdatedBy
	FROM #TMP_QC_TEST_RESULT1 A JOIN #TMP_QC_TESTSET_RESULT1 B ON [TESTSET_RESULT_INDEX] = [TS_RESULT_INDEX] 
	JOIN [ConsolidatedDB].[dbo].tblTestSetEquipmentResults C ON B.[SampleNumber] = C.[SampleNumber] AND B.[TestSetName]=C.[TestSetName]
	AND B.SiteIndex=C.SiteIndex 
	WHERE A.[TestName] IN (SELECT [TestName] FROM [ConsolidatedDB].[dbo].tblTestDefinitions WHERE [OwnerSiteIndex] = C.SiteIndex)

	DROP TABLE #TMP_QC_TESTSET_RESULT1
	DROP TABLE #TMP_QC_TEST_RESULT1
	DROP TABLE #TMPSITES
	drop table #tmp_qc


--select COUNT(*) FROM [ConsolidatedDB].[dbo].tblTestDefinitions --where CreatedBy='Varec'
--select COUNT(*) FROM [ConsolidatedDB].[dbo].tblTestSetDefinitions --where CreatedBy='Varec'
--select * FROM [ConsolidatedDB].[dbo].tblTestSetEquipmentResults where CreatedBy='abc'-- group by siteindex
--select * FROM [ConsolidatedDB].[dbo].tblTestEquipmentResults where CreatedBy='abc'-- group by siteindex

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

update [AviationDB6].[dbo].QC_TESTSET_RESULT SET TEST_ITEM = TEST_ITEM + '_XYZ'
*/
END