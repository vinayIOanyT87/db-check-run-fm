
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleUpdateByRowGuid] (
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblProductToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ProductGuid = @ProductGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ProductToAutoDistributionRuleGuid = @ProductToAutoDistributionRuleGuid
END