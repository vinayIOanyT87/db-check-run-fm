
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleUpdateByRowGuid] (
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGroupGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblProductGroupToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ProductGroupGuid = @ProductGroupGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ProductGroupToAutoDistributionRuleGuid = @ProductGroupToAutoDistributionRuleGuid
END