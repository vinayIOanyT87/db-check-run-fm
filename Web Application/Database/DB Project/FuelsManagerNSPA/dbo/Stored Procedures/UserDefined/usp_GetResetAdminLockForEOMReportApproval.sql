CREATE PROCEDURE [dbo].[usp_GetResetAdminLockForEOMReportApproval] 
(
	@SiteGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetResetAdminLockForEOMReportApproval] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
	-- Notes:
	-- 1. @SiteGuid: Identifies the site the report is being run from
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		Declare @CurrentTime date
	SET @CurrentTime = GETDATE()
	Declare @ProductsNoCloseout date
	SET @ProductsNoCloseout = ISNULL(
		(SELECT TOP 1 prods.CreatedDate FROM
		(
			SELECT DISTINCT p.ProductGuid, p.CreatedDate FROM tblProducts p
			INNER JOIN tblTransactionLineItems l
			ON p.ProductGuid = l.ProductGuid
			INNER JOIN tblTransactions t
			ON l.TransactionGuid = t.TransactionGuid
			WHERE t.SiteGuid = @SiteGuid
			UNION
			SELECT DISTINCT p.ProductGuid, p.CreatedDate FROM tblProducts p
			INNER JOIN tblTransactionSubLineItems l
			ON p.ProductGuid = l.ProductGuid
			INNER JOIN tblTransactions t
			ON l.TransactionGuid = t.TransactionGuid
			WHERE t.SiteGuid = @SiteGuid
		) prods 
		WHERE NOT EXISTS 
		(
			SELECT * FROM tblCloseoutInventory 
			WHERE ProductGuid = prods.ProductGuid)
			ORDER BY prods.CreatedDate ASC
		),@CurrentTime)

	Declare @ExistingCloseout date
	SET @ExistingCloseout = ISNULL((Select TOP 1 a.Closeout FROM (select MAX(CloseoutDate) AS Closeout, ProductGuid from dbo.tblCloseoutInventory WHERE SiteGuid = @SiteGuid Group By ProductGuid)a ORDER BY a.Closeout ASC),@CurrentTime)

	Declare @Result date
	SET @Result = IIF ( @ProductsNoCloseout > @ExistingCloseout,@ProductsNoCloseout,@ExistingCloseout )

	SET @Result = IIF ( @Result = @CurrentTime,(SELECT CreatedDate FROM tblSites WHERE SiteGuid = @SiteGuid),@Result)

	SELECT @Result AS AdminLockResetdate
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
						+ 'Procedure Name: [dbo].[usp_GetResetAdminLockForEOMReportApproval] ' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END     