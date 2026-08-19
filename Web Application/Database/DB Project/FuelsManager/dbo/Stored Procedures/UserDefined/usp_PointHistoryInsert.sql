CREATE PROCEDURE [dbo].[usp_PointHistoryInsert]
	@PointHistoryGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@StartDate datetimeoffset,
	@IntervalQuantity int,
	@IntervalType int,
	@RangeQuantity int,
	@RangeType int,
	@ColumnsDefinition varchar(max),
	@CreatedDate datetimeoffset,
	@CreatedBy dbo.udtUserID,
	@UpdatedDate datetimeoffset,
	@UpdatedBy dbo.udtUserID,
	@ID nvarchar(1)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		INSERT INTO tblPointHistory
		(
			PointHistoryGuid,
			UserGuid,
			SiteGuid,
			StartDate,
			IntervalQuantity,
			IntervalType,
			RangeQuantity,
			RangeType,
			ColumnsDefinition,
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate
		)
		VALUES
		(
			@PointHistoryGuid,
			@UserGuid,
			@SiteGuid,
			@StartDate,
			@IntervalQuantity,
			@IntervalType,
			@RangeQuantity,
			@RangeType,
			@ColumnsDefinition,
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
						+ 'Procedure Name: usp_PointHistoryInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END