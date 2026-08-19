USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6QCAssignedTo8EqQualityTagLog]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Populate_AviationDB6QCAssignedTo8EqQualityTagLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Populate_AviationDB6QCAssignedTo8EqQualityTagLog
GO

CREATE PROCEDURE [dbo].Populate_AviationDB6QCAssignedTo8EqQualityTagLog
 /*=============================================
 Author:			URVI PATEL
 Create date:		2/2/2010
Description:		Migrating AviationDB6.0 Assigned Quality Tags to ConsolidatedDB8.0 Equipment Quality Tag Log
 Modification History:
	Date		by			Description
	03/14/2010	A. Coker	Completed script.
 =============================================*/
/*

EXEC Populate_AviationDB6QCAssignedTo8EqQualityTagLog 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarchar(MAX) = NULL 

AS

BEGIN

IF @IsBaseDB = 2
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
	BEGIN
		RETURN
	END
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

CREATE TABLE #TMP_QC_TAGS (
	[QualityTagIndex] [int] NOT NULL,
	[SiteIndex] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Severity] [smallint] NOT NULL,
	[Active] [bit] NOT NULL
	)
	
INSERT INTO #TMP_QC_TAGS 
SELECT [QualityTagIndex]
      ,@SiteIndex8
      ,[Name]
      ,[Severity]
      ,[Active]
FROM [ConsolidatedDB].dbo.tblQualityTags 
WHERE SiteIndex=@SiteIndex8

INSERT INTO #TMP_QC_TAGS 
SELECT [QualityTagIndex]
      ,@SiteIndex8
      ,[Name]
      ,[Severity]
      ,[Active]
      FROM [ConsolidatedDB].dbo.tblQualityTags 
WHERE SiteIndex = -1 AND Name NOT IN (SELECT Name FROM [ConsolidatedDB].dbo.tblQualityTags WHERE SiteIndex=@SiteIndex8)

INSERT INTO [ConsolidatedDB].dbo.tblEquipmentQualityTagLog

(
       [QualityTagIndex]
      ,[QualityTagName]
      ,[EquipmentIndex]
      ,[EquipmentID]
      ,[EquipmentType]
      ,[TaggedDate]
      ,[TaggedBy]
      ,[Memo]
      ,[RemovedDate]
      ,[RemovedBy]
      ,[SiteIndex]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[TagNumber]
      ,DeleteFlag

)

SELECT --6.0   QC_TAGS_ASSIGNED
QT8.[QualityTagIndex]
,QT8.[Name]
,E8.[Index]
,REGISTRATION_ID --eqid
,ET8.[EqTypeName]
,DATE_ASSIGNED --tagdate
,TAGGED_BY 
,SubString(Replace(Replace(Replace(Isnull(REASON,''),';',''),':',''),'''',''),1,1000) 
,DATE_REMOVED 
,REMOVED_BY
,@SiteIndex8 
,QTA6.CreatedDate 
,'Varec' 
,QTA6.UpdatedDate 
,QTA6.UpdatedBy
,QTA6.[TAG_NUMBER]
,QTA6.DeleteFlag 
FROM AviationDB6.dbo.QC_TAGS_ASSIGNED QTA6
JOIN #TMP_QC_TAGS QT8
ON QTA6.TAG_NAME = QT8.Name
JOIN consolidatedDB.dbo.tblequipment E8
ON QTA6.REGISTRATION_ID = E8.ID
JOIN ConsolidatedDB.dbo.tblEntityToSiteMap M1
ON M1.TypeID='Equipment' AND M1.[Index]=E8.[Index]
JOIN consolidatedDB.dbo.tblEquipmentTypes ET8
ON  E8.EqTypeIndex = ET8.EqTypeIndex 
LEFT JOIN ConsolidatedDB.dbo.tblEntityToSiteMap M2
ON M2.TypeID='Equipment Type' AND M2.[Index]=ET8.[EqTypeIndex]
WHERE QT8.SiteIndex=@SiteIndex8 AND E8.SiteIndex=@SiteIndex8  
AND ET8.SiteIndex=@SiteIndex8 AND M1.SiteIndex=@SiteIndex8 AND M2.SiteIndex=@SiteIndex8

/*Set TagNumber column to a unique non-zero value if zero*/
SELECT ROW_NUMBER() OVER(ORDER BY TAGGEDDATE) AS TAGNO, EquipmentQualityTagLogIndex AS [Index] INTO #TMP 
		FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog WHERE TagNumber = 0
