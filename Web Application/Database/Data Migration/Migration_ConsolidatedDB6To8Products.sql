USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6To8Products]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Products') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8Products
GO


CREATE PROCEDURE [dbo].Migration_ConsolidatedDB6To8Products
 /*=============================================
 Author:			Ali Coker
 Create date:		3/8/2010
 Description:		Migrating ConsolidatedDB Products to tblProduct
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migration_ConsolidatedDB6To8Products 2,null

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
ORDER BY S6.SiteIndex


IF EXISTS(SELECT SiteIndex, ProductID FROM [ConsolidatedDB6].[dbo].tblProducts  WHERE DeleteFlag = 0 GROUP BY SiteIndex, ProductID HAVING Count(*) > 1)
BEGIN
	DECLARE @MSG nvarchar(256)
	SELECT TOP 1 @MSG = 'One or more duplicate product entries detected: SiteIndex=' + CAST(SiteIndex AS nvarchar) + ' ProductID=' + ProductID  
	FROM [ConsolidatedDB6].[dbo].tblProducts A
	WHERE DeleteFlag = 0 AND
		EXISTS(SELECT SiteIndex, ProductID FROM [ConsolidatedDB6].[dbo].tblProducts  
		WHERE DeleteFlag = 0 AND ProductID=A.ProductID AND SiteIndex=A.SiteIndex 
		GROUP BY SiteIndex, ProductID HAVING Count(*) > 1)
	RAISERROR ( @MSG, 0, 1)
	RETURN
END		






/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	
*/

DECLARE @date datetime
SET @date = GETDATE()


INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	TypeID, SiteIndex, [Index], CreatedDate, CreatedBy
)
SELECT 'User Data', S.SiteIndex8, -1, @date, 'Varec' FROM  #TMPSITES S WHERE 
NOT EXISTS (SELECT * FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE SiteIndex=S.SiteIndex8 AND TypeID='User Data' )



IF @IsBaseDB = 0
BEGIN	
	UPDATE	[ConsolidatedDB].[dbo].tblProducts SET SiteIndex=S.SiteIndex8
	FROM #TMPSITES S 
		WHERE ProductID IN (SELECT [ProductCode] FROM [ConsolidatedDB6].[dbo].tblProducts P WHERE DeleteFlag=0 AND SiteIndex=S.SiteIndex6 ) AND SiteIndex=-1
