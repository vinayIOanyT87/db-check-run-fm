CREATE PROCEDURE [dbo].[usp_MovementSummaryUpdateByPK]
	@MovementSummaryGuid UNIQUEIDENTIFIER,
	@ID nvarchar(30),
	@Description nvarchar(50),
	@MovementSummaryType int,
	@ColumnsDefinition nvarchar(max),
	@FontSize int,
	@RowsDefinition nvarchar(max),
	@OwnerUserGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@UpdatedDate datetimeoffset,
	@UpdatedBy dbo.udtUserID,
	@RowVersion timestamp
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF @RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblMovementSummary] WHERE MovementSummaryGuid = @MovementSummaryGuid AND _RowVersion = @RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END


		UPDATE tblMovementSummary WITH (UPDLOCK)
		SET ID = @ID,
			[Description] = @Description,
			MovementSummaryType = @MovementSummaryType,
			ColumnsDefinition = @ColumnsDefinition,
			FontSize = @FontSize,
			RowsDefinition = @RowsDefinition,
			OwnerUserGuid = @OwnerUserGuid,
			UpdatedBy = @UpdatedBy,
			UpdatedDate = @UpdatedDate,
			SiteGuid = @SiteGuid
			WHERE MovementSummaryGuid = @MovementSummaryGuid

			SELECT _RowVersion AS Row_Version FROM tblMovementSummary WHERE MovementSummaryGuid = @MovementSummaryGuid

	END TRY
	BEGIN CATCH
		DECLARE	@_ErrMessage NVARCHAR(2048)      
			, @_ErrNumber INT           
			, @_ErrProcName NVARCHAR(126)           
			, @_ErrLineNumber INT;      
				      
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: usp_MovementSummaryUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
GO
