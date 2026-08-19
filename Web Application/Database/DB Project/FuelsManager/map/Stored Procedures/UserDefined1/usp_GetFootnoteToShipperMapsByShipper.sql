
CREATE PROCEDURE [map].[usp_GetFootnoteToShipperMapsByShipper]
(
	@TargetSiteGuid uniqueidentifier, @ShipperGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetFootnoteToShipperMapsByShipper] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the map.tblApplicationStringToFootNoteProduct records for a given Shipper Company and a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Companies that have been assigned to this site/sitegroup only
	-- 2. @ShipperGuid: Company for which the Footnote mappings are to be retrieved.
	-- 3. This stored procedure replaces the ApplicationStringMapClass.EnumerateByAssignedToGuidAndTypeSQL SQL inline SQL for the case where Type = STRING_MAP_TYPE.FOOT_NOTE_SHIPPER.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 5. This stored procedure must also work/be tested for the special case where the CompanyGuid in the mapping is NULL, which indicates a Footnote mapping to ALL Companies.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT a.*, b.ID AS ID, d.ID AS AssignedToID, d.Name AS AssignedToName, d.Address1 AS AssignedToAddress, d.City AS AssignedToCity, d.State As AssignedToState 
		FROM map.tblApplicationStringToFootNoteShipper a 
		INNER JOIN tblApplicationString b
		ON b.ApplicationStringGuid = a.ApplicationStringGuid
		LEFT OUTER JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) c
		ON c.MasterRecordGuid = a.CompanyGuid
		LEFT OUTER JOIN tblCompanies d
		ON d.CompanyGuid = c.CompanyGuid	 
		WHERE 
		( 
			((@ShipperGuid IS NOT NULL) AND (a.CompanyGuid = @ShipperGuid ))
			OR 
			((@ShipperGuid IS NULL) AND (a.CompanyGuid IS NULL))
		)
		ORDER BY a.Sequence

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
						+ 'Procedure Name: [map].usp_GetFootnoteToShipperMapsByShipper' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     


