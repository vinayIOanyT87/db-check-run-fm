

/*
	EXEC [dbo].[usp_GetTransactionAliasByGuid] '00000000-0000-0000-0000-000000000001', '6FC6619C-EEF5-436C-8D68-0AB7CC7B0B6E'

*/



CREATE PROCEDURE [dbo].[usp_GetTransactionAliasByGuid]
(
	@TargetSiteGuid uniqueidentifier, @TransactionAliasGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetTransactionAliasByGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the TransactionAlias record that have a given TransactionAlias Guid and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Transactin aliases that have been assigned to this site/sitegroup only
	-- 2. @TransactionAliasGuid: If @TargetSiteGuid is null, then @TransactionAliasGuid is the Guid of the TransactionAlias to retrieve. Otherwise, it is the Guid that is used to retrieve the MasterRecordGuid of the TransactionAlias record to retrieve.
	-- 3. This stored procedure replaces the TransactionAliasClass.SelectSQL inline SQL for the case where the bInTransaction is false.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @masterRecordGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid FROM tblTransactionAliases
		WHERE TransactionAliasGuid = @TransactionAliasGuid
		
		DECLARE @targetRecordGuid uniqueidentifier
		SET @targetRecordGuid = NULL
		IF (@TargetSiteGuid IS NOT NULL)
		BEGIN
			SELECT @targetRecordGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @masterRecordGuid, @TargetSiteGuid)
		END
		ELSE
		BEGIN
			SET @targetRecordGuid = @TransactionAliasGuid
		END
		
		SELECT a.*, b.AliasName AssociatedAlias  
		FROM tblTransactionAliases a
		LEFT OUTER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.AssociatedTransactionAliasGuid
		WHERE a.TransactionAliasGuid = @targetRecordGuid

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
						+ 'Procedure Name: [dbo].usp_GetTransactionAliasByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END