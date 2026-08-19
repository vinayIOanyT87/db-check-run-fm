USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6To8MaintenanceLog]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8MaintenanceLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6To8MaintenanceLog
GO

CREATE PROCEDURE [dbo].Migration_AviationDB6To8MaintenanceLog
 /*=============================================
 Author:			URVI PATEL
 Create date:		2/2/2010
Description:		Migrating AviationDB6.0 MaintenanceLog to ConsolidatedDB8.0 MaintenanceLog
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_AviationDB6To8MaintenanceLog 2, null

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
DECLARE @currentDate datetime
SET @currentDate = GETDATE()

IF (NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].tblMaintenanceReasons 
	WHERE ISNULL(DeletedFlag,0) = 0 AND MaintenanceReasonIndex = 1))
BEGIN
SET IDENTITY_INSERT [ConsolidatedDB].[dbo].tblMaintenanceReasons ON
INSERT INTO [ConsolidatedDB].[dbo].tblMaintenanceReasons
(
	[MaintenanceReasonIndex] 
	,[ID]
	,[Description]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	,DeletedFlag
	)
VALUES( 1, 'IN SERVICE', 'In Service', @currentDate, 'Varec', @currentDate, 'Varec',0 )

SET IDENTITY_INSERT [ConsolidatedDB].[dbo].tblMaintenanceReasons OFF
END


INSERT INTO [ConsolidatedDB].[dbo].tblMaintenanceReasons
(
	[ID]
	,[Description]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	)
SELECT 
		CASE WHEN ISNULL(LTRIM(RTRIM([STATUS_CODE])),'') = ''
		THEN 'Unknown' 
		ELSE REPLACE(REPLACE(ISNULL(LTRIM(RTRIM([STATUS_CODE])),''),'<','-'),'>','-')
		END				AS [ID]
      ,CASE WHEN ISNULL(LTRIM(RTRIM([STATUS_DESCRIPTION])),'') = ''
		THEN 'Unknown' 
		ELSE REPLACE(REPLACE(LEFT(ISNULL(LTRIM(RTRIM([STATUS_DESCRIPTION])),''),50),'<','-'),'>','-')
		END						AS [Description]
      ,@currentDate				AS [CreatedDate]
      ,'Varec'					AS [CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
  FROM [AviationDB6].[dbo].[MAINT_STATUS_CODES] WHERE DeleteFlag = 0
  AND CASE WHEN ISNULL(LTRIM(RTRIM([STATUS_CODE])),'') = ''
		THEN 'Unknown' 
		ELSE REPLACE(REPLACE(ISNULL(LTRIM(RTRIM([STATUS_CODE])),''),'<','-'),'>','-')
		END NOT IN (SELECT [ID] FROM [ConsolidatedDB].[dbo].tblMaintenanceReasons WHERE ISNULL(DeletedFlag,0) = 0) 

IF (NOT EXISTS(SELECT UPPER([ID]) FROM [ConsolidatedDB].[dbo].tblMaintenanceReasons WHERE ISNULL(DeletedFlag,0) = 0 AND UPPER(ID)='NOT IN SERVICE' ))
BEGIN
INSERT INTO [ConsolidatedDB].[dbo].tblMaintenanceReasons
(
	[ID]
	,[Description]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	)
VALUES( 'NOT IN SERVICE', 'Maintenance reason not provided.', @currentDate, 'Varec', @currentDate, 'Varec' )
END

INSERT INTO [ConsolidatedDB].[dbo].tblMaintenanceReasons
(
	[ID]
	,[Description]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	)/**/
SELECT DISTINCT 
		CASE WHEN ISNULL(LTRIM(RTRIM(M.STATUS)),'') = ''
				THEN 'Unknown' 
				ELSE REPLACE(REPLACE(LEFT(ISNULL(LTRIM(RTRIM(M.STATUS)),''),50),'<','-'),'>','-')
				END	AS [ID]
		,CASE WHEN ISNULL(LTRIM(RTRIM(M.STATUS)),'') = ''
				THEN 'Unknown' 
				ELSE REPLACE(REPLACE(LEFT(ISNULL(LTRIM(RTRIM(M.STATUS)),''),50),'<','-'),'>','-')
				END AS [Description]
		, @currentDate, 'Varec', @currentDate, 'Varec' FROM [AviationDB6].[dbo].[MAINT_LOG] M 
