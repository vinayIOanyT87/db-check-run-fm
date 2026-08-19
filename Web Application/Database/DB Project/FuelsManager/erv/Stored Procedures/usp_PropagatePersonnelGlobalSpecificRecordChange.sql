/*
	DROP PROCEDURE [erv].[usp_PropagatePersonnelGlobalSpecificRecordChange]

	EXEC [erv].[usp_PropagatePersonnelGlobalSpecificRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagatePersonnelGlobalSpecificRecordChange] '0DC68ACA-11AD-4F43-AD2B-87609738C453'
*/

CREATE PROCEDURE [erv].[usp_PropagatePersonnelGlobalSpecificRecordChange]
(
	@SourceEntityGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagatePersonnelGlobalSpecificRecordChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate the changes made to the GlobalFields of a Personnel entity record to the all the record versions of that entity record.
	-- Notes:
	-- 1. @SourceEntityGuid: Guid of the Personnel record whose GlobalFields needs to be propagated throughout the site hierarchy. 
	--    This should correspond to the exact record version that has been changed (and not the parent record of the entity record).
	-- 2. GlobalSpecific change propagation takes place from the Master Record down the site hierarchy.
	-- 3. If @SourcePersonnelGuid corresponds to a child record version, its GlobalSpecific fields are first replicated onto the master
	--    record, before propagating the non-VersionSpecific changes down the site hierarchy from the Master Record.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @masterRecordGuid = _MasterRecordGuid FROM dbo.tblPersonnel
		WHERE PersonnelGuid = @SourceEntityGuid

		IF (@masterRecordGuid IS NULL)
		BEGIN
			-- Cannot locate the source master record for data propagation. Record must have been deleted. No propagation required.
			RETURN;
		END

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateGlobalSpecificChanges
            SET @BeginTran = 1   
		END  

		IF ((@masterRecordGuid <> @SourceEntityGuid))
		BEGIN
			EXEC [erv].[usp_ReplicatePersonnelGSChangesOnMaster] @SourceEntityGuid
		END

		EXEC [erv].[usp_PropagatePersonnelRevisionByEntityRecordChange] @masterRecordGuid

	
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --PropagateGlobalSpecificChanges
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --PropagateGlobalSpecificChanges
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
						+ 'Procedure Name: [erv].usp_PropagatePersonnelGlobalSpecificRecordChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
