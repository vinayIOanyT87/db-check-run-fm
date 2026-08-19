

/****************************************************************************************** 
  usp_tblAutoDistributionReasonCodes_Select 
  This SP selects data from the tblAutoDistributionReasonCodes table.
  Required parameters: @IsInTransaction, @SiteGuid, @LoginSiteGuid
  Optional parameters for filtering: @ReasonCode, @ReasonCodeGuid (passing in NULL if not used)
******************************************************************************************/
CREATE PROCEDURE [dbo].[usp_tblAutoDistributionReasonCodes_Select]
	@SiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ReasonCode NVARCHAR(50) = NULL,
	@ReasonCodeGuid UNIQUEIDENTIFIER = NULL
AS
BEGIN
	SELECT 
		RCODE.AutoDistributionReasonCodeGuid, RCODE.SiteGuid, RCODE.ReasonCode, 
		RCODE.[Description], RCODE.CreatedDate, RCODE.CreatedBy, 
		RCODE.UpdatedDate, RCODE.UpdatedBy 
	FROM 
		dbo.tblAutoDistributionReasonCodes RCODE WITH (NOLOCK)
		INNER JOIN map.tblEntityAutoDistributionReasonCodeToSite RMAP WITH (NOLOCK)
		ON RCODE.AutoDistributionReasonCodeGuid = RMAP.AutoDistributionReasonCodeGuid
			
	WHERE
		/* the site is assigned to the current site */
		RMAP.SiteGuid = @SiteGuid
		AND ( (@ReasonCode IS NULL) OR (@ReasonCode = RCODE.ReasonCode) )
		AND ( (@ReasonCodeGuid IS NULL) OR (@ReasonCodeGuid = RCODE.AutoDistributionReasonCodeGuid) )
END
