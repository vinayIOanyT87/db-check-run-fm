CREATE PROCEDURE [dbo].[usp_ErrorTransactionSubmissionInsert]
	@SubmittedUserGuid UNIQUEIDENTIFIER,
	@SubmittedSiteGuid UNIQUEIDENTIFIER,
	@TransactionSubmissionInformation NVARCHAR(MAX),
	@CreatedBy nvarchar(100),
	@UpdatedBy nvarchar(100)
AS
	--SELECT @SubmittedUserGuid, @SubmittedSiteGuid, @TransactionSubmissionInformation
	SET NOCOUNT ON;	
	DECLARE @InsertedGuids TABLE(InsertedGuid UNIQUEIDENTIFIER)
	INSERT INTO [dbo].[tblErrorTransactionSubmissions]
	(
		[SubmittedUserGuid],
		[SubmittedSiteGuid],
		[TransactionSubmissionInformation],
		[CreatedBy],
		[CreatedDate],
		[UpdatedBy],
		[UpdatedDate]
	)
	OUTPUT INSERTED.ErrorTransactionSubmissionGuid INTO @InsertedGuids(InsertedGuid)
	VALUES
	(
		@SubmittedUserGuid,
		@SubmittedSiteGuid,
		@TransactionSubmissionInformation,
		@CreatedBy,
		GETDATE(),
		@UpdatedBy,
		GETDATE()
	);
SELECT TOP 1 InsertedGuid FROM @InsertedGuids;