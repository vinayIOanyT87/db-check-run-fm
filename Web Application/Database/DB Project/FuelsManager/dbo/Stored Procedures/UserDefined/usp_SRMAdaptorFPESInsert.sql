

/*
=============================================
Author: Ryan Hill
Create date: 8/23/12
Description:

Insert the custom configuration data for the Delta FPES adaptor
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFPESInsert]
(
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMAdaptorFPESGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMAdaptorFPESGuid = NEWID()

	INSERT INTO tblSRMAdaptorFPES
	(
		SRMAdaptorFPESGuid,
		TransactionAliasGuid,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@SRMAdaptorFPESGuid,
		@TransactionAliasGuid,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END