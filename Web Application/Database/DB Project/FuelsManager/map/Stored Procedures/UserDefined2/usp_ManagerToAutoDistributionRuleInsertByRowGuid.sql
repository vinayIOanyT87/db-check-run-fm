
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@ManagerToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblManagerToAutoDistributionRule]
	( 
		ManagerToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ManagerGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ManagerGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ManagerToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END