END

 
SELECT 
	[ProductIndex]						AS	[ProductIndex]
      ,S.SiteIndex8						AS	[SiteIndex]	
      ,[ProductCode]					AS	[ProductID]	
      ,[Description]					AS	[Description]	
      ,[GenericType]					AS	[GenericType]
      ,0								AS	[ProductType]	
      ,[StockResetDate]					AS	[StockResetDate]	
      ,[StockTrack]						AS	[StockTrack]		
      ,CASE WHEN ISNULL([LowDensity],0) <= 0 THEN 610 ELSE [LowDensity] END		AS	[DensityLowLimit]		
      ,CASE WHEN ISNULL([HighDensity],0) <= 0 THEN 1074.98249429658 ELSE [LowDensity] END	AS	[DensityHighLimit]	
		,8.2							AS	[DensityDeadband]		
		,0								AS	[ApplyDensityLimits]	
		,-17.7777777777778				AS	[TemperatureHiHiLimit]	
		,-17.7777777777778				AS	[TemperatureHighLimit]	
		,-17.7777777777778				AS	[TemperatureLowLimit]	
		,-17.7777777777778				AS	[TemperatureLoLoLimit]	
		,0								AS	[TemperatureDeadband]	
		,0								AS	[ApplyTemperatureLimits]
      ,[Bonded]							AS	[Bonded]								
      ,[LowStockWarning]				AS	[LowStockWarning]						
      ,[GroundFuel]						AS	[GroundFuel]								
      ,[ProductID]						AS	[ProductCode]									
      ,[Price]							AS	[Price]																
      ,[AviationFuelFlag]				AS	[AviationFuelFlag]											
      ,(CASE ProductCode WHEN 'JP4'
		THEN [ConversionMethod]
		ELSE 4 END)						AS	[MajorCorrectionMethod]
      ,[ConversionMethodMinor]			AS	[MinorCorrectionMethod]	
      , 0								AS	[CorrectionFactor0]		
      , 0								AS	[CorrectionFactor1]		
      , 0								AS	[CorrectionFactor2]		
      , 0								AS	[CorrectionFactor3]		
      , 0								AS	[CorrectionFactor4]		
      , 1074.98249429658				AS	[StandardDensity]		
      , -17.7777777777778				AS	[StandardTemperature]	
      , -17.7777777777778				AS	[AlternateTemperature]	
      , 0								AS	[AlternatePressure]		
      , 0								AS	[ApplyVolumeCorrection]	
      ,[UnitOfIssue]																	
      ,[VolumeUnitIndex]				AS	[VolumeUnitIndex]															
      ,[TemperatureUnitIndex]			AS	[TemperatureUnitIndex]											
      ,[DensityUnitIndex]				AS	[DensityUnitIndex]																
      ,[VolumeDecimalPlaces]			AS	[VolumeDecimalPlaces]												
      ,[TemperatureDecimalPlaces]		AS	[TemperatureDecimalPlaces]									
      ,[DensityDecimalPlaces]			AS	[DensityDecimalPlaces]											
      ,@date						AS	[CreatedDate]									
      ,'Varec'							AS	[CreatedBy]											
      ,[UpdatedDate]					AS	[UpdatedDate]										
      ,[UpdatedBy]						AS	[UpdatedBy]										
      ,[DeleteFlag]											
      ,[Capitalize]						AS	[Capitalize]										
		,0								AS	[OctaneNumber]							
		,0								AS	[ReidVaporPressure]						
		,0								AS	[HazardousMaterial]						
		,0								AS	[RegulatoryClass]						
		,''								AS	[LoadRackDisplayText]					
		,0								AS	[ComponentTolerance]					
		,0								AS	[VaporRecovery]							
		,0								AS	[LockedOut]								
		,''								AS	[LockedOutReason]						
		,@date						AS	[LockedOutDate]							
		,0								AS	[VarianceTolerance]						
		,0								AS	[LoadByWeight]							
		,''								AS	[PIDXCode]								
		,''								AS	[ContaminationPromptLoadRackText]		
		,0								AS	[InhibitAccounting]						
		,null							AS	[TrackingProductIndex]					
      ,CASE P.Capitalize 
       WHEN 0 THEN 'No'
       WHEN 1 THEN 'Yes'
       END								AS	[UserData1]							
      ,CASE P.AviationFuelFlag
       WHEN 0 THEN 'No'
       WHEN 1 THEN 'Yes'
       END								AS	[UserData2]							
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
      ,[InflightConversionFactor]		
	,0									AS	[MassUnitIndex]
	,0									AS	[LevelUnitIndex]
	,0									AS	[FlowUnitIndex]
	,0									AS	[PressureUnitIndex]
	,0									AS	[MassDecimalPlaces]
	,0									AS	[LevelDecimalPlaces]
	,0									AS	[FlowDecimalPlaces]
	,0									AS [PressureDecimalPlaces]
	,0									AS [VolumePackageSize]
	,0									AS [MassPackageSize]
INTO #TMP_PRODUCT
FROM [ConsolidatedDB6].[dbo].tblProducts P JOIN #TMPSITES S ON P.SiteIndex = S.SiteIndex6
WHERE P.DeleteFlag = 0  AND LEN(LTRIM(RTRIM(P.ProductCode))) > 0
 

