

CREATE FUNCTION [dbo].[udf_CheckUniquenessAutoDistributionReasonCode]
(@AutoDistributionReasonCodeGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ReasonCode nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblAutoDistributionReasonCode
	IF 0 < (SELECT COUNT(*) FROM tblAutoDistributionReasonCodes e
	LEFT JOIN map.tblEntityAutoDistributionReasonCodeToSite em1 ON em1.AutoDistributionReasonCodeGuid = e.AutoDistributionReasonCodeGuid
	RIGHT JOIN map.tblEntityAutoDistributionReasonCodeToSite em2 ON em2.AutoDistributionReasonCodeGuid = @AutoDistributionReasonCodeGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.AutoDistributionReasonCodeGuid <> @AutoDistributionReasonCodeGuid
	AND ReasonCode = @ReasonCode)
		SET @Exists = 0

	RETURN @Exists
END
