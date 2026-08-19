
/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Insert a record into the tblSRMConfiguration table.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMConfigurationInsert]
(
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@MessageRetryAttempts TINYINT,
	@MessageRetryInterval INT,
	@MessageRetentionTime INT = NULL,
	@LogFailedMessages BIT,
	@LogSuccessfulMessages BIT,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMConfigurationGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMConfigurationGuid = NEWID()

	INSERT INTO tblSRMConfiguration
	(
		SRMConfigurationGuid,
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
	)
	VALUES
	(
		@SRMConfigurationGuid,
		@SiteGuid,
		@MessageRetryAttempts,
		@MessageRetryInterval,
		@MessageRetentionTime,
		@LogFailedMessages,
		@LogSuccessfulMessages,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END