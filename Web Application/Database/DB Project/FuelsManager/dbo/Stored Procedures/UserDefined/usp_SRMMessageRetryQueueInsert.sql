

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Enqueue a Service Request Messaging Archived Message to be retried
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMMessageRetryQueueInsert]
(
	@SRMMessageGuid UNIQUEIDENTIFIER,
	@AttemptNumber INT,
	@ConvertedMessageXML XML,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMMessageRetryQueueGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMMessageRetryQueueGuid = NEWID()

	INSERT INTO tblSRMMessageRetryQueue
	(
		SRMMessageRetryQueueGuid,
		SRMMessageGuid,
		AttemptNumber,
		ConvertedMessageXML,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@SRMMessageRetryQueueGuid,
		@SRMMessageGuid,
		@AttemptNumber,
		@ConvertedMessageXML,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)

END