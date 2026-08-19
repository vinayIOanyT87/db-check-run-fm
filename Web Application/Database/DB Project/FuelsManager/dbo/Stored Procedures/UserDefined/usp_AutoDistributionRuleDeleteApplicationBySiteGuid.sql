

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/10/2012
-- Description:	Delete(cascade) all Rules for a given site from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteApplicationBySiteGuid]
	@SiteGuid UNIQUEIDENTIFIER

AS
BEGIN
	SET NOCOUNT ON

	DECLARE @AutoDistributionRuleGuid UNIQUEIDENTIFIER
	DECLARE ruleCursor CURSOR FOR
	
	-- Prepare cursor
	SELECT 
		AutoDistributionRuleGuid
	FROM 
		[dbo].[tblAutoDistributionRule]
	WHERE 
		SiteGuid = @SiteGuid
	  
	OPEN ruleCursor;
	
	FETCH NEXT
	FROM ruleCursor
	INTO @AutoDistributionRuleGuid;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC [dbo].[usp_AutoDistributionRuleDeleteApplication] @AutoDistributionRuleGuid
		FETCH NEXT
		FROM ruleCursor
		INTO @AutoDistributionRuleGuid;

	END
	CLOSE ruleCursor;

END