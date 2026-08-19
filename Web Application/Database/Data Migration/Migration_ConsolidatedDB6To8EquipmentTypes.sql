USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6To8EquipmentTypes]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8EquipmentTypes') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8EquipmentTypes
GO


CREATE PROCEDURE [dbo].Migration_ConsolidatedDB6To8EquipmentTypes
 /*=============================================
 Author:			Ali Coker
 Create date:		3/8/2010
 Description:		Migrating ConsolidatedDB equipment types to tblEquipmentTypes
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migration_ConsolidatedDB6To8EquipmentTypes 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarchar(MAX) = null

AS 
BEGIN

IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex



/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	

*/

SELECT SIteIndex8, [EqTypeIndex],[AttributeIndex],[AttributeName],[Description]
INTO #TMP_ATTRIBUTES FROM [ConsolidatedDB6].[dbo].tblEqTypeAttributeMap A 
	JOIN [ConsolidatedDB6].[dbo].tblEqTypeAttributes B ON A.AttributeCode=B.AttributeIndex 
	JOIN #TMPSITES S ON A.SiteIndex=S.SiteIndex6  WHERE A.DeleteFlag = 0 AND B.DeleteFlag = 0  


IF @IsBaseDB = 0
BEGIN
	UPDATE	[ConsolidatedDB].[dbo].tblEquipmentTypes SET SiteIndex=S.SiteIndex8 
	FROM #TMPSITES S WHERE EqTypeName IN 
	(SELECT EqTypeName FROM [ConsolidatedDB6].[dbo].tblEquipmentTypes E WHERE DeleteFlag=0 AND SiteIndex=S.SiteIndex6 ) AND SiteIndex=-1
END	
			
