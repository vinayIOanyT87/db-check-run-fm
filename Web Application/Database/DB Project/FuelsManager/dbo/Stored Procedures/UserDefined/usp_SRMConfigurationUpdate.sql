

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Update a Service Request Messaging Configuration record.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMConfigurationUpdate]
(
	@SRMConfigurationGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@MessageRetryAttempts TINYINT,
	@MessageRetryInterval INT,
	@MessageRetentionTime INT,
	@LogFailedMessages BIT,
	@LogSuccessfulMessages BIT,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE tblSRMConfiguration
	SET SiteGuid = @SiteGuid,
		MessageRetryAttempts = @MessageRetryAttempts,
		MessageRetryInterval = @MessageRetryInterval,
		MessageRetentionTime = @MessageRetentionTime,
		LogFailedMessages = @LogFailedMessages,
		LogSuccessfulMessages = @LogSuccessfulMessages,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE SRMConfigurationGuid = @SRMConfigurationGuid

END