DECLARE @LastTagNumber int
SELECT @LastTagNumber=MAX(TagNumber) FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog
UPDATE [ConsolidatedDB].dbo.tblEquipmentQualityTagLog SET TagNumber = TAGNO + @LastTagNumber
FROM #TMP WHERE [Index]=EquipmentQualityTagLogIndex 
DROP TABLE #TMP

DROP TABLE #TMP_QC_TAGS

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
	0 = (SELECT COUNT(*) FROM ConsolidatedDB.dbo.tblEquipmentQualityTagLog WHERE SiteIndex=s8.SiteIndex) 
	AND 
	0 < (SELECT COUNT(*) FROM ConsolidatedDB.dbo.tblEntityToSiteMap WHERE TypeID='Equipment' AND SiteIndex=s8.SiteIndex)

CREATE TABLE #TMP_QC_TAGS1 (
	[QualityTagIndex] [int] NOT NULL,
	[SiteIndex] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Severity] [smallint] NOT NULL,
	[Active] [bit] NOT NULL
	)
	
INSERT INTO #TMP_QC_TAGS1 
SELECT [QualityTagIndex]
      ,SiteIndex
      ,[Name]
      ,[Severity]
      ,[Active]
FROM [ConsolidatedDB].dbo.tblQualityTags t JOIN #TMPSITES S ON t.SiteIndex = s.SiteIndex8 

INSERT INTO #TMP_QC_TAGS1 
SELECT [QualityTagIndex]
      ,S.SiteIndex8
      ,[Name]
      ,[Severity]
      ,[Active]
      FROM [ConsolidatedDB].dbo.tblQualityTags, #TMPSITES S 
WHERE SiteIndex = -1 AND Name NOT IN (SELECT Name FROM [ConsolidatedDB].dbo.tblQualityTags WHERE SiteIndex=S.SiteIndex8)

SELECT ROW_NUMBER() OVER(ORDER BY ID) AS L_Index8, ID INTO #TMPEQUIPMENT8 
FROM (SELECT DISTINCT e.ID FROM [ConsolidatedDB].dbo.tblEquipment e
JOIN [ConsolidatedDB].dbo.tblEntityToSiteMap m ON m.TypeID='Equipment' AND e.[Index]=m.[Index]
JOIN #TMPSITES S ON s.[SiteIndex8] = m.[SiteIndex] WHERE
e.ID NOT IN (SELECT EquipmentID FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog) AND 
e.ID NOT IN (SELECT REGISTRATION_ID FROM AviationDB6.dbo.QC_TAGS_ASSIGNED)) x

SELECT ROW_NUMBER() OVER(ORDER BY REGISTRATION_ID) AS L_Index6, REGISTRATION_ID INTO #TMPEQUIPMENT6  
FROM (SELECT DISTINCT REGISTRATION_ID FROM AviationDB6.dbo.QC_TAGS_ASSIGNED WHERE
DeleteFlag=0 /*AND 
REGISTRATION_ID NOT IN (SELECT EquipmentID FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog) AND 
REGISTRATION_ID NOT IN (SELECT ID FROM [ConsolidatedDB].dbo.tblEquipment)*/) x

SELECT DATE_ASSIGNED, ID AS REGISTRATION_ID, TAG_NAME, TAGGED_BY, REASON, DATE_REMOVED, REMOVED_BY,
CreatedDate,CreatedBy, UpdatedDate, UpdatedBy, DeleteFlag, TAG_INDEX, TAG_NUMBER 
INTO #tmpQC 
FROM AviationDB6.dbo.QC_TAGS_ASSIGNED q 
JOIN #TMPEQUIPMENT6 e6 
ON e6.REGISTRATION_ID=q.REGISTRATION_ID
JOIN #TMPEQUIPMENT8 e8 ON e6.L_Index6 = e8.L_Index8
/*
select * from #TMPEQUIPMENT8
select * from #TMPEQUIPMENT6
select '-', * from #TMP_QC_TAGS1

select '+', * from #tmpQC*/
DROP TABLE #TMPEQUIPMENT8
DROP TABLE #TMPEQUIPMENT6
--update AviationDB6.dbo.QC_TAGS_ASSIGNED set REGISTRATION_ID = substring(REGISTRATION_ID,1, LEN(REGISTRATION_ID)-4)
--select substring(REGISTRATION_ID, 1,LEN(REGISTRATION_ID)-4) from AviationDB6.dbo.QC_TAGS_ASSIGNED
--INSERT INTO AviationDB6.dbo.QC_TAGS_ASSIGNED 
--(DATE_ASSIGNED, REGISTRATION_ID, TAG_NAME, TAGGED_BY, REASON, DATE_REMOVED, REMOVED_BY,
--CreatedDate,CreatedBy, UpdatedDate, UpdatedBy, DeleteFlag, TAG_NUMBER)
--SELECT DATE_ASSIGNED, REGISTRATION_ID+'_XYZ', TAG_NAME, TAGGED_BY, REASON, DATE_REMOVED, REMOVED_BY,
--CreatedDate,CreatedBy, UpdatedDate, UpdatedBy, DeleteFlag, TAG_NUMBER 
--FROM AviationDB6.dbo.QC_TAGS_ASSIGNED q 
--delete from AviationDB6.dbo.QC_TAGS_ASSIGNED where REGISTRATION_ID like '%_XYZ'