INSERT INTO [ConsolidatedDB].[dbo].tblProducts
(
 [SiteIndex]
,[ProductID]
,[Description]
,[GenericType]
,[ProductType]
,[StockResetDate]
,[StockTrack]
,[DensityHighLimit]
,[DensityLowLimit]
,[DensityDeadband]
,[ApplyDensityLimits]
,[TemperatureHiHiLimit]
,[TemperatureHighLimit]
,[TemperatureLowLimit]
,[TemperatureLoLoLimit]
,[TemperatureDeadband]
,[ApplyTemperatureLimits]
,[Bonded]
,[LowStockWarning]
,[GroundFuel]
,[ProductCode]
,[Price]
,[AviationFuelFlag]
,[MajorCorrectionMethod]
,[MinorCorrectionMethod]
,[CorrectionFactor0]
,[CorrectionFactor1]
,[CorrectionFactor2]
,[CorrectionFactor3]
,[CorrectionFactor4]
,[StandardDensity]
,[StandardTemperature]
,[AlternateTemperature]
,[AlternatePressure]
,[ApplyVolumeCorrection]
,[VolumeUnitIndex]
,[TemperatureUnitIndex]
,[DensityUnitIndex]
,[VolumeDecimalPlaces]
,[TemperatureDecimalPlaces]
,[DensityDecimalPlaces]
,[Capitalize]
,[OctaneNumber]
,[ReidVaporPressure]
,[HazardousMaterial]
,[RegulatoryClass]
,[LoadRackDisplayText]
,[ComponentTolerance]
,[VaporRecovery]
,[LockedOut]
,[LockedOutReason]
,[LockedOutDate]
,[VarianceTolerance]
,[LoadByWeight]
,[PIDXCode]
,[ContaminationPromptLoadRackText]
,[InhibitAccounting]
,[TrackingProductIndex]
,[UserData1]
,[UserData2]
,[UserData3]
,[UserData4]
,[UserData5]
,[UserData6]
,[UserData7]
,[UserData8]
,[CreatedDate]
,[CreatedBy]
,[UpdatedDate]
,[UpdatedBy]
,[MassUnitIndex]
,[LevelUnitIndex]
,[FlowUnitIndex]
,[PressureUnitIndex]
,[MassDecimalPlaces]
,[LevelDecimalPlaces]
,[FlowDecimalPlaces]
,[PressureDecimalPlaces]
,[VolumePackageSize]
,[MassPackageSize]
)
SELECT
[SiteIndex]
,[ProductID]
,[Description]
,[GenericType]
,[ProductType]
,[StockResetDate]
,[StockTrack]
,[DensityHighLimit]
,[DensityLowLimit]
,[DensityDeadband]
,[ApplyDensityLimits]
,[TemperatureHiHiLimit]
,[TemperatureHighLimit]
,[TemperatureLowLimit]
,[TemperatureLoLoLimit]
,[TemperatureDeadband]
,[ApplyTemperatureLimits]
,[Bonded]
,[LowStockWarning]
,[GroundFuel]
,[ProductCode]
,[Price]
,[AviationFuelFlag]
,[MajorCorrectionMethod]
,[MinorCorrectionMethod]
,[CorrectionFactor0]
,[CorrectionFactor1]
,[CorrectionFactor2]
,[CorrectionFactor3]
,[CorrectionFactor4]
,[StandardDensity]
,[StandardTemperature]
,[AlternateTemperature]
,[AlternatePressure]
,[ApplyVolumeCorrection]
,[VolumeUnitIndex]
,[TemperatureUnitIndex]
,[DensityUnitIndex]
,[VolumeDecimalPlaces]
,[TemperatureDecimalPlaces]
,[DensityDecimalPlaces]
,[Capitalize]
,[OctaneNumber]
,[ReidVaporPressure]
,[HazardousMaterial]
,[RegulatoryClass]
,[LoadRackDisplayText]
,[ComponentTolerance]
,[VaporRecovery]
,[LockedOut]
,[LockedOutReason]
,[LockedOutDate]
,[VarianceTolerance]
,[LoadByWeight]
,[PIDXCode]
,[ContaminationPromptLoadRackText]
,[InhibitAccounting]
,[TrackingProductIndex]
,[UserData1]
,[UserData2]
,[UserData3]
,[UserData4]
,[UserData5]
,[UserData6]
,[UserData7]
,[UserData8]
,[CreatedDate]
,[CreatedBy]
,[UpdatedDate]
,[UpdatedBy]
,[MassUnitIndex]
,[LevelUnitIndex]
,[FlowUnitIndex]
,[PressureUnitIndex]
,[MassDecimalPlaces]
,[LevelDecimalPlaces]
,[FlowDecimalPlaces]
,[PressureDecimalPlaces]
,[VolumePackageSize]
,[MassPackageSize]
FROM #TMP_PRODUCT t
where ProductID NOT IN (Select ProductID from [ConsolidatedDB].[dbo].[tblProducts] P WHERE P.SiteIndex IN (-1, t.SiteIndex))

INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
TypeID,
SiteIndex,
[Index],
CreatedDate,
CreatedBy
)
SELECT
'Products',
t.SiteIndex,
P.[ProductIndex],
T.CreatedDate,
T.CreatedBy
FROM [ConsolidatedDB].[dbo].tblProducts P JOIN #TMP_PRODUCT T ON P.ProductID=T.ProductID
WHERE P.SiteIndex IN (-1, t.SiteIndex) 
AND P.[ProductIndex] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Products' AND SiteIndex = t.SiteIndex)


