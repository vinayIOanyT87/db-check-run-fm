CREATE PROCEDURE [dbo].[usp_MovementHistoryGetMovementsByInitialLoadRequest]
(
	@SiteGuid UNIQUEIDENTIFIER
	, @InitialLoadCount INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @RecordTypeAG INT
	DECLARE @RecordTypeHG INT
	DECLARE @MidnightRecord BIT

	SET @RecordTypeAG = 1
	SET @RecordTypeHG = 2
	SET @MidnightRecord = 1

	BEGIN TRY
		-- We never want to show midnight records in the history
		-- Midnight records have a status that is anything but
		-- Inactive or Complete.
		SELECT TOP(@InitialLoadCount) * 
		FROM tblMovementHistory
		WHERE SiteGuid = @SiteGuid 
			  AND RecordType <> @RecordTypeAG 
			  AND RecordType <> @RecordTypeHG
			  AND (MidnightRecord <> @MidnightRecord OR MidnightRecord IS NULL)
		ORDER BY [TimeStamp] DESC, [Name], [InitiationCount], RecordSeq, TransferDirection DESC
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
						+ 'Procedure Name: dbo.usp_MovementHistoryGetMovementsByInitialLoadRequest' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
