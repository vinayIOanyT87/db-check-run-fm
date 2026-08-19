CREATE PROCEDURE [dbo].[usp_PointPropertyDataUpdate]
(
	@PointPropertyGuid uniqueidentifier,
	@Value xml,
	@UpdatedBy nvarchar(30),
	@UpdatedDate datetimeoffset,
	@BypassUpdatePointRowVersion bit,
	@BypassIsPointInSystemUse bit
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
		DECLARE @PointGuid uniqueidentifier = (SELECT PointGuid FROM [dbo].[tblPointProperty] WHERE PointPropertyGuid = @PointPropertyGuid)

		IF @BypassIsPointInSystemUse IS NOT NULL AND @BypassIsPointInSystemUse = 0
		BEGIN
			DECLARE @Result NVARCHAR(100) = [dbo].[udf_CheckIsPointInUseBySystem](@PointGuid)
			IF @Result IS NOT NULL
			BEGIN
				RAISERROR(@Result,16,1);
				RETURN;
			END
		END


		-- perform the table update
		UPDATE [dbo].[tblPointProperty] SET [Value] = @Value, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate WHERE PointPropertyGuid = @PointPropertyGuid

		IF @BypassUpdatePointRowVersion = 0
		BEGIN
			UPDATE [dbo].[tblPoint]  SET [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate WHERE PointGuid = @PointGuid
		END


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
						+ 'Procedure Name: usp_PointPropertyDataUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 

