CREATE PROCEDURE [dbo].[usp_OwnerCloseoutInsert]
(
	@OwnerCloseouts dbo.OwnerCloseoutType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		------------------------------------------------------------------------------------------------------
		-- Stored procedure: usp_OwnerCloseoutInsert
		-- Author: Ryan Hill
		-- Purpose: Insert owner closeout records in bulk
		------------------------------------------------------------------------------------------------------

		INSERT INTO tblOwnerCloseout
		(
			[Site],
			ManagerName,
			OwnerName,
			ProductName,
			CloseoutDate,
			SiteGuid,
			ManagerCompanyGuid,
			OwnerCompanyGuid,
			ProductGuid,
			GrossBookInventory,
			NetBookInventory,
			MassBookInventory,
			GrossBookPrice,
			NetBookPrice,
			MassBookPrice,   
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		)
		SELECT 
			[Site],
			ManagerName,
			OwnerName,
			ProductName,
			CloseoutDate,
			SiteGuid,
			ManagerCompanyGuid,
			OwnerCompanyGuid,
			ProductGuid,
			GrossBookInventory,
			NetBookInventory,
			MassBookInventory,
			GrossBookPrice,
			NetBookPrice,		
			MassBookPrice,    
			SYSDATETIMEOFFSET(),
			CreatedBy,
			SYSDATETIMEOFFSET(),
			CreatedBy
		FROM @OwnerCloseouts ownerCloseouts

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
						+ 'Procedure Name: usp_OwnerCloseoutInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
