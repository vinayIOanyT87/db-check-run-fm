

/*
	EXEC [map].[usp_GetFootnoteToProductMapsByFootnote] '00000000-0000-0000-0000-000000000001', 'eb5826b2-2acb-418c-9985-65999152b9d2'
	EXEC [map].[usp_GetFootnoteToProductMapsByFootnote] '46426312-E408-4AF8-85FD-338B622B32BF', 'eb5826b2-2acb-418c-9985-65999152b9d2'
	EXEC [map].[usp_GetFootnoteToProductMapsByFootnote] '00000000-0000-0000-0000-000000000001', '0E6BC809-D5A7-4F58-894F-E9E7C309EE7C'
*/



CREATE PROCEDURE [map].[usp_GetFootnoteToProductMapsByFootnote]
(
	@TargetSiteGuid uniqueidentifier, @ApplicationStringGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetFootnoteToProductMapsByFootnote] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the map.tblApplicationStringToFootNoteProduct records for a given Footnote and a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ApplicationStringGuid: Footnote for which the mappings are to be retrieved.
	-- 3. This stored procedure replaces the ApplicationStringMapClass.EnumerateByApplicationStringGuidAndTypeSQL SQL inline SQL for the case where Type = STRING_MAP_TYPE.FOOT_NOTE_PRODUCT and where the bInTransaction is false.
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
		WHERE a.ApplicationStringGuid = @ApplicationStringGuid 
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
						+ 'Procedure Name: [map].usp_GetFootnoteToProductMapsByFootnote' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
