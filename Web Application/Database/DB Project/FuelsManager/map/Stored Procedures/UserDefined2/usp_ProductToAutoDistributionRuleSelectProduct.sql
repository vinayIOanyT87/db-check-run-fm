
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleSelectProduct] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ProductToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ProductGuid,
		ASSIGNED.*
	FROM 
		[map].[tblProductToAutoDistributionRule] MAIN WITH (NOLOCK)
		Inner Join [erv].[udf_GetProductRecordVersions](@SiteGuid) rp
		ON MAIN.ProductGuid = rp.MasterRecordGuid 
		inner join tblProducts ASSIGNED WITH(NOLOCK) 
		on ASSIGNED.ProductGuid = rp.ProductGuid 
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END