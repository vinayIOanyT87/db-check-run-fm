
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleUpdateByRowGuid] (
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGroupGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblManagerGroupToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ManagerGroupGuid = @ManagerGroupGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ManagerGroupToAutoDistributionRuleGuid = @ManagerGroupToAutoDistributionRuleGuid
END