/*Assign Dummy Product (index=-1) to migrated site*/
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
TypeID,
SiteIndex,
[Index],
CreatedDate,
CreatedBy
)
SELECT
'Products',
S.SiteIndex8,
P.[ProductIndex],
P.CreatedDate,
'Varec'
FROM [ConsolidatedDB].[dbo].tblProducts P , #TMPSITES S
WHERE P.SiteIndex IN (-1, S.SiteIndex8) AND P.[ProductIndex] = -1 
AND P.[ProductIndex] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Products' AND SiteIndex = S.SiteIndex8)



IF @IsBaseDB = 0
BEGIN
	INSERT INTO [ConsolidatedDB].[dbo].tblProductMap (
	AssignedToIndex
	,AssignedIndex
	,[Type]	
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
	)
	SELECT L.[Index], P.ProductIndex, 14, 'Varec', @date, 'Varec', @date FROM  [ConsolidatedDB].[dbo].tblListViews L,  
	[ConsolidatedDB].[dbo].tblProducts P JOIN #TMP_PRODUCT T ON P.ProductID=T.ProductID AND P.SiteIndex = T.SiteIndex  
	WHERE L.SiteIndex=-1 AND L.[ID]='DOD Standard'
	AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].tblProductMap M 
		WHERE M.AssignedToIndex = L.[Index] AND M.AssignedIndex = P.ProductIndex AND [Type]=14)
END
ELSE
BEGIN
	INSERT INTO [ConsolidatedDB].[dbo].tblProductMap (
	AssignedToIndex
	,AssignedIndex
	,[Type]	
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
	)
	SELECT L.[Index], P.ProductIndex, 14, 'Varec', @date, 'Varec', @date FROM  [ConsolidatedDB].[dbo].tblListViews L,  
	[ConsolidatedDB].[dbo].tblProducts P JOIN #TMP_PRODUCT T ON P.ProductID=T.ProductID AND P.SiteIndex IN ( -1, T.SiteIndex) 
	WHERE L.SiteIndex=-1 AND L.[ID]='DOD Standard'
	AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].tblProductMap M 
		WHERE M.AssignedToIndex = L.[Index] AND M.AssignedIndex = P.ProductIndex AND [Type]=14)
END


DROP TABLE #TMP_PRODUCT


--Create products that are referenced in transactions but are not defined.

CREATE TABLE #TMP_UNDEFINED_PRODUCTS (
	SiteIndex int, 
	ProductID nvarchar(30),
	ProductCode nvarchar(15),
	CreatedBy nvarchar(100),
	CreatedDate DateTime,
	UpdatedBy nvarchar(100),
	UpdatedDate DateTime)
	

