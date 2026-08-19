
CREATE PROCEDURE [sync].[usp_SyncDependencyGroupSelect](
	@IdentityGuid uniqueidentifier = NULL
	,@ID nvarchar(80) = NULL
)
AS
BEGIN
	BEGIN TRY
		SELECT SyncDependencyGroupGuid
				,ID
				,FriendlyName
				,LongDescription
				,DependencyLevel
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy
				,_RowVersion
				FROM sync.tblSyncDependencyGroup  WITH (NOLOCK)
				WHERE (@IdentityGuid IS NULL OR SyncDependencyGroupGuid = @IdentityGuid)
					AND (@ID IS NULL OR ID = @ID)
				ORDER BY DependencyLevel
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
						+ 'Procedure Name: usp_SyncDependencyGroupSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END