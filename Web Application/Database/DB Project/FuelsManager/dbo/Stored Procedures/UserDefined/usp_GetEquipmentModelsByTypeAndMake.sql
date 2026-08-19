

/*
	EXEC [dbo].[usp_GetEquipmentModelsByTypeAndMake] NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipmentModelsByTypeAndMake] NULL, NULL, 'Make_SGG_001'

*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentModelsByTypeAndMake]
(
	@TargetSiteGuid uniqueidentifier, @EquipmentType int, @Make nvarchar(30)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetEquipmentModelsByTypeAndMake] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the distinct Equipment Models for a given target site/sitegroup, by Equipment Type and by Make
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @EquipmentType: Limit the results to Equipments that have an EquipmentType value that correspond to the @EquipmentType value
	-- 2. @Make: Limit the results to Equipments that have a Make value that correspond to the @Make value
	-- 3. This stored procedure replaces the EquipmentClass.EnumerateModelsByTypeAndMakeSQL inline SQL.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT DISTINCT b.Model
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblEquipment b ON b.EquipmentGuid = a.EquipmentGuid
		LEFT JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) cx ON cx.MasterRecordGuid = b.CompanyGuid
		LEFT JOIN tblCompanies c ON c.CompanyGuid = cx.CompanyGuid		
		LEFT JOIN tblEquipmentTypes d ON d.EquipmentTypeGuid = b.EquipmentTypeGuid  
		WHERE ((d.LookupEquipmentTypeIndex = @EquipmentType) OR (@EquipmentType IS NULL))
		AND b.Make = @Make
		ORDER BY b.Model

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
						+ 'Procedure Name: [dbo].usp_GetEquipmentModelsByTypeAndMake' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END