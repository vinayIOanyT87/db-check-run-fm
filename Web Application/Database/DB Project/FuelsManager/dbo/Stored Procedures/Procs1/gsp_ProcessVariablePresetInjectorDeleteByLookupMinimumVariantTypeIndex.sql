CREATE PROCEDURE [dbo].[gsp_ProcessVariablePresetInjectorDeleteByLookupMinimumVariantTypeIndex](@LookupMinimumVariantTypeIndex int,@DetachOnly BIT=NULL,@SwapToIndex int=NULL,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblProcessVariablePresetInjector] WHERE [LookupMinimumVariantTypeIndex] = @LookupMinimumVariantTypeIndex AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR('Attempted to delete a stale version of ProcessVariablePresetInjector.',18,1); 
				 RETURN; 
			END 
		END 
		--
		-- REPLACE LookupMinimumVariantTypeIndex PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT @SwapToIndex IS NULL
		BEGIN
			UPDATE [dbo].[tblProcessVariablePresetInjector]
			SET [LookupMinimumVariantTypeIndex]=@SwapToIndex
			WHERE [LookupMinimumVariantTypeIndex] = @LookupMinimumVariantTypeIndex;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [dbo].[tblProcessVariablePresetInjector]
				SET [LookupMinimumVariantTypeIndex]=NULL
				WHERE [LookupMinimumVariantTypeIndex] = @LookupMinimumVariantTypeIndex;
			END
			ELSE
			BEGIN
				DELETE [dbo].[tblProcessVariablePresetInjector] WHERE [LookupMinimumVariantTypeIndex] = @LookupMinimumVariantTypeIndex; 
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