INSERT INTO #TMP_UNDEFINED_PRODUCTS 
	(SiteIndex, ProductID, ProductCode,	CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	SELECT -1,'AUTO_GENERATED_'+T.ProductID,T.ProductID, 'Varec',@date,'Varec',@date 
	FROM 
		(
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx1 
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx2
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx3
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx4
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx5
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx6
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx7
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx8
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx9
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx10
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx11
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx12
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx13
		UNION  
		SELECT DISTINCT SiteIndex, ProductID FROM AccountingDB6.dbo.t_Acct_Tx14
		UNION  
		SELECT DISTINCT SiteIndex, ToProductID FROM AccountingDB6.dbo.t_Acct_Tx15
		UNION  
		SELECT DISTINCT SiteIndex, FromProductID FROM AccountingDB6.dbo.t_Acct_Tx15
		UNION  
		SELECT DISTINCT SiteIndex, ToProductID FROM AccountingDB6.dbo.t_Acct_Tx16
		UNION  
		SELECT DISTINCT SiteIndex, FromProductID FROM AccountingDB6.dbo.t_Acct_Tx16
		) T
		JOIN ConsolidatedDB6.dbo.tblSites S6 ON T.SiteIndex = S6.SiteIndex 
		JOIN ConsolidatedDB.dbo.tblSites S8 ON S8.ID = S6.SiteID
		WHERE
		NOT EXISTS(SELECT * FROM ConsolidatedDB.dbo.tblProducts P8 JOIN
		ConsolidatedDB6.dbo.tblProducts P6 ON P8.ProductID = P6.ProductCode
		WHERE T.ProductID = P6.ProductID AND p8.SiteIndex IN (-1, S8.SiteIndex) )


INSERT INTO ConsolidatedDB.dbo.tblProducts 
(
	SiteIndex, 
	ProductID, 
	ProductCode, 
	CreatedBy, 
	CreatedDate, 
	UpdatedBy, 
	UpdatedDate
      ,[Description]	
      ,[GenericType]
      ,[ProductType]	
      ,[StockResetDate]	
      ,[StockTrack]		
      ,[DensityLowLimit]		
      ,[DensityHighLimit]	
		,[DensityDeadband]		
		,[ApplyDensityLimits]	
		,[TemperatureHiHiLimit]	
		,[TemperatureHighLimit]	
		,[TemperatureLowLimit]	
		,[TemperatureLoLoLimit]	
		,[TemperatureDeadband]	
		,[ApplyTemperatureLimits]
      ,[Bonded]								
      ,[LowStockWarning]						
      ,[GroundFuel]								
      ,[Price]																
      ,[AviationFuelFlag]											
      ,[MajorCorrectionMethod]
      ,[MinorCorrectionMethod]	
      ,[CorrectionFactor0]		
      ,[CorrectionFactor1]		
      ,[CorrectionFactor2]		
      ,[CorrectionFactor3]		
      ,[CorrectionFactor4]		
      ,[StandardDensity]		
      ,[StandardTemperature]	
      ,[AlternateTemperature]	
      ,[AlternatePressure]		
      ,[ApplyVolumeCorrection]	
      ,[VolumeUnitIndex]															
      ,[TemperatureUnitIndex]											
      ,[DensityUnitIndex]																
      ,[VolumeDecimalPlaces]												
      ,[TemperatureDecimalPlaces]									
      ,[DensityDecimalPlaces]											
      ,[Capitalize]										
		,[OctaneNumber]							
		,[ReidVaporPressure]						
		,[HazardousMaterial]						
		,[RegulatoryClass]						
		,[LoadRackDisplayText]					
		,[ComponentTolerance]					
		,[VaporRecovery]							
		,[LockedOut]								
		,[LockedOutReason]						
		,[LockedOutDate]							
		,[VarianceTolerance]						
		,[LoadByWeight]							
		,[PIDXCode]								
		,[ContaminationPromptLoadRackText]		
		,[InhibitAccounting]						
		,[TrackingProductIndex]					
      ,[UserData1]							
      ,[UserData2]							
      ,[UserData3]							
      ,[UserData4]								
      ,[UserData5]						
      ,[UserData6]									
      ,[UserData7]									
      ,[UserData8]						
	,[MassUnitIndex]
	,[LevelUnitIndex]
	,[FlowUnitIndex]
	,[PressureUnitIndex]
	,[MassDecimalPlaces]
	,[LevelDecimalPlaces]
	,[FlowDecimalPlaces]
	,[PressureDecimalPlaces]

	,[VolumePackageSize]
	,[MassPackageSize]
)
SELECT 
	DISTINCT 
	-1, 
	ProductID, 
	LEFT(ProductCode,15), 
	CreatedBy, 
	CreatedDate, 
	UpdatedBy, 
	UpdatedDate 
      ,'Created by migration script.'					AS	[Description]	
      ,''								AS	[GenericType]
      ,0								AS	[ProductType]	
      ,@date					AS	[StockResetDate]	
      ,0						AS	[StockTrack]		
      ,610 AS	[DensityLowLimit]		
      ,1074.98249429658 AS	[DensityHighLimit]	
		,8.2							AS	[DensityDeadband]		
		,0								AS	[ApplyDensityLimits]	
		,-17.7777777777778				AS	[TemperatureHiHiLimit]	
		,-17.7777777777778				AS	[TemperatureHighLimit]	
		,-17.7777777777778				AS	[TemperatureLowLimit]	
		,-17.7777777777778				AS	[TemperatureLoLoLimit]	
		,0								AS	[TemperatureDeadband]	
		,0								AS	[ApplyTemperatureLimits]
      ,0							AS	[Bonded]								
      ,0				AS	[LowStockWarning]						
      ,0						AS	[GroundFuel]								
      ,0							AS	[Price]																
      ,0				AS	[AviationFuelFlag]											
      ,4						AS	[MajorCorrectionMethod]
      ,1			AS	[MinorCorrectionMethod]	
      , 0								AS	[CorrectionFactor0]		
      , 0								AS	[CorrectionFactor1]		
      , 0								AS	[CorrectionFactor2]		
      , 0								AS	[CorrectionFactor3]		
      , 0								AS	[CorrectionFactor4]		
      , 1074.98249429658				AS	[StandardDensity]		
      , -17.7777777777778				AS	[StandardTemperature]	
      , -17.7777777777778				AS	[AlternateTemperature]	
      , 0								AS	[AlternatePressure]		
      , 0								AS	[ApplyVolumeCorrection]	
      ,0				AS	[VolumeUnitIndex]															
      ,0			AS	[TemperatureUnitIndex]											
      ,0				AS	[DensityUnitIndex]																
      ,0			AS	[VolumeDecimalPlaces]												
      ,0		AS	[TemperatureDecimalPlaces]									
      ,0			AS	[DensityDecimalPlaces]											
      ,0						AS	[Capitalize]										
		,0								AS	[OctaneNumber]							
		,0								AS	[ReidVaporPressure]						
		,0								AS	[HazardousMaterial]						
		,0								AS	[RegulatoryClass]						
		,''								AS	[LoadRackDisplayText]					
		,0								AS	[ComponentTolerance]					
		,0								AS	[VaporRecovery]							
		,0								AS	[LockedOut]								
		,''								AS	[LockedOutReason]						
		,@date						AS	[LockedOutDate]							
		,0								AS	[VarianceTolerance]						
		,0								AS	[LoadByWeight]							
		,''								AS	[PIDXCode]								
		,''								AS	[ContaminationPromptLoadRackText]		
		,0								AS	[InhibitAccounting]						
		,null							AS	[TrackingProductIndex]					
      ,'No' AS	[UserData1]							
      ,'No' AS	[UserData2]							
      ,null	 as [UserData3]						
      ,null	 as [UserData4]								
      ,null	 as [UserData5]						
      ,null	 as [UserData6]									
      ,null	 as [UserData7]									
      ,null	 as [UserData8]						
	,0									AS	[MassUnitIndex]
	,0									AS	[LevelUnitIndex]
	,0									AS	[FlowUnitIndex]
	,0									AS	[PressureUnitIndex]
	,0									AS	[MassDecimalPlaces]
	,0									AS	[LevelDecimalPlaces]
	,0									AS	[FlowDecimalPlaces]
	,0									AS [PressureDecimalPlaces]
	,0									AS [VolumePackageSize]
	,0									AS [MassPackageSize]
	FROM #TMP_UNDEFINED_PRODUCTS

INSERT INTO ConsolidatedDB.dbo.tblEntityToSiteMap (TypeID, SiteIndex, [Index], CreatedBy, CreatedDate)
SELECT DISTINCT 'Products', T.SiteIndex, ProductIndex, T.CreatedBy, T.CreatedDate 
FROM #TMP_UNDEFINED_PRODUCTS T JOIN ConsolidatedDB.dbo.tblProducts P ON T.ProductID=P.ProductID AND P.SiteIndex=-1


INSERT INTO [ConsolidatedDB].[dbo].tblProductMap (
	AssignedToIndex
	,AssignedIndex
	,[Type]	
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
)
SELECT distinct L.[Index], P.ProductIndex, 14, 'Varec', @date, 'Varec', @date FROM  [ConsolidatedDB].[dbo].tblListViews L,  
	[ConsolidatedDB].[dbo].tblProducts P JOIN #TMP_UNDEFINED_PRODUCTS T ON P.ProductID=T.ProductID AND P.SiteIndex = -1 
	WHERE L.SiteIndex=-1 AND L.[ID]='DOD Standard'
	AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].[dbo].tblProductMap M 
		WHERE M.AssignedToIndex = L.[Index] AND M.AssignedIndex = P.ProductIndex AND [Type]=14)


DROP TABLE #TMP_UNDEFINED_PRODUCTS	;
DROP TABLE #TMPSITES	;
--select * from [ConsolidatedDB].[dbo].tblProducts p right join [ConsolidatedDB6].[dbo].tblProducts q on p.productid=q.productcode
--where p.productindex is null;





  
		
/*	-- Left it commented out rather than delete so that it can be used when debugging.


IF @@TRANCOUNT > 0    
BEGIN     
	--ROLLBACK TRANSACTION;     
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