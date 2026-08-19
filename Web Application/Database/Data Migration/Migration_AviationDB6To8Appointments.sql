USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6To8Appointments]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8Appointments') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6To8Appointments
GO

CREATE PROCEDURE [dbo].Migration_AviationDB6To8Appointments
 /*=============================================
 Author:			URVI PATEL
 Create date:		2/2/2010
Description:		Migrating AviationDB6.0 Appointments to ConsolidatedDB8.0 Appointments
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_AviationDB6To8Appointments 1, null

*/

@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 
BEGIN

IF @IsBaseDB = 2
BEGIN
		RETURN;
END

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex;





	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION;
	
*/

	
SELECT PersonID, E6.EmployeeIndex, P8.PersonIndex, p8.LastName, P8.FirstName, P8.MiddleName  INTO #TMP_PERSON_INDEX_MAP
 FROM [ConsolidatedDB6].[dbo].tblEmployees E6 
 JOIN [ConsolidatedDB].[dbo].tblPersonnel P8  ON E6.EmployeeID = P8.PersonID 
 JOIN [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=P8.PersonIndex AND M.TypeID='Personnel' 
 JOIN #TMPSITES S ON  M.SiteIndex=S.SiteIndex8
 WHERE E6.DeleteFlag = 0; 
 
SELECT E8.[ID] AS EquipmentID, E6.EquipmentIndex AS EquipmentIndex6, E8.[Index] AS EquipmentIndex8 INTO #TMP_EQUIPMENT_INDEX_MAP
 FROM [ConsolidatedDB6].[dbo].tblEquipment E6 
 JOIN [ConsolidatedDB].[dbo].tblEquipment E8  ON E6.EquipmentID = E8.[ID]
 JOIN [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=E8.[Index] AND M.TypeID='Equipment' 
 JOIN #TMPSITES S ON  M.SiteIndex=S.SiteIndex8
WHERE E6.DeleteFlag = 0; 
 
 
 
SELECT 
	 S.SiteIndex8									AS 	[SiteIndex]				
	,ISNULL(CASE A.[ENTITY_TYPE] 
		WHEN 'Operators'
		THEN 'Personnel'
		ELSE A.[ENTITY_TYPE]
		END, 'Unknown')								AS [AssociatedType]						
	,ISNULL(CASE A.[ENTITY_TYPE] 
		WHEN 'Operators'
		THEN (SELECT PersonIndex FROM #TMP_PERSON_INDEX_MAP WHERE PersonID = A.[ENTITY_ID])
		ELSE (SELECT EquipmentIndex8 FROM #TMP_EQUIPMENT_INDEX_MAP WHERE EquipmentID = A.[ENTITY_ID])
		END,-1)										AS [AssociatedTypeIndex]					
	,ISNULL(CASE A.[ENTITY_TYPE] 
		WHEN 'Operators'
		THEN (SELECT RTRIM(LTRIM(RTRIM(LASTNAME)) + ' ' + 
					LTRIM(RTRIM(FIRSTNAME)) + ' ' + 
					LTRIM(RTRIM(MIDDLENAME))) FROM #TMP_PERSON_INDEX_MAP WHERE PersonID = A.[ENTITY_ID])
		ELSE A.[ENTITY_ID]
		END	,A.[ENTITY_ID])							AS [AssetText]							  
	,A.[START_TIME]									AS [StartDate]					  
	,A.[DURATION]									AS [Duration]					  
	,LEFT(A.[DESCRIPTION],50)						AS [Description]					  	
	,A.[APPOINTMENT_TYPE]							AS [AppointmentCategory]					 			
	,A.[USE_RESERVED] 								AS [ScheduleOnHolidays]			  																
	,A.[USE_WEEKENDS] 								AS [ScheduleOnWeekends]				  	  									
	,CASE [INTERVAL_TYPE] WHEN 1 THEN 1 ELSE 0 END	AS [AppointmentIsSingle]
	,(CASE [INTERVAL_PARAM_1]
		WHEN 3 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  [INTERVAL_PARAM_4]
				WHEN 2 THEN 1
				END)
		ELSE 1
		END) 										AS [AppointmentReoccuranceInterval]
	,(CASE [INTERVAL_PARAM_1]
		WHEN 2 THEN [INTERVAL_PARAM_3]
		WHEN 3 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN 1 
				WHEN 2 THEN [INTERVAL_PARAM_4]
				END)
		WHEN 4 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN 1 
				WHEN 2 THEN [INTERVAL_PARAM_4]
				END)
		ELSE 1

		END) 										AS [AppointmentDayOfTheWeek]	-- int									 				
	,[INTERVAL_PARAM_1] 							AS [AppointmentPeriod]					  	
	,(CASE [INTERVAL_PARAM_1]
		WHEN 0 THEN 'Single'
		WHEN 1 THEN 'Daily' 
		WHEN 2 THEN 'Weekly'
		WHEN 3 THEN 'Monthly'
		WHEN 4 THEN 'Yearly'
		END)										AS [AppointmentPeriodText]				  	
	,(CASE [INTERVAL_PARAM_1]
		WHEN 2 THEN 
			(CASE [INTERVAL_PARAM_3] 
				WHEN 0 THEN 'Sunday' 
				WHEN 1 THEN 'Monday' 
				WHEN 2 THEN 'Tuesday' 
				WHEN 3 THEN 'Wednesday' 
				WHEN 4 THEN 'Thursday' 
				WHEN 5 THEN 'Friday' 
				WHEN 6 THEN 'Saturday' 
			END)
		ELSE 1
		END)										AS [AppointmentDayOfTheWeekText]							  					
	,(CASE [INTERVAL_PARAM_1]
		WHEN 1 THEN [INTERVAL_PARAM_2] 
		WHEN 2 THEN [INTERVAL_PARAM_2]
		WHEN 3 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  [INTERVAL_PARAM_3]
				WHEN 2 THEN 1
				END)
		ELSE 1
		END) 										AS [AppointmentTimeInterval]				  						
--	,[APPOINTMENT_ID]													  	 														
	,(CASE [INTERVAL_PARAM_1]
		WHEN 3 THEN [INTERVAL_PARAM_2]
		WHEN 4 THEN [INTERVAL_PARAM_2]
		ELSE 0
		END) 										AS [AppointmentOption2Selected]			  														
	,(CASE (CASE [INTERVAL_PARAM_1]
		WHEN 3 THEN 			
			(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  1
				WHEN 2 THEN [INTERVAL_PARAM_3]
				END)
		WHEN 4 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  1
				WHEN 2 THEN [INTERVAL_PARAM_3]
				END)
		ELSE 0
		END)
		WHEN 1 THEN 'First'
		WHEN 2 THEN 'Second'
		WHEN 3 THEN 'Third'
		WHEN 4 THEN 'Fourth'
		WHEN 5 THEN 'Last'
		ELSE 'First'
		END)
													AS [AppointmentTimeOptionSelectionText]	  																
	,(CASE [INTERVAL_PARAM_1]
		WHEN 3 THEN 			
			(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  1
				WHEN 2 THEN [INTERVAL_PARAM_3]
				END)
		WHEN 4 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  1
				WHEN 2 THEN [INTERVAL_PARAM_3]
				END)
		ELSE 0
		END)										AS [AppointmentTimeOptionSelection]																	
	,(CASE (CASE [INTERVAL_PARAM_1]
		WHEN 3 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  1
				WHEN 2 THEN [INTERVAL_PARAM_5]
				END)
		WHEN 4 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  [INTERVAL_PARAM_3]
				WHEN 2 THEN [INTERVAL_PARAM_5]
				END)
		ELSE 1		
		END) 
		WHEN 1 THEN 'January'
		WHEN 2 THEN 'February'
		WHEN 3 THEN 'March'
		WHEN 4 THEN 'April'
		WHEN 5 THEN 'May'
		WHEN 6 THEN 'June'
		WHEN 7 THEN 'July'
		WHEN 8 THEN 'August'
		WHEN 9 THEN 'September'
		WHEN 10 THEN 'October'
		WHEN 11 THEN 'November'
		WHEN 12 THEN 'December'
		END)  
													AS [AppointmentMonthSelectionText]																											
	,(CASE [INTERVAL_PARAM_1]
		WHEN 3 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  1
				WHEN 2 THEN [INTERVAL_PARAM_5]
				END)
		WHEN 4 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  [INTERVAL_PARAM_3]
				WHEN 2 THEN [INTERVAL_PARAM_5]
				END)
		ELSE 1		
		END) 										AS [AppointmentMonthSelection]																																	
	,(CASE [INTERVAL_PARAM_1]
		WHEN 4 THEN 
				(CASE [INTERVAL_PARAM_2] 
				WHEN 1 THEN  [INTERVAL_PARAM_4]
				WHEN 2 THEN 1
				END)
		ELSE 1		
		END) 										AS [AppointmentDayOfTheMonth]			--int	
	,TestSetDefinitionIndex							AS [TestSetIndex]																								
	,GetDate()										AS [CreatedDate]--y
	,'Varec'										AS [CreatedBy]--y
	,A.[UpdatedDate]--y
	,A.[UpdatedBy]--y
