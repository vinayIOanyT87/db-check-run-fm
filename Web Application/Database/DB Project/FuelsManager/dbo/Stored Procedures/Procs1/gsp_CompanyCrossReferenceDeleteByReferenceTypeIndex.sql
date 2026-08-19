CREATE PROCEDURE [dbo].[gsp_CompanyCrossReferenceDeleteByReferenceTypeIndex](@ReferenceTypeIndex int,@DetachOnly BIT=NULL,@SwapToIndex int=NULL,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblCompanyCrossReference] WHERE [ReferenceTypeIndex] = @ReferenceTypeIndex AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR('Attempted to delete a stale version of CompanyCrossReference.',18,1); 
				 RETURN; 
			END 
		END 
		--
		-- REPLACE ReferenceTypeIndex PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT @SwapToIndex IS NULL
		BEGIN
			UPDATE [dbo].[tblCompanyCrossReference]
			SET [ReferenceTypeIndex]=@SwapToIndex
			WHERE [ReferenceTypeIndex] = @ReferenceTypeIndex;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [dbo].[tblCompanyCrossReference]
				SET [ReferenceTypeIndex]=NULL
				WHERE [ReferenceTypeIndex] = @ReferenceTypeIndex;
			END
			ELSE
			BEGIN
				DELETE [dbo].[tblCompanyCrossReference] WHERE [ReferenceTypeIndex] = @ReferenceTypeIndex; 
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
