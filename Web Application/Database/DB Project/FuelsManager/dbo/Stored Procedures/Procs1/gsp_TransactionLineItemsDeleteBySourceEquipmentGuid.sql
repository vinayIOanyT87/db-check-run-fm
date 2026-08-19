CREATE PROCEDURE [dbo].[gsp_TransactionLineItemsDeleteBySourceEquipmentGuid](@SourceEquipmentGuid uniqueidentifier,@DetachOnly BIT=NULL,@SwapToGuid uniqueidentifier=NULL,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblTransactionLineItems] WHERE [SourceEquipmentGuid] = @SourceEquipmentGuid AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR('Attempted to delete a stale version of TransactionLineItems.',18,1); 
				 RETURN; 
			END 
		END 
		--
		-- REPLACE SourceEquipmentGuid PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT @SwapToGuid IS NULL
		BEGIN
			UPDATE [dbo].[tblTransactionLineItems]
			SET [SourceEquipmentGuid]=@SwapToGuid
			WHERE [SourceEquipmentGuid] = @SourceEquipmentGuid;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [dbo].[tblTransactionLineItems]
				SET [SourceEquipmentGuid]=NULL
				WHERE [SourceEquipmentGuid] = @SourceEquipmentGuid;
			END
			ELSE
			BEGIN
				UPDATE [dbo].[tblTransactionLineItems]SET [DeleteFlag]=1  WHERE [SourceEquipmentGuid] = @SourceEquipmentGuid; 
			END
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
		RAISERROR(@ErrMessage,18,1)
	END CATCH
END 
