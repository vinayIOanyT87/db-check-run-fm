

CREATE PROCEDURE [sync].[usp_SyncTableSelectForSyncDependencyGroupKey](
	@SyncDependencyGroupGuid uniqueidentifier = NULL
	,@SyncDependencyGroupID nvarchar(80) = NULL
)
AS
BEGIN
	BEGIN TRY
		IF (@SyncDependencyGroupGuid IS NOT NULL OR @SyncDependencyGroupID IS NOT NULL)
		BEGIN
			SELECT st.SyncTableGuid
					,st.TableName
					,st.SyncDependencyGroupGuid
					,st.LastSchemaDate
					,st.IsSiteFilteredFlag
					,st.IsSiteFilteredOnDeleteFlag
					,ParentSyncTableGuid
					,ParentForeignKeyColumnName
					,st.CreatedDate
					,st.CreatedBy
					,st.UpdatedDate
					,st.UpdatedBy
					,st._RowVersion
					FROM sync.tblSyncTable st WITH (NOLOCK)
						INNER JOIN sync.tblSyncDependencyGroup sdg WITH (NOLOCK)
							ON st.SyncDependencyGroupGuid = sdg.SyncDependencyGroupGuid
					WHERE (@SyncDependencyGroupGuid IS NULL OR st.SyncDependencyGroupGuid = @SyncDependencyGroupGuid)
						AND (@SyncDependencyGroupID IS NULL OR sdg.ID = @SyncDependencyGroupID)
					ORDER BY st.TableName
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
						+ 'Procedure Name: usp_SyncTableSelectForSyncDependencyGroupKey' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END