INSERT INTO [ConsolidatedDB].dbo.tblEquipmentQualityTagLog
(
       [QualityTagIndex]
      ,[QualityTagName]
      ,[EquipmentIndex]
      ,[EquipmentID]
      ,[EquipmentType]
      ,[TaggedDate]
      ,[TaggedBy]
      ,[Memo]
      ,[RemovedDate]
      ,[RemovedBy]
      ,[SiteIndex]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[TagNumber]
      ,DeleteFlag

)
SELECT --6.0   QC_TAGS_ASSIGNED
QT8.[QualityTagIndex]
,QT8.[Name]
,E8.[Index]
,REGISTRATION_ID --eqid
,ET8.[EqTypeName]
,DATE_ASSIGNED --tagdate
,TAGGED_BY 
,SubString(Replace(Replace(Replace(Isnull(REASON,''),';',''),':',''),'''',''),1,1000) 
,DATE_REMOVED 
,REMOVED_BY
,QT8.SiteIndex 
,QTA6.CreatedDate 
,'Varec' 
,QTA6.UpdatedDate 
,QTA6.UpdatedBy
,QTA6.[TAG_NUMBER]
,QTA6.DeleteFlag 
FROM #tmpQC QTA6
JOIN #TMP_QC_TAGS1 QT8
ON QTA6.TAG_NAME = QT8.Name
JOIN consolidatedDB.dbo.tblequipment E8
ON QTA6.REGISTRATION_ID = E8.ID
JOIN ConsolidatedDB.dbo.tblEntityToSiteMap M1
ON M1.TypeID='Equipment' AND M1.[Index]=E8.[Index]
JOIN consolidatedDB.dbo.tblEquipmentTypes ET8
ON  E8.EqTypeIndex = ET8.EqTypeIndex 
LEFT JOIN ConsolidatedDB.dbo.tblEntityToSiteMap M2
ON M2.TypeID='Equipment Type' AND M2.[Index]=ET8.[EqTypeIndex]
JOIN #TMPSITES S ON QT8.SiteIndex=S.SiteIndex8 AND M1.SiteIndex=S.SiteIndex8
AND E8.SiteIndex=S.SiteIndex8  
AND ET8.SiteIndex=S.SiteIndex8 
AND M2.SiteIndex=S.SiteIndex8
--select '*', * FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog WHERE CreatedBy='xyz2'

/*Set TagNumber column to a unique non-zero value if zero*/
SELECT ROW_NUMBER() OVER(ORDER BY TAGGEDDATE) AS TAGNO, EquipmentQualityTagLogIndex AS [Index] INTO #TMP1 
		FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog WHERE TagNumber = 0
SELECT @LastTagNumber=MAX(TagNumber) FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog
UPDATE [ConsolidatedDB].dbo.tblEquipmentQualityTagLog SET TagNumber = TAGNO + @LastTagNumber
FROM #TMP1 WHERE [Index]=EquipmentQualityTagLogIndex 
DROP TABLE #TMP1

/*
select * FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog WHERE CreatedBy='xyz1' order by EquipmentID,siteindex
select '**',* FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog WHERE CreatedBy='xyz2' order by EquipmentID,siteindex
DROP TABLE #TMP_QC_TAGS1
DROP TABLE #TMPSITES
DROP TABLE #TMPQC*/
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