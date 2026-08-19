
CREATE PROCEDURE [dbo].[usp_TransactionPIDXGet]
(
	@TransactionGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	-- The use of the NOLOCK hint in this procedure is questionable at best. It should be removed and any deadlocks that result should be fixed.
	SELECT p.*, 
		t.SiteGuid, 
		t.TransID 
	FROM tblTransactionPIDX p WITH(NOLOCK) 
	LEFT OUTER JOIN tblTransactions t WITH(NOLOCK) ON p.TransactionGuid = t.TransactionGuid 
	WHERE p.TransactionGuid = @TransactionGuid AND t.SiteGuid = @SiteGuid

END 

