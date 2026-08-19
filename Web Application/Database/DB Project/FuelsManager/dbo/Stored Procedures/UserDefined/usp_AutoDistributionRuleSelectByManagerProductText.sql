
-- ==================================================================================================================
-- Author:		Daniel Or
-- Updated Date:	7/31/2013
-- Description:	Select record(s) from the [dbo].[tblAutoDistributionRule] by Manger/Product/Rule Description/Rule ID
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleSelectByManagerProductText] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ManagerGuid UNIQUEIDENTIFIER = NULL,
	@ProductGuid UNIQUEIDENTIFIER = NULL,
	@FindText NVARCHAR(1024) = NULL
) AS
BEGIN	
	
	DECLARE @SearchText NVARCHAR(1024)
	
	SET @SearchText = 
		CASE WHEN @FindText IS NULL OR RTRIM(@FindText) = ''
			THEN  NULL 
			ELSE @FindText
		END
	

	CREATE TABLE #tblAutoDistributionRuleSelectManagerProduct (
		[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
	)
	-- Insert all matched Managers and Products into the temp table
	IF @ManagerGuid IS NOT NULL OR @ProductGuid IS NOT NULL
	BEGIN
		INSERT INTO #tblAutoDistributionRuleSelectManagerProduct
		SELECT
			AutoDistributionRuleGuid
		FROM
			[dbo].[vw_AutoDistributionRuleManagersProducts] WITH (NOLOCK)
		WHERE
			((@ManagerGuid IS NULL) OR (@ManagerGuid = [CompanyGuid]))
			AND ((@ProductGuid IS NULL) OR (@ProductGuid = [ProductGuid]))
	END
	ELSE
	BEGIN
		-- no manager/product specified, select all rules
		INSERT INTO #tblAutoDistributionRuleSelectManagerProduct
		SELECT
			AutoDistributionRuleGuid
		FROM
			[dbo].[tblAutoDistributionRule]	 WITH (NOLOCK)
	END
	
	CREATE TABLE #tblAutoDistributionRuleSelect (
		[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
	)
	
	IF @SearchText IS NULL
	BEGIN
		INSERT INTO #tblAutoDistributionRuleSelect
		SELECT
			AutoDistributionRuleGuid
		FROM
			 #tblAutoDistributionRuleSelectManagerProduct WITH (NOLOCK)
	END
	ELSE
	BEGIN

		-- Search for Manager ID, Product ID and RuleDescription
		INSERT INTO #tblAutoDistributionRuleSelect
		SELECT
			MP.AutoDistributionRuleGuid
		FROM
			[dbo].[vw_AutoDistributionRuleManagersProducts] ALLRULES WITH (NOLOCK)
			INNER JOIN #tblAutoDistributionRuleSelectManagerProduct MP WITH (NOLOCK)
			ON MP.AutoDistributionRuleGuid = ALLRULES.AutoDistributionRuleGuid
		WHERE
			(ALLRULES.CompanyID LIKE '%'+ @SearchText + '%')
			OR (ALLRULES.ProductID LIKE '%'+ @SearchText + '%')
			OR (ALLRULES.RuleDescription LIKE '%'+ @SearchText + '%')
			OR (ALLRULES.RuleID LIKE '%'+ @SearchText + '%')
	
		-- Manager Group ID
		INSERT INTO #tblAutoDistributionRuleSelect
		SELECT
			MAIN.AutoDistributionRuleGuid
		FROM
			[dbo].[vw_AutoDistributionRuleManagersProducts] MP WITH (NOLOCK)
			INNER JOIN [dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)			
			ON MAIN.AutoDistributionRuleGuid = MP.AutoDistributionRuleGuid
			
			
			INNER JOIN [map].[tblManagerGroupToAutoDistributionRule] MGPMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = MGPMAP.AutoDistributionRuleGuid

			INNER JOIN [dbo].[tblApplicationString] APPSTR WITH (NOLOCK)
			ON APPSTR.ApplicationStringGuid = MGPMAP.ManagerGroupGuid
						
		WHERE
			APPSTR.ID LIKE '%'+ @SearchText + '%'

		-- Product Group ID
		INSERT INTO #tblAutoDistributionRuleSelect
		SELECT
			MAIN.AutoDistributionRuleGuid
		FROM
			[dbo].[vw_AutoDistributionRuleManagersProducts] MP WITH (NOLOCK)
			INNER JOIN [dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)			
			ON MAIN.AutoDistributionRuleGuid = MP.AutoDistributionRuleGuid
			
			INNER JOIN [map].[tblProductGroupToAutoDistributionRule] RPGPMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = RPGPMAP.AutoDistributionRuleGuid

			INNER JOIN [dbo].[tblApplicationString] APPSTR WITH (NOLOCK)
			ON APPSTR.ApplicationStringGuid = RPGPMAP.ProductGroupGuid

		WHERE
			APPSTR.ID LIKE '%'+ @SearchText + '%'
			
	
	END

	SELECT 
		MAIN.AutoDistributionRuleGuid, MAIN.SiteGuid, MAIN.RuleID, 
		MAIN.RuleDescription, MAIN.RuleEnabled, MAIN.DefaultEOM, MAIN.TransactionAliasGuid, 
		MAIN.DefaultReasonCodeGuid, MAIN.DefaultNotes, MAIN.CreatedDate, MAIN.CreatedBy, 
		MAIN.UpdatedDate, MAIN.UpdatedBy, MAIN._RowVersion
	FROM 
		[dbo].[udf_AutoDistributionRuleSelectRulesBySite](@SelectedSiteGuid,@LoginSiteGuid) MAIN 		
		INNER JOIN 
		(
			SELECT DISTINCT [AutoDistributionRuleGuid] FROM #tblAutoDistributionRuleSelect WITH (NOLOCK)
		) RESULT 
		ON MAIN.AutoDistributionRuleGuid = RESULT.AutoDistributionRuleGuid
		
	ORDER BY
		MAIN.RuleID

	DROP TABLE #tblAutoDistributionRuleSelect			
END
