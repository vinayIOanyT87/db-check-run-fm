

/*
	EXEC [map].[usp_GetFootnoteToProductMapsByProduct] '00000000-0000-0000-0000-000000000001', '80B08634-D356-4569-B9A2-CD36DF955BD0'
	EXEC [map].[usp_GetFootnoteToProductMapsByProduct] '46426312-E408-4AF8-85FD-338B622B32BF', '80B08634-D356-4569-B9A2-CD36DF955BD0'
	EXEC [map].[usp_GetFootnoteToProductMapsByProduct] '46426312-E408-4AF8-85FD-338B622B32BF', NULL
	EXEC [map].[usp_GetFootnoteToProductMapsByProduct] '00000000-0000-0000-0000-000000000001', NULL

*/



CREATE PROCEDURE [map].[usp_GetFootnoteToProductMapsByProduct]
(
	@TargetSiteGuid uniqueidentifier, @ProductGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetFootnoteToProductMapsByProduct] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the map.tblApplicationStringToFootNoteProduct records for a given Product and a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ProductGuid: Product for which the Footnote mappings are to be retrieved.
	-- 3. This stored procedure replaces the ApplicationStringMapClass.EnumerateByAssignedToGuidAndTypeSQL SQL inline SQL for the case where Type = STRING_MAP_TYPE.FOOT_NOTE_PRODUCT.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 5. This stored procedure must also work/be tested for the special case where the ProductGuid in the mapping is NULL, which indicates a Footnote mapping to ALL Products.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT a.*, b.ID, d.ProductID AssignedToID, d.ProductCode AssignedCode, d.Description AssignedDescription, d.LookupProductTypeIndex AssignedProductType  
		FROM map.tblApplicationStringToFootNoteProduct a
		INNER JOIN tblApplicationString b
		ON b.ApplicationStringGuid = a.ApplicationStringGuid	
		LEFT OUTER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) c
		ON c.MasterRecordGuid = a.ProductGuid
		LEFT OUTER JOIN tblProducts d
		ON d.ProductGuid = c.ProductGuid	
		WHERE 
		( 
			((@ProductGuid IS NOT NULL) AND (a.ProductGuid = @ProductGuid ))
			OR 
			((@ProductGuid IS NULL) AND (a.ProductGuid IS NULL))
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
						+ 'Procedure Name: [map].usp_GetFootnoteToProductMapsByProduct' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
