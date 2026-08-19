
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleUpdateByRowGuid] (
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblOwnerToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, OwnerGuid = @OwnerGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		OwnerToAutoDistributionRuleGuid = @OwnerToAutoDistributionRuleGuid
END