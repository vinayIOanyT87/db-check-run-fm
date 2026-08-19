

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Delete a Service Request Messaging Adaptor Configuration record.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorDelete]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TransactionCreated BIT
	SET @TransactionCreated = 0;

	-- If this procedure wasn't executed from a WCF method, make sure we rollback any changes if something goes wrong
	BEGIN TRY		
		IF @@TRANCOUNT = 0 -- A transaction does not exist
		BEGIN
			-- Open a transaction
			BEGIN TRAN
			SET @TransactionCreated = 1;
		END

		DELETE FROM map.tblSRMAdaptorIATAToSite 
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid

		DELETE FROM tblSRMAdaptorFilter 
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid

		DELETE FROM tblSRMAdaptor
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid

		-- If the transaction was created by the procedure, commit it.
		IF @TransactionCreated = 1
		BEGIN
			COMMIT
		END

	END TRY
	BEGIN CATCH
		DECLARE @ErrMessage NVARCHAR(2048)
			,	@ErrNumber INT
			,	@ErrProcName NVARCHAR(126)
			,	@LineNumber INT
		
		SET @ErrMessage = ERROR_MESSAGE()
		SET	@ErrNumber = ERROR_NUMBER()
		SET @ErrProcName= ERROR_PROCEDURE()
		SET @LineNumber = ERROR_LINE()
		
		SET @ErrMessage =		'Error: ' + @ErrMessage + CHAR(13)+CHAR(10)
							+	'Number: ' + CAST(@ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10) 
							+	'Procedure Name: ' + ISNULL(@ErrProcName,OBJECT_NAME(@@PROCID)) + CHAR(13)+CHAR(10) 
							+	'Line Number: ' + ISNULL(CAST(@LineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10) 

		RAISERROR(@ErrMessage, 16, 1)
		
		-- If the transaction was created by the procedure, roll it back.
		IF @TransactionCreated = 1 
		BEGIN
			ROLLBACK
		END
	END CATCH
END