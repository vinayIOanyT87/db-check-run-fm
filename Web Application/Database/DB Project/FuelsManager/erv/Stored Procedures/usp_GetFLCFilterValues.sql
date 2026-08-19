

/*
	EXEC [erv].[usp_GetFLCFilterValues]
	EXEC [erv].[usp_GetFLCFilterValues] 'Equipment', '00000000-0000-0000-0000-000000000001', 'EquipmentTypeGuid'
*/
CREATE PROCEDURE [erv].[usp_GetFLCFilterValues]
(
	@EntityTypeId nvarchar(100) = NULL,
	@SiteGuid uniqueidentifier = NULL,
	@FilterFieldName nvarchar(100) = NULL
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetFLCFilterValues] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the Field Level Configuration filter values for a given filter of an entity type
	-- Notes:
	-- 1. 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @tblFilterValues TABLE
		(
			EntitySegmentTemplateGuid uniqueidentifier
			, EntityTypeId nvarchar(100)
			, FilterFieldName nvarchar(100)
			, FilterValueGuid uniqueidentifier
			, FilterValueName nvarchar(100)
			, SiteGuid uniqueidentifier
		);

		DECLARE @generate nvarchar(150)
		DECLARE @ParmDefinition nvarchar(500);
		DECLARE @entitySegmentTemplateGuid uniqueidentifier
		DECLARE @entityTypeIdTemp nvarchar(100)
		DECLARE @filterFieldNameTemp nvarchar(100)
		DECLARE @filterValuesStoredProc nvarchar(100)

		DECLARE EntitySegmentFilterCursor CURSOR FOR 
			SELECT EntitySegmentTemplateGuid, EntityTypeId, FilterFieldName, FilterValuesStoredProc
			FROM erv.tblEntitySegmentTemplate
			WHERE FilterValuesStoredProc IS NOT NULL
			AND ((EntityTypeId = @EntityTypeId) OR (@EntityTypeId IS NULL))
			AND ((FilterFieldName = @FilterFieldName) OR (@FilterFieldName IS NULL))
		OPEN EntitySegmentFilterCursor

		FETCH NEXT FROM EntitySegmentFilterCursor 
		INTO @entitySegmentTemplateGuid, @entityTypeIdTemp, @filterFieldNameTemp, @filterValuesStoredProc

		WHILE @@FETCH_STATUS = 0
		BEGIN			
			SET @generate = N'EXEC ' + @filterValuesStoredProc + N' @sgGuid';
			SET @ParmDefinition = N'@sgGuid uniqueidentifier';
			INSERT @tblFilterValues 
			(FilterValueGuid, FilterValueName, SiteGuid)
			EXECUTE sp_executesql @generate, @ParmDefinition, @sgGuid = @SiteGuid;
			UPDATE @tblFilterValues SET EntitySegmentTemplateGuid = @entitySegmentTemplateGuid, EntityTypeId = @EntityTypeIdTemp, FilterFieldName = @filterFieldNameTemp  WHERE EntitySegmentTemplateGuid IS NULL
			FETCH NEXT FROM EntitySegmentFilterCursor INTO @entitySegmentTemplateGuid, @entityTypeIdTemp, @filterFieldNameTemp, @filterValuesStoredProc
		END 
		CLOSE EntitySegmentFilterCursor;
		DEALLOCATE EntitySegmentFilterCursor;
	
		SELECT DISTINCT EntityTypeId, FilterFieldName, FilterValueGuid, FilterValueName FROM @tblFilterValues
		ORDER BY EntityTypeId, FilterFieldName, FilterValueName

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
						+ 'Procedure Name: dbo.usp_GetFLCFilterValues' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    	
END