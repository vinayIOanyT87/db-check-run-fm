USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6QCAssignedTo8EqQualityTagLog]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6QCAssignedTo8EqQualityTagLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6QCAssignedTo8EqQualityTagLog
GO

CREATE PROCEDURE [dbo].Migration_AviationDB6QCAssignedTo8EqQualityTagLog
 /*=============================================
 Author:			URVI PATEL
 Create date:		2/2/2010
Description:		Migrating AviationDB6.0 Assigned Quality Tags to ConsolidatedDB8.0 Equipment Quality Tag Log
 Modification History:
	Date		by			Description
	03/14/2010	A. Coker	Completed script.
 =============================================*/
/*

EXEC Migration_AviationDB6QCAssignedTo8EqQualityTagLog 2, null

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

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex

	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	
*/

CREATE TABLE #TMP_QC_TAGS (
	[QualityTagIndex] [int] NOT NULL,
	[SiteIndex] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Severity] [smallint] NOT NULL,
	[Active] [bit] NOT NULL
	)
	
INSERT INTO #TMP_QC_TAGS 
SELECT [QualityTagIndex]
      ,S.SiteIndex8
      ,[Name]
      ,[Severity]
      ,[Active]
FROM [ConsolidatedDB].dbo.tblQualityTags JOIN #TMPSITES S 
ON SiteIndex=S.SiteIndex8

INSERT INTO #TMP_QC_TAGS 
SELECT [QualityTagIndex]
      ,S.SiteIndex8
      ,[Name]
      ,[Severity]
      ,[Active]
      FROM [ConsolidatedDB].dbo.tblQualityTags Q, #TMPSITES S  
WHERE SiteIndex = -1 AND Name NOT IN (SELECT Name FROM [ConsolidatedDB].dbo.tblQualityTags WHERE SiteIndex=S.SiteIndex8)

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
,S.SiteIndex8 
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
JOIN #TMPSITES S  
ON QT8.SiteIndex=S.SiteIndex8 AND E8.SiteIndex=S.SiteIndex8  
AND ET8.SiteIndex=S.SiteIndex8 AND M1.SiteIndex=S.SiteIndex8 AND M2.SiteIndex=S.SiteIndex8

/*Set TagNumber column to a unique non-zero value if zero*/
SELECT ROW_NUMBER() OVER(ORDER BY TAGGEDDATE) AS TAGNO, EquipmentQualityTagLogIndex AS [Index] INTO #TMP 
		FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog WHERE TagNumber = 0
DECLARE @LastTagNumber int
SELECT @LastTagNumber=MAX(TagNumber) FROM [ConsolidatedDB].dbo.tblEquipmentQualityTagLog
UPDATE [ConsolidatedDB].dbo.tblEquipmentQualityTagLog SET TagNumber = TAGNO + @LastTagNumber
FROM #TMP WHERE [Index]=EquipmentQualityTagLogIndex 
DROP TABLE #TMP

DROP TABLE #TMP_QC_TAGS
DROP TABLE #TMPSITES

 

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