------------------------------------------------------------------------------------------------------
-- Stored Procedure: [dbo].[usp_DeleteNodeToMovementByNodeGuid] 
-- Author: Warren C. Gray
-- Version/Date: 1.0.000 / 2023-04-03
-- Purpose: Delete the node mapping to a Movement.
-- Notes:
-- 1. @NodeGuid: The Node Guid to delete the mapping.
--
-- DECLARE @NodeGuid uniqueidentifier = '2A8766A8-93A3-4F3F-ADC3-EC5357E3054B'
-- EXEC [dbo].[usp_DeleteNodeToMovementByNodeGuid] @NodeGuid
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteNodeToMovementByNodeGuid]
(
	@NodeGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY	
		DECLARE @NodeGuidString VARCHAR(36) = CONVERT(varchar(36),LOWER(@NodeGuid))
		DECLARE @PointGuidTable TABLE (PointGuid UNIQUEIDENTIFIER);

		INSERT INTO @PointGuidTable SELECT PointGuid FROM tblPointProperty
		WHERE ID = 'Movement Settings' AND[Value].exist('/MovementModuleSettings/MovementNodeDataList/MovementNodeData[MovementNodeGuid = sql:variable("@NodeGuidString")]') = 1

		-- Delete the Nodes from Movement Settings.
		UPDATE [dbo].[tblPointProperty] SET Value.modify('delete /MovementModuleSettings/MovementNodeDataList/MovementNodeData[MovementNodeGuid = sql:variable("@NodeGuidString")]') 
		WHERE ID = 'Movement Settings'

		-- Update associated points so they will reload in Point Service and Sync
		UPDATE tblPoint SET UpdatedDate = sysdatetimeoffset() WHERE PointGuid IN (SELECT PointGuid FROM @PointGuidTable)


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
						+ 'Procedure Name: [dbo].usp_DeleteNodeToMovementByNodeGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END


