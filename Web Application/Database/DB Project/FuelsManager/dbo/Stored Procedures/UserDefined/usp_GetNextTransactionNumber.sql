CREATE PROCEDURE [dbo].[usp_GetNextTransactionNumber]
	@SiteGuid uniqueidentifier,
	@UpdatedBy nvarchar(50),
	@UpdatedDate datetimeoffset
AS
	DECLARE @NextNumberTable AS Table (NextNumber int)
	DECLARE @NextNumber int

	UPDATE tblSites SET TransactionNextNumber = TransactionNextNumber + 1,
                                UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
                                OUTPUT DELETED.TRansactionNextNumber INTO @NextNumberTable (NextNumber)
                                WHERE SiteGuid = @SiteGuid

	SELECT NextNumber FROM @NextNumberTable

	RETURN 0
