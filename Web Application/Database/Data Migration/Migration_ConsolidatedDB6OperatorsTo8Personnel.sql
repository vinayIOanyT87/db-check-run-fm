USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6OperatorsTo8Personnel]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'dbo.Migration_ConsolidatedDB6OperatorsTo8Personnel') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE dbo.Migration_ConsolidatedDB6OperatorsTo8Personnel
GO

CREATE PROCEDURE dbo.Migration_ConsolidatedDB6OperatorsTo8Personnel

 /*=============================================
 Author:			H Hunsaker
 Create date:		1/28/2010
 Description:		Migrating ConsolidatedDB 6.0 Operator data to the ConsolidatedDB 8.0 Personnel Table
 Modification History:
	Date		by			Description
	2/08/2010	Urvi Patel	Mapped all fields from Operators to Personnel 
 =============================================*/
/*

EXEC Migration_ConsolidatedDB6OperatorsTo8Personnel 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL

AS 
BEGIN


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex;

/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
	
*/
BEGIN TRANSACTION;

IF @IsBaseDB = 0
BEGIN	
	UPDATE	[ConsolidatedDB].[dbo].tblPersonnel SET SiteIndex=S.SiteIndex8 
	FROM #TMPSITES S 
		WHERE PersonID IN (SELECT E.[ID] FROM [ConsolidatedDB6].[dbo].tblOperators E WHERE DeleteFlag=0 AND SiteIndex=S.SiteIndex6 ) AND SiteIndex=-1;
