CREATE PROCEDURE [dbo].[usp_MobileFlightSelctionSelectIssueTransactions] (
	@OperatorID 		NVARCHAR(100),
	@filterOperatorID 	BIT,
	@VehicleID 		NVARCHAR(100),
	@filterVehicleID 	BIT,
	@GateID 		NVARCHAR(100),
	@filterGateID 		BIT,
	@HoursInPast 		INT,
	@HoursInFuture 		INT
) AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_MobileFlightSelctionSelectIssueTransactions]
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.001 / 2012-10-30 
	-- Purpose: Select Issue transactions based on some filter criteria
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	SELECT 	tblTransactions.RoutingID as Flight,tblTransactionLineItems.LoadingLocationID as Gate,
			tblTransactions.ETA,tblTransactions.ETD,tblTransactions.LoadID as Load			 
	FROM tblTransactions INNER JOIN tblTransactionLineItems 
	ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid 
	WHERE AliasName = 'Issue' 
		AND (@filterVehicleID = 0 OR tblTransactions.SourceRegistrationID1 = @VehicleID) 
		AND (@filterGateID = 0 OR tblTransactionLineItems.LoadingLocationID = @GateID) 
		AND (@filterOperatorID = 0 OR tblTransactions.OperatorID = @OperatorID)
		AND (DATEADD(hour,@HoursInFuture,SYSDATETIMEOFFSET()) > tblTransactions.ETD)
		AND (SYSDATETIMEOFFSET() < DATEADD(hour,@HoursInPast,tblTransactions.ETD))
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
						+ 'Procedure Name: gsp_AdditiveProfilesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END