WHERE M.[DeleteFlag] = 0 AND M.STATUS IS NOT NULL
AND M.[STATUS] NOT IN (SELECT STATUS_DESCRIPTION  FROM [AviationDB6].[dbo].[MAINT_STATUS_CODES] WHERE DeleteFlag = 0)
/**/




SELECT
	EM.SiteIndex,
	[STATUS_DATE]						AS [ChangeDate],
	--[REFERENCE_CODE]				,
	E.[Index]							AS [EquipmentIndex],
	[REGISTRATION_ID]					AS [EquipmentID], 
	CASE [IN_SERVICE] 
		WHEN 1 THEN 1
		ELSE C.MaintenanceReasonIndex
		END								AS [MaintenanceReasonIndex],
	CASE [IN_SERVICE] 
		WHEN 1 THEN 'In Service'
		ELSE C.[Description]
		END								AS [MaintenanceReason],
	ISNULL([EQ_TYPE],'Unknown')			AS [EquipmentType],
	--[PRODUCT_CODE]					,
	[IN_SERVICE]						AS [InServiceFlag],
	--[FUELING_STATE]					,
	ISNULL([RTS_DATE],@currentDate)		AS [EstReturnToServiceDate],
	ISNULL([WORK_ORDER],'')				AS [WorkOrder],
	SUBSTRING(replace(replace(replace(isnull([MEMO],''),';',''),':',''),'''',''),1,1000)	AS [Memo],
	@currentDate							AS [CreatedDate],
	'Varec'								AS [CreatedBy]						,
	M.[UpdatedDate]					,
	ISNULL(M.[UpdatedBy],ISNULL(M.[CreatedBy],'Unknown')) AS [UpdatedBy]						
	INTO #TMP_MAINT_LOG
FROM [AviationDB6].[dbo].[MAINT_LOG] M JOIN [ConsolidatedDB].[dbo].tblEquipment E ON M.[REGISTRATION_ID] = E.[ID]
JOIN [ConsolidatedDB].[dbo].tblEntityToSiteMap EM ON E.[Index]=EM.[Index] AND EM.TypeID='Equipment'
JOIN #TMPSITES S 
ON S.SiteIndex8 = EM.SiteIndex
LEFT JOIN [ConsolidatedDB].[dbo].tblMaintenanceReasons C 
ON CASE WHEN ISNULL(LTRIM(RTRIM(M.STATUS)),'') = ''
				THEN 'Unknown' 
				ELSE REPLACE(REPLACE(LEFT(ISNULL(LTRIM(RTRIM(M.STATUS)),''),50),'<','-'),'>','-')
				END=C.[Description]
WHERE M.[DeleteFlag] = 0 AND ISNULL(C.[DeletedFlag],0) = 0 

UPDATE 	#TMP_MAINT_LOG SET  
	#TMP_MAINT_LOG.[MaintenanceReasonIndex] = M.[MaintenanceReasonIndex],
	#TMP_MAINT_LOG.[MaintenanceReason]		 = M.[Description]
	FROM [ConsolidatedDB].[dbo].tblMaintenanceReasons M
	WHERE #TMP_MAINT_LOG.[MaintenanceReasonIndex] IS NULL AND UPPER(M.ID) =
	(CASE #TMP_MAINT_LOG.InServiceFlag WHEN 1 THEN 'IN SERVICE' ELSE 'NOT IN SERVICE' END)
					

INSERT INTO [ConsolidatedDB].[dbo].tblEquipmentMaintenanceLog
(
	[SiteIndex],
	[ChangeDate],
	[EquipmentIndex],
	[EquipmentID], 
	[MaintenanceReasonIndex],
	[MaintenanceReason],
	[EquipmentType],
	[InServiceFlag],
	[EstReturnToServiceDate],
	[WorkOrder],
	[Memo],
	[CreatedDate],
	[CreatedBy]	,				
	[UpdatedDate]					,
	[UpdatedBy]	
	)					
SELECT
	[SiteIndex],
	[ChangeDate],
	[EquipmentIndex],
	[EquipmentID], 
	[MaintenanceReasonIndex],
	[MaintenanceReason],
	[EquipmentType],
	[InServiceFlag],
	[EstReturnToServiceDate],
	[WorkOrder],
	[Memo],
	[CreatedDate],
	[CreatedBy]	,				
	[UpdatedDate],					
	[UpdatedBy]	
FROM #TMP_MAINT_LOG

DROP TABLE #TMP_MAINT_LOG







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