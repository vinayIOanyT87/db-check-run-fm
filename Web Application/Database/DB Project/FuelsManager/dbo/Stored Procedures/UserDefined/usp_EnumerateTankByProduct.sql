------------------------------------------------------------------------------------------------------
-- Stored Procedure: [dbo].[usp_EnumerateTankByProduct] 
-- Author: Javi Martin
-- Version/Date: 1.0.000 / 2022-12-07 
-- Purpose: Retrieve all Tanks that contain a product.
-- 
-- Testing:
-- EXEC [dbo].[usp_EnumerateTankByProduct] @SiteGuid @ProductGuid @HideHiddenTanks
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_EnumerateTankByProduct]
	 @SiteGuid uniqueidentifier, 
	 @ProductGuid uniqueidentifier,
	 @HideHiddenTanks bit = 0
AS
BEGIN
	BEGIN TRY	

		SELECT tblTanks.*, 
		E.EstReturnToServiceDate AS ReturnToServiceDate, 
		E.MaintenanceReason AS StatusDescription, 
		E.InServiceFlag, 
		E.Memo AS MaintenanceNote,  
		ISNULL('QC Tag Memo: ' + G.Memo + CHAR(0x0d) + CHAR(0x0d), '') + ISNULL( 'Test Result Memo: ' + F.Memo, '') as QCNote,  
		G.QualityTagGuid, 
		G.SiteGuid AS QualitySiteGuid, 
		G.Name, 
		G.Severity, 
		G.Active , 
		DeviceID,
		CAST (NULL AS nvarchar(30)) AS ProductID,
		CAST (NULL AS nvarchar(15)) AS ProductCode,
		CAST (NULL AS nvarchar(30)) AS ManagerID,
		CAST (NULL AS nvarchar(15)) AS ManagerCode,
		CAST (NULL AS nvarchar(30)) AS OwnerID,
		CAST (NULL AS nvarchar(15)) AS OwnerCode 
		INTO #results
		FROM tblTanks 
		LEFT JOIN tblTankMaintenanceLog E 
		ON E.TankGuid  = tblTanks.[TankGuid]  
		LEFT JOIN tblTestSetTankResults F 
		ON F.TankGuid = tblTanks.[TankGuid]  
		LEFT JOIN tblAssetTrackingDevice atd 
		ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid]  
		LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo 
		FROM tblTankQualityTagLog HH            
		LEFT JOIN tblQualityTags GG  
		ON GG.QualityTagGuid = HH.QualityTagGuid 
		WHERE RemovedDate IS NULL            
		AND  HH.TaggedDate = (SELECT MAX(TaggedDate) 
								FROM tblTankQualityTagLog             
								WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G 
		ON G.TankGuid = tblTanks.[TankGuid]  
		WHERE ProductGuid = @ProductGuid  
		AND tblTanks.SiteGuid = @SiteGuid  
		AND (E.ChangeDate IS NULL 
			OR E.ChangeDate = (SELECT MAX(ChangeDate) 
								FROM tblTankMaintenanceLog 
								WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid))  
		AND (F.ResultTimeStamp        IS NULL 
			OR F.ResultTimeStamp        = (SELECT MAX(ResultTimeStamp) 
										FROM tblTestSetTankResults 
										WHERE tblTestSetTankResults.TankGuid = F.TankGuid))  
		ORDER BY tblTanks.TankID

		IF EXISTS (Select 1 FROM #results )
		BEGIN
			SELECT c.ID, c.Code, c._MasterRecordGuid 
			INTO #SiteCompanies
			FROM tblCompanies c 
			INNER JOIN [erv].[udf_GetCompanyRecordVersions](@SiteGuid) rc 
			ON c.CompanyGuid= rc.CompanyGuid

			SELECT p.ProductID, p.ProductCode, p._MasterRecordGuid 
			INTO #SiteProducts
			FROM tblProducts p 
			INNER JOIN [erv].[udf_GetProductRecordVersions](@SiteGuid) rc 
			ON p.ProductGuid= rc.ProductGuid

			UPDATE r
			SET ProductID =  p.ProductID,
			ProductCode = p.ProductCode
			FROM #results r
			JOIN #SiteProducts p 
			ON p._MasterRecordGuid = r.ProductGuid

			UPDATE r
			SET ManagerID =  c.ID,
			ManagerCode = c.Code
			FROM #results r
			JOIN #SiteCompanies c 
			ON c._MasterRecordGuid = r.ManagerCompanyGuid

			UPDATE r
			SET OwnerID =  c.ID,
			OwnerCode = c.Code
			FROM #results r
			JOIN #SiteCompanies c 
			ON c._MasterRecordGuid = r.OwnerCompanyGuid
		END

		SELECT * 
		FROM #results 
		WHERE (@HideHiddenTanks = 0) OR ( HiddenDate IS NULL AND @HideHiddenTanks = 1)


	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_EnumerateTankByProduct' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END