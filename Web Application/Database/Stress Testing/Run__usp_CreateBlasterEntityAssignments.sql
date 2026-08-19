USE [ConsolidatedDB]
GO

-- ==============================================================================================
-- Author:		Ivan Orndorff
-- Create date: April 29, 2009
-- Description:	This will run the usp_CreateBlasterEntityAssignments for all seventy generated sites
-- Version:		1.0
-- ==============================================================================================

DECLARE @SiteIndexArg int
DECLARE TheCursor CURSOR FOR
	SELECT SiteIndex FROM tblSites WHERE 
		ID LIKE '%site %' AND (ID NOT IN ('Site 1', 'Site 2', 'Site 3'))
	
OPEN TheCursor
FETCH NEXT FROM TheCursor INTO @SiteIndexArg
WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC usp_CreateBlasterEntityAssignments
		@SiteIndex = @SiteIndexArg				-- Run each of the Sites.

	FETCH NEXT FROM TheCursor INTO @SiteIndexArg
END

-- Close and deallocate the cursor.
CLOSE TheCursor
DEALLOCATE TheCursor