INTO #TMP_APPOINTMENTS 
FROM #TMPSITES S, AviationDB6.dbo.APPOINTMENTS A  
LEFT JOIN [ConsolidatedDB].[dbo].tblTestSetDefinitions T ON UPPER(A.DESCRIPTION) = UPPER(T.TestSetName)  AND T.DeleteFlag = 0
WHERE A.DeleteFlag = 0;


INSERT INTO [ConsolidatedDB].dbo.tblAppointments
(
       [SiteIndex]
      ,[AssociatedType]
      ,[AssociatedTypeIndex]
      ,[AssetText]
      ,[AppointmentCategory]
      ,[AppointmentIsSingle]
      ,[ScheduleOnWeekends]
      ,[ScheduleOnHolidays]
      ,[StartDate]  -- y
      ,[Duration]   -- y
      ,[AppointmentPeriod]
      ,[AppointmentPeriodText]
      ,[Description]  --y
      ,[AppointmentTimeInterval]
      ,[AppointmentDayOfTheWeekText]
      ,[AppointmentDayOfTheWeek]
      ,[AppointmentReoccuranceInterval]
      ,[AppointmentOption2Selected]
      ,[AppointmentTimeOptionSelectionText]
      ,[AppointmentTimeOptionSelection]
      ,[AppointmentMonthSelectionText]
      ,[AppointmentMonthSelection]
      ,[AppointmentDayOfTheMonth]
      ,[TestSetIndex]
      ,[CreatedDate]--y
      ,[CreatedBy]--y
      ,[UpdatedDate]--y
      ,[UpdatedBy]--y
     
      )/**/
