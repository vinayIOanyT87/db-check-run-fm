CREATE PROCEDURE [dbo].[usp_ExportPaiceGetOwnerCodeByTransID]
(
	@TransID nvarchar (64)
)
AS
BEGIN

	DECLARE @OwnerCode nvarchar(10)
	SELECT @OwnerCode=OwnerCode FROM tblTransactions
		WHERE TransID = @TransID
	select @OwnerCode
		
END