/*

Bit settings for Attributes column in table tblEquipmentTypes:
	bit 2 - System
	bit 1 - Vehicle
	bit 0 - Hydrant

*/
SELECT
	[EqTypeIndex]				AS [EqTypeIndex]		,
	S.SiteIndex8					AS [SiteIndex]			,
	[EqTypeName]				AS [EqTypeName]		,
	[EqTypeDescription]			AS [EqTypeDescription]	,
	dbo.ConvertToSIUnits([Capacity],46)					AS [Capacity]			,--Convert from USGallons
	dbo.ConvertToSIUnits([SafeFill],46)					AS [SafeFill]			,
	[Make]						AS [Make]				,
	[Model]						AS [Model]				,
	[Year]						AS [Year]				,
	CASE [Attributes] 
		WHEN 1						--Hydrant in 6.0 maps to HYDRANT_CART_TYPE in 8.0
			THEN 8					
		WHEN 2						--Vehicle maps to SYSTEM_TYPE in 8.0
			THEN 12					
		WHEN 3						--Vehicle and Hydrant in 6.0 map to TRAILER_TYPE in 8.0
			THEN 0				
		WHEN 4						--System in 6.0 maps to SYSTEM_TYPE in 8.0
			THEN 12
		WHEN 5						--System and Hydrant in 6.0 map to SYSTEM_TYPE in 8.0
			THEN 12
		WHEN 6						--System and Vehicle
			THEN 2
		WHEN 7						--System, Vehicle, and Hydrant. Need to check if Aircraft attribute is assigned.
			THEN 
				(CASE 
					WHEN 'Aircraft' IN (SELECT AttributeName FROM #TMP_ATTRIBUTES WHERE EqTypeIndex = E.EqTypeIndex) THEN 2
					ELSE 13
					END)
		ELSE 0
	END							AS [Attribute]			,
	GETDATE()					AS [CreatedDate]		,
	'Varec'						AS [CreatedBy]			,
	[UpdatedDate]				AS [UpdatedDate]		,
	[UpdatedBy]				AS [UpdatedBy]			,
	[DeleteFlag]				AS [DeleteFlag]		,
	[IssPt]					AS [IssPt],	
	0							AS [MultiCompartment]
INTO #TMP_EQUIPMENT_TYPES		   	
FROM [ConsolidatedDB6].[dbo].tblEquipmentTypes E JOIN #TMPSITES S ON SiteIndex=S.SiteIndex6 
WHERE DeleteFlag = 0   


UPDATE	[ConsolidatedDB].[dbo].tblEquipmentTypes SET  
	[EqTypeDescription]			= T.[EqTypeDescription]	,
	[Capacity] = T.[Capacity],
	[SafeFill] = T.[SafeFill],
	[Make]						= T.[Make],
	[Model]						= T.[Model],
	[Year]						= T.[Year],
	[IssPt]						= T.[IssPt]
FROM #TMP_EQUIPMENT_TYPES T
WHERE [ConsolidatedDB].[dbo].tblEquipmentTypes.SiteIndex=T.SiteIndex 
AND T.EqTypeName = [ConsolidatedDB].[dbo].tblEquipmentTypes.EqTypeName
AND [ConsolidatedDB].[dbo].tblEquipmentTypes.EqTypeName IN (SELECT EqTypeName FROM [ConsolidatedDB].[dbo].tblEquipmentTypes E WHERE SiteIndex=T.SiteIndex ) 



INSERT INTO [ConsolidatedDB].[dbo].tblEquipmentTypes (
	[SiteIndex] ,
	[EqTypeName],
	[EqTypeDescription] ,
	[Capacity] ,
	[SafeFill],
	[Make] ,
	[Model] ,
	[Year],
	[Attribute] ,
	[CreatedDate] ,
	[CreatedBy] ,
	[UpdatedDate],
	[UpdatedBy],
	[DeleteFlag] ,
	[IssPt],
	[MultiCompartment])
SELECT
	[SiteIndex] ,
	[EqTypeName],
	[EqTypeDescription] ,
	[Capacity] ,
	[SafeFill],
	[Make] ,
	[Model] ,
	[Year],
	[Attribute] ,
	[CreatedDate] ,
	[CreatedBy] ,
	[UpdatedDate],
	[UpdatedBy],
	[DeleteFlag] ,
	[IssPt],
	[MultiCompartment]
FROM #TMP_EQUIPMENT_TYPES 	T
WHERE [EqTypeName] NOT IN (SELECT EqTypeName FROM [ConsolidatedDB].[dbo].tblEquipmentTypes E WHERE E.SiteIndex IN (-1, T.SiteIndex ))


	
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	TypeID,
	SiteIndex,
	[Index],
	CreatedDate,
	CreatedBy
)
SELECT
	'Equipment Type',
	T.SiteIndex,
	E.[EqTypeIndex],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblEquipmentTypes E JOIN #TMP_EQUIPMENT_TYPES T ON E.EqTypeName = T.EqTypeName 
WHERE E.SiteIndex IN (-1, T.SiteIndex) 
AND E.[EqTypeIndex] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Equipment Type' AND SiteIndex = T.SiteIndex)

--Eric Simmons (4-30-2010)
--Added to resolve bug 13933
--This will ensure that 'Aircraft' and 'Vehicle' are always associtated with the current site in context.
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	TypeID,
	SiteIndex,
	[Index],
	CreatedDate,
	CreatedBy
)
SELECT
	'Equipment Type',
	S.SiteIndex8,
	E.[EqTypeIndex],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblEquipmentTypes E , #TMPSITES S 
WHERE E.SiteIndex = -1 and E.EqTypeName in ('Aircraft','Vehicle')
AND E.[EqTypeIndex] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Equipment Type' AND SiteIndex = S.SiteIndex8)

--SELECT *
--  FROM [ConsolidatedDB].[dbo].[tblEquipmentTypes] e8
--  right join consolidateddb6.dbo.tblequipmenttypes e6 on e8.eqtypename=e6.eqtypename
--  where e8.eqtypename is null or e6.eqtypename is null

DROP TABLE #TMP_ATTRIBUTES
DROP TABLE #TMP_EQUIPMENT_TYPES
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