END;


 SELECT  
	[OperatorIndex]
	,CASE WHEN @IsBaseDB = 0 THEN (SELECT TOP 1 SiteIndex8 FROM #TMPSITES) ELSE -1 END	AS [SiteIndex]
	,[ID]										AS [PersonID]						
	,''											AS [CardNumber]
	,0											AS [UserIndex]								
	,[FirstName]								AS [FirstName]									
	,ISNULL([MiddleName],'')					AS [MiddleName]						
	,[LastName]									AS [LastName]							
	,ISNULL([Title],'')							AS [Title]										
	,ISNULL([Department],'')					AS [Department]									
	,(SELECT EmployeeIndex FROM [ConsolidatedDB6].dbo.tblEmployees E 
				WHERE E.EmployeeID=A.[SupervisorID] AND E.DeleteFlag=0)							AS [SupervisorIndex]				
	,[SupervisorID]								AS [SupervisorID]								
	,ISNULL([Address1],'')						AS [Address1]						
	,ISNULL([Address2],'')						AS [Address2]									
	,ISNULL([City],'')							AS [City]												
	,ISNULL([State],'')							AS [State]									
	,ISNULL([Zip],'')							AS [Zip]													
	,ISNULL([Country],'')						AS [Country]												
	,ISNULL([Phone1],'')						AS [Phone1]										
	,ISNULL([Phone2],'')						AS [Phone2]											
	,ISNULL([AssignmentDate],GETDATE())			AS [AssignmentDate]										
	,ISNULL([SupervisionDate],GETDATE())		AS [SupervisionDate]											
	,''											AS [SSAN]										
	,ISNULL([BirthDate],GETDATE())				AS [BirthDate]											
	,ISNULL([PayRate],0)						AS [PayRate]																
	,ISNULL([LaborRate1],0)						AS [LaborRate1]												
	,ISNULL([LaborRate2],0)						AS [LaborRate2]													
	,ISNULL([LaborRate3],0)						AS [LaborRate3]												
	,ISNULL([LaborRate4],0)						AS [LaborRate4]														
	,0											AS [Status]			-- All migrated operator status set to enum value of 'In'											
	,GETDATE()									AS [CreatedDate]											
	,'Varec'									AS [CreatedBy]																	
	,[UpdatedDate]								AS [UpdatedDate]											
	,'Varec'								AS [UpdatedBy]														
	--  ,[DeleteFlag]																		
	,''											AS [Email]																			
	,0											AS [ResponsibleOfficer]													
	,0											AS [Shift]															
	,0											AS [CompanyIndex]									
	,''											AS [PINNumber]															
	,0											AS [PINRequired]														
	,0											AS [LockedOut]																
	,''											AS [LockedOutReason]																
	,GetDate()									AS [LockedOutDate]													
	,GETDATE()									AS [LastActivityDate]													
	,0											AS [CardedIn]														
	,''											AS [ShortCardNumber]														
	,0											AS [AssignedEquipmentIndex]											
	,null										AS [OnFileSignature]														
	,null										AS [UserData1]															
	,null										AS [UserData2]															
	,null										AS [UserData3]													
	,null										AS [UserData4]													
	,null										AS [UserData5]																
	,null										AS [UserData6]														
	,null										AS [UserData7]												
	,null										AS [UserData8]															
	,null										AS [UserData9]										
	,null										AS [UserData10]					
	,null										AS [UserData11]				
	,null										AS [UserData12]				
	,null										AS [UserData13]				
	,null										AS [UserData14]				
	,null										AS [UserData15]				
	,null										AS [UserData16]
	,null										AS [UserData17]
	,null										AS [UserData18]
	,null										AS [UserData19]
	,null										AS [UserData20]
	,null										AS [UserData21]
	,null										AS [UserData22]
	,null										AS [UserData23]
	,null										AS [UserData24]
INTO #TMP_OPERATORS	
FROM [ConsolidatedDB6].dbo.tblOperators A 
WHERE DeleteFlag = 0 ;


INSERT INTO ConsolidatedDB.dbo.tblPersonnel(
	 [SiteIndex]
	,[PersonID]
	,[CardNumber]
	,[UserIndex]
	,[FirstName]
	,[MiddleName]
	,[LastName]
	,[Title]
	,[Department]
	,[SupervisorIndex]
	,[Address1]
	,[Address2]
	,[City]
	,[State]
	,[Zip]
	,[Country]
	,[Phone1]
	,[Phone2]
	,[AssignmentDate]
	,[SupervisionDate]
	,[SSAN]
	,[BirthDate]
	,[PayRate]
	,[LaborRate1]
	,[LaborRate2]
	,[LaborRate3]
	,[LaborRate4]
	,[Status]
	,[Email]
	,[ResponsibleOfficer]
	,[Shift]
	,[CompanyIndex]
	,[PINNumber]
	,[PINRequired]
	,[LockedOut]
	,[LockedOutReason]
	,[LockedOutDate]
	,[LastActivityDate]
	,[CardedIn]
	,[ShortCardNumber]
	,[AssignedEquipmentIndex]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	--,[OnFileSignature]
	,[UserData1]
	,[UserData2]
	,[UserData3]
	,[UserData4]
	,[UserData5]
	,[UserData6]
	,[UserData7]
	,[UserData8]
	,[UserData9]
	,[UserData10]
	,[UserData11]
	,[UserData12]
	,[UserData13]
	,[UserData14]
	,[UserData15]
	,[UserData16]
	,[UserData17]
	,[UserData18]
	,[UserData19]
	,[UserData20]
	,[UserData21]
	,[UserData22]
	,[UserData23]
	,[UserData24]

 )
 
 SELECT  
	 [SiteIndex]
	,[PersonID]
	,[CardNumber]
	,[UserIndex]
	,[FirstName]
	,[MiddleName]
	,[LastName]
	,REPLACE(REPLACE([TITLE],',',''), '''', '')
	,[Department]
	,[SupervisorIndex]
	,[Address1]
	,[Address2]
	,[City]
	,[State]
	,[Zip]
	,[Country]
	,[Phone1]
	,[Phone2]
	,[AssignmentDate]
	,[SupervisionDate]
	,[SSAN]
	,[BirthDate]
	,[PayRate]
	,[LaborRate1]
	,[LaborRate2]
	,[LaborRate3]
	,[LaborRate4]
	,[Status]
	,[Email]
	,[ResponsibleOfficer]
	,[Shift]
	,[CompanyIndex]
	,[PINNumber]
	,[PINRequired]
	,[LockedOut]
	,[LockedOutReason]
	,[LockedOutDate]
	,[LastActivityDate]
	,[CardedIn]
	,[ShortCardNumber]
	,[AssignedEquipmentIndex]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	--,[OnFileSignature]
	,[UserData1]
	,[UserData2]
	,[UserData3]
	,[UserData4]
	,[UserData5]
	,[UserData6]
	,[UserData7]
	,[UserData8]
	,[UserData9]
	,[UserData10]
	,[UserData11]
	,[UserData12]
	,[UserData13]
	,[UserData14]
	,[UserData15]
	,[UserData16]
	,[UserData17]
	,[UserData18]
	,[UserData19]
	,[UserData20]
	,[UserData21]
	,[UserData22]
	,[UserData23]
	,[UserData24]
FROM #TMP_OPERATORS t
WHERE [PersonID] NOT IN (SELECT [PersonID] FROM [ConsolidatedDB].[dbo].tblPersonnel P WHERE P.SiteIndex IN (-1, t.SiteIndex));


INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
TypeID,
SiteIndex,
[Index],
CreatedDate,
CreatedBy
)
SELECT
'Personnel',
t.SiteIndex,
P.[PersonIndex],
T.CreatedDate,
T.CreatedBy
FROM [ConsolidatedDB].[dbo].tblPersonnel P JOIN #TMP_OPERATORS T ON P.PersonID=T.PersonID
WHERE P.SiteIndex IN (-1, t.SiteIndex) 
AND P.[PersonIndex] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap 
WHERE TypeID='Personnel' AND SiteIndex = t.SiteIndex);

SELECT P.PersonID, PersonIndex, OperatorIndex AS EmployeeIndex 
INTO #TMP_PERSON_INDEX_MAP 
FROM [ConsolidatedDB].[dbo].tblPersonnel P JOIN #TMP_OPERATORS T ON P.PersonID=T.PersonID
WHERE P.SiteIndex IN (-1, T.SiteIndex);

UPDATE [ConsolidatedDB].[dbo].tblPersonnel 
SET supervisorIndex = t.PersonIndex
FROM #TMP_PERSON_INDEX_MAP t
WHERE [ConsolidatedDB].[dbo].tblPersonnel.SupervisorIndex = t.EmployeeIndex;


--Assign person roles.
INSERT INTO [ConsolidatedDB].[dbo].tblPersonRoleMap (
	[PersonIndex],
	[Role],
	[CreatedDate],
	[CreatedBy]
	)
SELECT DISTINCT
	[PersonIndex],
	0,
	GETDATE(),
	'Varec'
FROM #TMP_PERSON_INDEX_MAP
WHERE PersonIndex NOT IN (SELECT PersonIndex FROM [ConsolidatedDB].[dbo].tblPersonRoleMap WHERE Role=0);


INSERT INTO [ConsolidatedDB].[dbo].tblPersonRoleMap (
	[PersonIndex],
	[Role],
	[CreatedDate],
	[CreatedBy]
	)
SELECT DISTINCT
	[SupervisorIndex],
	1,
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblPersonnel P 
JOIN [ConsolidatedDB].[dbo].tblEntityToSiteMap m ON P.[PersonIndex]=m.[Index] AND TypeID='Personnel'
JOIN #TMPSITES S ON m.SiteIndex = s.SiteIndex8
WHERE [SupervisorIndex] NOT IN (SELECT PersonIndex FROM [ConsolidatedDB].[dbo].tblPersonRoleMap WHERE Role=1)
AND [SupervisorIndex] IS NOT NULL;


DROP TABLE #TMP_OPERATORS;
DROP TABLE #TMP_PERSON_INDEX_MAP;
DROP TABLE #TMPSITES;


		
ROLLBACK TRANSACTION;     

/*

IF @@TRANCOUNT > 0    
BEGIN     
	COMMIT TRANSACTION  
END   
 
END TRY

BEGIN CATCH
IF @@TRANCOUNT > 0    
BEGIN     
	--SELECT  'ERROR: ' + ISNULL(@MSG,'Unknown Error')  as [Status]; 
	DECLARE @MSG nvarchar(MAX)
	SET @MSG = ERROR_MESSAGE()    
	RAISERROR  (@MSG,0,1)  
END  
END CATCH
*/
END