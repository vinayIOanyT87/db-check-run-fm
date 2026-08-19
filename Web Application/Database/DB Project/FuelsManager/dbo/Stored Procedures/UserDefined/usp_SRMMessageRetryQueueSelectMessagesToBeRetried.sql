
/*
=============================================
Author: Ryan Hill
Create date: 7/30/12
Description:

Get messages that need to be retried, with the maximum number of messages 
returned specified as a parameter
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMMessageRetryQueueSelectMessagesToBeRetried]
(
	@RetryMessageCount INT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Results TABLE 
	(
		SRMMessageRetryQueueGuid UNIQUEIDENTIFIER NOT NULL, 
		SRMMessageGuid UNIQUEIDENTIFIER NOT NULL, 
		RetryID BIGINT NOT NULL,
		AttemptNumber INT NOT NULL,
		ConvertedMessageXML XML NOT NULL,
		CreatedDate DATETIMEOFFSET(7),
		CreatedBy dbo.udtUserID NOT NULL,
		UpdatedDate DATETIMEOFFSET(7), 
		UpdatedBy dbo.udtUserID NOT NULL
	);

	-- Limit the maximum number of messages returned so that the SRM Service doesn't get overwhelmed 
	-- The use of ROWLOCK, READPAST here should ensure that if multiple threads call this procedure simultaneously, 
	-- The results will be split across the threads rather than one thread getting all of the messages.
	WITH RecordsToDequeue AS 
	(
		SELECT TOP(@RetryMessageCount)
			SRMMessageRetryQueueGuid, 
			SRMMessageGuid, 
			RetryID,
			AttemptNumber,
			ConvertedMessageXML,
			CreatedDate,
			CreatedBy,
			UpdatedDate, 
			UpdatedBy
		FROM tblSRMMessageRetryQueue WITH (ROWLOCK, READPAST)
		ORDER BY RetryID
	)
	DELETE 
	FROM RecordsToDequeue  
	OUTPUT 
		Deleted.SRMMessageRetryQueueGuid, 
		Deleted.SRMMessageGuid, 
		Deleted.RetryID,
		Deleted.AttemptNumber,
		Deleted.ConvertedMessageXML,
		Deleted.CreatedDate,
		Deleted.CreatedBy,
		Deleted.UpdatedDate, 
		Deleted.UpdatedBy
	INTO @Results

	SELECT * FROM @Results
END