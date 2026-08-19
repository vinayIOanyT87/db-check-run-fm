USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6To8Equipment]   ERIC ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Equipment') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8Equipment
GO


CREATE PROCEDURE [dbo].Migration_ConsolidatedDB6To8Equipment
 /*=============================================
 Author:			Ali Coker
 Create date:		3/8/2010
 Description:		Migrating ConsolidatedDB equipments to tblEquipment
 Modification History:
	Date		by			Description
	04-06-2010	E. Simmons
	
 =============================================*/
/*

EXEC Migration_ConsolidatedDB6To8Equipment 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarchar(MAX) = null

AS 
BEGIN

IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END

SELECT S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8  INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex


	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	

*/	
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	TypeID, SiteIndex, [Index], CreatedDate, CreatedBy
)
SELECT 'User Data', S.SiteIndex8, -1, GETDATE(), 'Varec' FROM  #TMPSITES S WHERE 
NOT EXISTS (SELECT * FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE SiteIndex=S.SiteIndex8 AND TypeID='User Data' )


 SELECT E6.EQTypeIndex AS EqTypeIndex6, E8.EQTypeIndex AS EqTypeIndex8 INTO #TMP_EQUIPMENT_TYPE_INDEX_MAP
 FROM [ConsolidatedDB6].[dbo].tblEquipmentTypes E6 
 JOIN [ConsolidatedDB].[dbo].tblEquipmentTypes E8  ON E6.EqTypeName = E8.EqTypeName 
 JOIN [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=E8.EqTypeIndex AND M.TypeID='Equipment Type' 
 JOIN #TMPSITES S ON M.SiteIndex=S.SiteIndex8 AND E6.SiteIndex=S.SiteIndex6 
 WHERE E6.DeleteFlag = 0 AND E8.DeleteFlag = 0
 

 SELECT P6.ProductIndex AS ProductIndex6, P8.ProductIndex AS ProductIndex8 INTO #TMP_PRODUCT_INDEX_MAP
 FROM [ConsolidatedDB6].[dbo].tblProducts P6 
 JOIN [ConsolidatedDB].[dbo].tblProducts P8  ON P6.ProductCode = P8.ProductID 
 JOIN [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=P8.ProductIndex AND M.TypeID='Products'  
 JOIN #TMPSITES S ON M.SiteIndex=S.SiteIndex8  AND P6.SiteIndex=S.SiteIndex6
 WHERE P6.DeleteFlag = 0 
 
 
IF @IsBaseDB = 0
BEGIN	
	UPDATE	[ConsolidatedDB].[dbo].tblEquipment SET SiteIndex=S.SiteIndex8 
	FROM #TMPSITES S
		WHERE ID IN (SELECT [EquipmentID] FROM [ConsolidatedDB6].[dbo].tblEquipment E WHERE DeleteFlag=0 AND SiteIndex=S.SiteIndex6 ) AND SiteIndex=-1
END

SELECT 
	[EquipmentIndex]							,
	S.SiteIndex8 AS [SiteIndex]									,
	[EquipmentID]								,
	[Description]								,
	T8.[EqTypeIndex]								,
	[ManagerIndex]								,
	[VendorIndex]								,
	[Fixed]										,
	[StorageType]								,
	[InUse]										,
	PM.[ProductIndex8]							AS [ProductIndex],
	[FixedVolume]								,
	[IntoPlane]									,
	[Mobile]									,
	[AttachedTo]								,
	[MediaType]									,
	[Meters]									,
	[DefuelMeterForwards]						,
	[Units]										AS [VolumeUnitIndex]				,
	[PulseRatio]								,
	[Round]										,
	[Xref]										,
	[LowStockWarning]							,
	[StockTrack]								,
	[Totalisor1]								,
	[Totalisor2]								,
	[FuelingState]								,
	dbo.ConvertToSIUnits([Volume], 46)		AS Volume		,--Convert to SI from US Gallons
	[MeterReading]								,
	[Consecutive_OOS_Variance]					,
	[Notes]										,
	GetDate() AS [CreatedDate]								,
	'Varec'										AS [CreatedBy]									,
	E.[UpdatedDate]								,
	E.[UpdatedBy]									,
	E.[DeleteFlag]								,
	T8.Capacity									AS SafeFill,	--Already converted to SI when assigned to EquipmentTypes.
	T8.Capacity,
	T8.IssPt,
	T8.Make,
	T8.Model,
	T8.[Year],
	[IssPtNum]									,
	[SpecialFuelFlag]			AS [FuelAdditiveFlag],
	(CASE WHEN EXISTS(SELECT * FROM  [ConsolidatedDB6].[dbo].tblEquipmentTypes A JOIN [ConsolidatedDB6].[dbo].tblEqTypeAttributeMap B
	ON A.EqTypeIndex=B.EqTypeIndex JOIN [ConsolidatedDB6].[dbo].tblEqTypeAttributes C ON B.AttributeCode=C.AttributeIndex
	WHERE AttributeName='Hydrant' AND A.EqTypeIndex=E.EqTypeIndex)
	THEN 1 
	ELSE 0
	END)							AS [SecondaryStorageFlag],
	1							AS [ManagedEquipmentFlag],
	CASE WHEN UPPER(FuelingState) = 'REFUEL'
	THEN 1
	WHEN UPPER(FuelingState) = 'DEFUEL' 
	THEN 2
	ELSE 0	
	END										AS FuelingType,	
	[UserData1] 								,
	[UserData2] 								,
	[UserData3] 								,
	CASE WHEN LEN([UserData4]) > 0
			THEN UPPER(LEFT([UserData4],1))+
					LOWER(RIGHT([UserData4],LEN([UserData4])-1))
			END									[UserData4] 								,
	[UserData5] 								,
	[UserData6] 								,
	CASE WHEN LEN([UserData7]) > 0
			THEN UPPER(LEFT([UserData7],1))+
					LOWER(RIGHT([UserData7],LEN([UserData7])-1))
			END									[UserData7] 								,
	[UserData8] 								,
	CASE WHEN LEN([UserData9]) > 0
			THEN UPPER(LEFT([UserData9],1))+
					LOWER(RIGHT([UserData9],LEN([UserData9])-1))
			END									[UserData9] 								,
	[UserData10] 								,
	[UserData11] 								,
	[UserData12] 								,
	[UserData13] 								,
	[UserData14] 								,
	[UserData15] 								,
	[UserData16] 								,
	[UserData17] 								,
	[UserData18] 								,
	[UserData19] 								,
	[UserData20] 								,
	[UserData21] 								,
	[UserData22] 								,
	[UserData23] 								,
	[UserData24] 								,
	[RatedFlowRate]								,
	[ActualFlowRate]							,
	[ManufactureDate]							,
	[InstallationDate]							,
	[InspectionDate]							,
	[CalibrationDate]				
INTO #TMP_EQUIPMENT
FROM [ConsolidatedDB6].[dbo].tblEquipment E 
LEFT JOIN #TMP_EQUIPMENT_TYPE_INDEX_MAP TM ON E.EqTypeIndex=TM.EqTypeIndex6
LEFT JOIN [ConsolidatedDB].[dbo].tblEquipmentTypes T8 ON T8.[EqTypeIndex] = TM.[EqTypeIndex8]
LEFT JOIN #TMP_PRODUCT_INDEX_MAP PM ON E.ProductIndex=PM.ProductIndex6
JOIN #TMPSITES S ON E.SiteIndex=S.SiteIndex6
WHERE E.DeleteFlag = 0 AND T8.DeleteFlag = 0   

INSERT INTO [ConsolidatedDB].[dbo].tblEquipment
(
	[SiteIndex],
	[ID] ,
	[Description] ,
	[EqTypeIndex],
	Capacity,
	SafeFill,
	IssPtNum,
	Make,
	Model,
	[Year],
--	[CompanyIndex],
	[Fixed],
	[StorageType] ,
	[InUse] ,
	[ProductIndex],
--	[FuelCardIndex],
	[FixedVolume] ,
	[IntoPlane],
	[Mobile] ,
	[AttachedTo] ,
	[MediaType] ,
	[Meters] ,
	[DefuelMeterForwards] ,
	[PulseRatio] ,
	[Round] ,
	[Xref] ,
	[LowStockWarning] ,
	[StockTrack] ,
	[Totalisor1] ,
	[Totalisor2] ,
	[FuelingState] ,
	[Volume] ,
	[MeterReading],
	[Consecutive_OOS_Variance] ,
	[Notes] ,
	[VolumeUnitIndex],
	--[TemperatureUnitIndex] ,
--	[DensityUnitIndex] ,
--	[MassUnitIndex] ,
--	[VolumeDecimalPlaces] ,
--	[TemperatureDecimalPlaces],
--	[DensityDecimalPlaces] ,
--	[MassDecimalPlaces] ,
--	[EquipmentIndex] ,
--	[EquipmentSequence] ,
--	[LockedOut] ,
--	[LockedOutReason] ,
--	[LockedOutDate],
--	[SerialNumber] ,
--	[CompanyEquipmentID] ,
--	[TruckCardNumber] ,
	[CreatedDate] ,
	[CreatedBy] ,
	[UpdatedDate] ,
	[UpdatedBy] ,
	[QCDate] ,
	[FuelAdditiveFlag] ,
	[SecondaryStorageFlag] ,
	[ManagedEquipmentFlag] ,
	[FuelingType] ,
	[UserData1] ,
	[UserData2] ,
	[UserData3] ,
	[UserData4] ,
	[UserData5] ,
	[UserData6] ,
	[UserData7] ,
	[UserData8] ,
	[UserData9] ,
	[UserData10] ,
	[UserData11] ,
	[UserData12] ,
	[UserData13] ,
	[UserData14] ,
	[UserData15] ,
	[UserData16] ,
	[UserData17] ,
	[UserData18] ,
	[UserData19] ,
	[UserData20] ,
	[UserData21] ,
	[UserData22] ,
	[UserData23] ,
	[UserData24], 
	[RatedGPM] ,
	[ActualGPM] ,
	[ManufactureDate] ,
	[InstallationDate] ,
	[InspectionDate] ,
	[CalibrationDate] 
	)
	SELECT
	TE.[SiteIndex] 									,
	TE.[EquipmentID]								,
	[Description]								,
	[EqTypeIndex]								,
	Capacity,
	SafeFill,
	IssPtNum,
	Make,
	Model,
	[Year],
--	[ManagerIndex]								,
--	[VendorIndex]								,
	[Fixed]										,
	[StorageType]								,
	[InUse]										,
	[ProductIndex]								,
	[FixedVolume]								,
	[IntoPlane]									,
	[Mobile]									,
	[AttachedTo]								,
	[MediaType]									,
	[Meters]									,
	[DefuelMeterForwards]						,
	[PulseRatio]								,
	[Round]										,
	[Xref]										,
	[LowStockWarning]							,
	[StockTrack]								,
	[Totalisor1]								,
	[Totalisor2]								,
	[FuelingState]								,
	[Volume]									,
	[MeterReading]								,
	[Consecutive_OOS_Variance]					,
	[Notes]										,
	[VolumeUnitIndex]							,
	GETDATE()									,
	'Varec',--[CreatedBy]						,
	TE.[UpdatedDate]								,
	TE.[UpdatedBy]									,
	(Select Max(QualityDueDate) from ConsolidatedDB6.dbo.tblQualityControlEQStatus tqces where tqces.EquipmentID = TE.EquipmentID ),--QCDate--	[IssPtNum]									,
	[FuelAdditiveFlag] ,
	[SecondaryStorageFlag] ,
	[ManagedEquipmentFlag] ,
	[FuelingType],
	[UserData1] 								,
	[UserData2] 								,
	[UserData3] 								,
	[UserData4] 								,
	[UserData5] 								,
	[UserData6] 								,
	[UserData7] 								,
	[UserData8] 								,
	[UserData9] 								,
	[UserData10] 								,
	[UserData11] 								,
	[UserData12] 								,
	[UserData13] 								,
	[UserData14] 								,
	[UserData15] 								,
	[UserData16] 								,
	[UserData17] 								,
	[UserData18] 								,
	[UserData19] 								,
	[UserData20] 								,
	[UserData21] 								,
	[UserData22] 								,
	[UserData23] 								,
	[UserData24] 								,
	[RatedFlowRate]								,
	[ActualFlowRate]							,
	[ManufactureDate]							,
	[InstallationDate]							,
	[InspectionDate]							,
	[CalibrationDate]				
FROM #TMP_EQUIPMENT TE
WHERE TE.EquipmentID NOT IN (SELECT ID FROM [ConsolidatedDB].[dbo].tblEquipment E WHERE E.SiteIndex IN (-1, te.SiteIndex))

--Added as per John Ary and Steve Conrad when testing
--on 3/30/2010 at 9:58 PM.  Eric Simmons confirmed with Steve
--to set this to the capacity since the volumetric data in the
--original database had erroneous data
Update [ConsolidatedDB].[dbo].tblEquipment
set Volume = tet.Capacity
from ConsolidatedDB.dbo.tblEquipmentTypes tet
where 
[ConsolidatedDB].[dbo].tblEquipment.EqTypeIndex = tet.EqTypeIndex and
EXISTS(SELECT * FROM #TMPSITES S WHERE ConsolidatedDB.dbo.tblEquipment.SiteIndex = S.SiteIndex8)
	
INSERT INTO [ConsolidatedDB].[dbo].tblEntityToSiteMap
(
	TypeID,
	SiteIndex,
	[Index],
	CreatedDate,
	CreatedBy
)
SELECT
	'Equipment',
	t.SiteIndex,
	[Index],
	GETDATE(),
	'Varec'
FROM [ConsolidatedDB].[dbo].tblEquipment E JOIN #TMP_EQUIPMENT T ON E.ID = T.EquipmentID 
WHERE E.SiteIndex IN (-1, t.SiteIndex) 
AND E.[Index] NOT IN (SELECT [Index] FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap WHERE TypeID='Equipment' AND SiteIndex = t.SiteIndex)


DROP TABLE #TMP_EQUIPMENT
DROP TABLE #TMP_EQUIPMENT_TYPE_INDEX_MAP
DROP TABLE #TMP_PRODUCT_INDEX_MAP
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