/*
	DROP FUNCTION [archive].[udf_GetAllScopesArchivingOnString]

	SELECT [archive].[udf_GetAllScopesArchivingOnString] ()	

*/
CREATE FUNCTION [archive].[udf_GetAllScopesArchivingOnString]
(
)
RETURNS NVARCHAR(250)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [archive].[udf_GetAllScopesArchivingOnString]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns a string that combines the IsArchivingOn flag status of all the scopes.
	-- Notes:
	------------------------------------------------------------------------------------------------------

	DECLARE @isArchivingOn bit
	DECLARE @scopeId nvarchar(50)
	DECLARE @result nvarchar(250)
	
	SET @result = NULL

	DECLARE ArchiveScopeCursor CURSOR FOR 
	  SELECT ScopeId, IsArchivingOn FROM [archive].[tblArchiveScope]
	  ORDER BY ScopeId
	OPEN ArchiveScopeCursor 

		FETCH NEXT FROM ArchiveScopeCursor INTO @ScopeId, @IsArchivingOn 
 
		WHILE @@FETCH_STATUS = 0  
		BEGIN 
			IF (@result IS NOT NULL)
			BEGIN
				SET @result = @result + '; '
			END
			SET @result = ISNULL(@result, '') + '[' + @scopeId + '].[IsArchivingOn] = ' + (CASE WHEN ISNULL(@isArchivingOn, 0) = 1 THEN 'True' ELSE 'False' END)
		
			FETCH NEXT FROM ArchiveScopeCursor INTO @ScopeId, @IsArchivingOn  
		END 
	CLOSE ArchiveScopeCursor 
	DEALLOCATE ArchiveScopeCursor 
	
	RETURN @result;

END

GO

