
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByOwnerGroupGuid]
	@OwnerGroupGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerGroupToAutoDistributionRule]
			WHERE [OwnerGroupGuid] = @OwnerGroupGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerGroupToAutoDistributionRule] 
	WHERE [OwnerGroupGuid] = @OwnerGroupGuid; 
END