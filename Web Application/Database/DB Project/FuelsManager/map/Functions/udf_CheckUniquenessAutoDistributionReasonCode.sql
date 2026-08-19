

CREATE FUNCTION [map].[udf_CheckUniquenessAutoDistributionReasonCode]
(@AutoDistributionReasonCodeGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ReasonCode nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @ReasonCode = (SELECT ReasonCode FROM tblAutoDistributionReasonCodes e WHERE e.AutoDistributionReasonCodeGuid = @AutoDistributionReasonCodeGuid)
	IF 0 < (SELECT COUNT(*) FROM tblAutoDistributionReasonCodes e 
	RIGHT JOIN map.tblEntityAutoDistributionReasonCodeToSite em ON em.SiteGuid = @SiteGuid AND em.AutoDistributionReasonCodeGuid = e.AutoDistributionReasonCodeGuid 
	WHERE e.AutoDistributionReasonCodeGuid <> @AutoDistributionReasonCodeGuid
	AND e.ReasonCode = @ReasonCode)
		SET @Exists = 0

	RETURN @Exists
END
