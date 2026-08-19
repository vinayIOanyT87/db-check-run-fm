
/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Read a Service Request Messaging Configuration record.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMConfigurationSelect]
AS
BEGIN
	SET NOCOUNT ON

	--There should only really be one SRM configuration setting, so we select the most recent one
	--just in case
	SELECT TOP(1) SRMConfigurationGuid,
		SiteGuid,
		MessageRetryAttempts,
		MessageRetryInterval,
		MessageRetentionTime,
		LogFailedMessages,
		LogSuccessfulMessages,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	FROM tblSRMConfiguration WITH (NOLOCK)	
	ORDER BY CreatedDate DESC

END