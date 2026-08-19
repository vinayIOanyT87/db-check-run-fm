
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteInsertByRowGuid] (
	@SiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblEntityAutoDistributionRuleToSite]
	( 
		AutoDistributionRuleToSiteGuid, SiteGuid, AutoDistributionRuleGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @SiteGuid, @AutoDistributionRuleGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @AutoDistributionRuleToSiteGuid = @NewPrimaryKeyGuid
END