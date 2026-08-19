CREATE PROCEDURE [dbo].[usp_MovementSummaryInsert]
	@MovementSummaryGuid UNIQUEIDENTIFIER,
	@ID nvarchar(30),
	@Description nvarchar(50),
	@MovementSummaryType int,
	@ColumnsDefinition nvarchar(max),
	@FontSize int,
	@RowsDefinition nvarchar(max),
	@SiteGuid UNIQUEIDENTIFIER,
	@OwnerUserGuid UNIQUEIDENTIFIER,
	@CreatedDate datetimeoffset,
	@CreatedBy dbo.udtUserID,
	@UpdatedDate datetimeoffset,
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		INSERT INTO tblMovementSummary
		(
			MovementSummaryGuid,
			ID,
			[Description],
			[MovementSummaryType],
			ColumnsDefinition,
			FontSize,
			RowsDefinition,
			SiteGuid,
			OwnerUserGuid,
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate
		)
		VALUES
		(
			@MovementSummaryGuid,
			@ID,
			@Description,
			@MovementSummaryType,
			@ColumnsDefinition,
			@FontSize,
			@RowsDefinition,
			@SiteGuid,
			@OwnerUserGuid,
			@CreatedDate,
			@CreatedDate,
			@UpdatedBy,
			@UpdatedDate
		)

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
						+ 'Procedure Name: usp_MovementSummaryInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END