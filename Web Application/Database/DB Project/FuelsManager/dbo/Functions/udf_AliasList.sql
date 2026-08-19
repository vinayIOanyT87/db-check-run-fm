

CREATE FUNCTION [dbo].[udf_AliasList]
(@SiteGuid UNIQUEIDENTIFIER)
RETURNS TABLE 
AS
RETURN 
    SELECT b.AliasName,
		   b.LookupTransTypeIndex,
		   b._MasterRecordGuid TransactionAliasGuid
	FROM [erv].[udf_GetTransactionAliasRecordVersions](@SiteGuid) a
	INNER JOIN tblTransactionAliases b
	ON b.TransactionAliasGuid = a.TransactionAliasGuid