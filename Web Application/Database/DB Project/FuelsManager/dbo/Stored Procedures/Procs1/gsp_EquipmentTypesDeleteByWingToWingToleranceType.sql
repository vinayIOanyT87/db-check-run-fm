CREATE PROCEDURE [dbo].[gsp_EquipmentTypesDeleteByWingToWingToleranceType](@WingToWingToleranceType smallint,@DetachOnly BIT=NULL,@SwapToIndex smallint=NULL,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblEquipmentTypes] WHERE [WingToWingToleranceType] = @WingToWingToleranceType AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR('Attempted to delete a stale version of EquipmentTypes.',18,1); 
				 RETURN; 
			END 
		END 
		--
		-- REPLACE WingToWingToleranceType PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT @SwapToIndex IS NULL
		BEGIN
			UPDATE [dbo].[tblEquipmentTypes]
			SET [WingToWingToleranceType]=@SwapToIndex
			WHERE [WingToWingToleranceType] = @WingToWingToleranceType;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [dbo].[tblEquipmentTypes]
				SET [WingToWingToleranceType]=NULL
				WHERE [WingToWingToleranceType] = @WingToWingToleranceType;
			END
			ELSE
			BEGIN
				UPDATE [dbo].[tblEquipmentTypes]SET [DeleteFlag]=1  WHERE [WingToWingToleranceType] = @WingToWingToleranceType; 
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
