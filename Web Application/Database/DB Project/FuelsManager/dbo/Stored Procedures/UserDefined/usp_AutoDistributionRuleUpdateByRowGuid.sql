
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleUpdateByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@RuleID NVARCHAR(50),
	@RuleDescription NVARCHAR(255),
	@RuleEnabled BIT,
	@DefaultEOM BIT,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@DefaultReasonCodeGuid UNIQUEIDENTIFIER,
	@DefaultNotes NVARCHAR(1000),
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [dbo].[tblAutoDistributionRule]
	SET
		SiteGuid = @SiteGuid, RuleID = @RuleID, 
		RuleDescription = @RuleDescription, RuleEnabled = @RuleEnabled, DefaultEOM = @DefaultEOM, TransactionAliasGuid = @TransactionAliasGuid, 
		DefaultReasonCodeGuid = @DefaultReasonCodeGuid, DefaultNotes = @DefaultNotes, 
		UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid
END