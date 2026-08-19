CREATE PROCEDURE [dbo].[usp_PointTagDeleteByPointGuid](@PointGuid uniqueidentifier,@DetachOnly BIT=NULL,@SwapToGuid uniqueidentifier=NULL,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblPointTag] WHERE [PointGuid] = @PointGuid AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR('Attempted to delete a stale version of PointTag.',18,1); 
				 RETURN; 
			END 
		END


		DECLARE @Result NVARCHAR(100) = [dbo].[udf_CheckIsPointInUseBySystem](@PointGuid)
		IF @Result IS NOT NULL
		BEGIN
			RAISERROR(@Result,16,1);
			RETURN
		END
 
		--
		-- REPLACE PointGuid PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT @SwapToGuid IS NULL
		BEGIN
			UPDATE [dbo].[tblPointTag]
			SET [PointGuid]=@SwapToGuid
			WHERE [PointGuid] = @PointGuid;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [dbo].[tblPointTag]
				SET [PointGuid]=NULL
				WHERE [PointGuid] = @PointGuid;
			END
			ELSE
			BEGIN
				DELETE pagtpt FROM [map].[tblPointAccessGroupToPointTag] pagtpt
				INNER JOIN [tblPointTag] pt ON pt.PointTagGuid = pagtpt.TagGuid
				WHERE pt.[PointGuid] = @PointGuid

				DELETE [dbo].[tblPointTag] WHERE [PointGuid] = @PointGuid; 
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