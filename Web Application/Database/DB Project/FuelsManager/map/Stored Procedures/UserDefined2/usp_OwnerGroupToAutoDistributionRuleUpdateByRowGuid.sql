
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleUpdateByRowGuid] (
	@OwnerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGroupGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblOwnerGroupToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, OwnerGroupGuid = @OwnerGroupGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		OwnerGroupToAutoDistributionRuleGuid = @OwnerGroupToAutoDistributionRuleGuid
END