SELECT 	
       [SiteIndex]
      ,[AssociatedType]
      ,[AssociatedTypeIndex]
      ,[AssetText]
      ,[AppointmentCategory]
      ,[AppointmentIsSingle]
      ,[ScheduleOnWeekends]
      ,[ScheduleOnHolidays]
      ,[StartDate]  -- y
      ,[Duration]   -- y
      ,[AppointmentPeriod]
      ,[AppointmentPeriodText]
      ,[Description]  --y
      ,[AppointmentTimeInterval]
      ,[AppointmentDayOfTheWeekText]
      ,[AppointmentDayOfTheWeek]
      ,[AppointmentReoccuranceInterval]
      ,[AppointmentOption2Selected]
      ,[AppointmentTimeOptionSelectionText]
      ,[AppointmentTimeOptionSelection]
      ,[AppointmentMonthSelectionText]
      ,[AppointmentMonthSelection]
      ,[AppointmentDayOfTheMonth]
      ,[TestSetIndex]
      ,[CreatedDate]--y
      ,[CreatedBy]--y
      ,[UpdatedDate]--y
      ,[UpdatedBy]--y
FROM #TMP_APPOINTMENTS;

INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
TypeID,
SiteIndex,
[Index],
CreatedDate,
CreatedBy
)
SELECT
'Appointment',
A.SiteIndex,
[Index],
GETDATE(),
'Varec'
FROM [ConsolidatedDB].[dbo].tblAppointments A JOIN #TMPSITES S ON A.SiteIndex = S.SiteIndex8
WHERE  
[Index] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE SiteIndex=S.SiteIndex8 AND TypeID='Appointment');

DROP TABLE #TMP_APPOINTMENTS;
DROP TABLE #TMP_PERSON_INDEX_MAP;
DROP TABLE #TMP_EQUIPMENT_INDEX_MAP;
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







END;