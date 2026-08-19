

/*
=============================================
Author: Ryan Hill
Create date: 8/23/12
Description:

Update the custom configuration data for the Delta FPES adaptor
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFPESUpdate]
(
	@SRMAdaptorFPESGuid UNIQUEIDENTIFIER,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE tblSRMAdaptorFPES
	SET
		SRMAdaptorFPESGuid = @SRMAdaptorFPESGuid,
		TransactionAliasGuid = @TransactionAliasGuid,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE SRMAdaptorFPESGuid = @SRMAdaptorFPESGuid
END