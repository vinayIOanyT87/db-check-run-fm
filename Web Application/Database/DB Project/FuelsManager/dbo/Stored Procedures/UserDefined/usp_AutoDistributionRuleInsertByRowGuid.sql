
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleInsertByRowGuid] (
	@SiteGuid UNIQUEIDENTIFIER,
	@RuleID NVARCHAR(50),
	@RuleDescription NVARCHAR(255),
	@RuleEnabled BIT,
	@DefaultEOM BIT,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@DefaultReasonCodeGuid UNIQUEIDENTIFIER,
	@DefaultNotes NVARCHAR(1000),
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [dbo].[tblAutoDistributionRule]
	( 
		AutoDistributionRuleGuid, SiteGuid, RuleID, 
		RuleDescription, RuleEnabled, DefaultEOM, TransactionAliasGuid, 
		DefaultReasonCodeGuid, DefaultNotes, CreatedDate, CreatedBy, 
		UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @SiteGuid, @RuleID, 
		@RuleDescription, @RuleEnabled, @DefaultEOM, @TransactionAliasGuid, 
		@DefaultReasonCodeGuid, @DefaultNotes, @CreatedDate, @CreatedBy, 
		@UpdatedDate, @UpdatedBy
	)
	SET @AutoDistributionRuleGuid = @NewPrimaryKeyGuid
END