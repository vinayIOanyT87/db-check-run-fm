
CREATE PROCEDURE [sync].[usp_SchemaChangeDetailSelectForSchemaChangeHistory](
    @SchemaChangeHistoryGuid uniqueidentifier = NULL
)
AS
BEGIN
	BEGIN TRY
        SELECT SchemaChangeDetailGuid
				,SchemaChangeHistoryGuid
                ,SchemaObjectTypeIndex
                ,SchemaName
                ,ObjectName
                ,CreatedDate
                ,CreatedBy
                ,UpdatedDate
                ,UpdatedBy
				,_RowVersion
    		FROM [sync].[tblSchemaChangeDetail] WITH (NOLOCK)
    		WHERE @SchemaChangeHistoryGuid IS NOT NULL AND SchemaChangeHistoryGuid = @SchemaChangeHistoryGuid
			ORDER BY CreatedDate
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
						+ 'Procedure Name: usp_SchemaChangeDetailSelectForSchemaChangeHistory' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
END