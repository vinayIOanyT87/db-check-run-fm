

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleSelectTransactionAlias] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN


DECLARE @startSiteIndex uniqueidentifier
SELECT @startSiteIndex = a.SiteGuid FROM dbo.tblAutoDistributionRule a 
WHERE a.AutoDistributionRuleGuid = @AutoDistributionRuleGuid

	SELECT 
		MAIN.TransactionAliasToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.TransactionAliasGuid,
		ASSIGNED.*
	FROM 
		[map].[tblTransactionAliasToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblTransactionAliases] ASSIGNED WITH (NOLOCK)
		ON MAIN.TransactionAliasGuid = ASSIGNED._MasterRecordGuid
		INNER JOIN [erv].[udf_GetTransactionAliasRecordVersions](@startSiteIndex) a
		ON a.TransactionAliasGuid = ASSIGNED.TransactionAliasGuid

	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END