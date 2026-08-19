
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByRowGuid]
	@OwnerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerGroupToAutoDistributionRule]
			WHERE [OwnerGroupToAutoDistributionRuleGuid] = @OwnerGroupToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerGroupToAutoDistributionRule] 
	WHERE [OwnerGroupToAutoDistributionRuleGuid] = @OwnerGroupToAutoDistributionRuleGuid; 
END