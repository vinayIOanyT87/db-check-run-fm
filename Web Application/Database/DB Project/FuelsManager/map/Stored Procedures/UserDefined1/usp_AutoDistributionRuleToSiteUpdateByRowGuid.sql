
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteUpdateByRowGuid] (
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblEntityAutoDistributionRuleToSite]
	SET
		SiteGuid = @SiteGuid, AutoDistributionRuleGuid = @AutoDistributionRuleGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		AutoDistributionRuleToSiteGuid = @AutoDistributionRuleToSiteGuid
END