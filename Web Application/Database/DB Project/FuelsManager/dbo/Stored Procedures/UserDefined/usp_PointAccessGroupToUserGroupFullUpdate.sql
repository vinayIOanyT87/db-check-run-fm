CREATE PROCEDURE [dbo].[usp_PointAccessGroupToUserGroupFullUpdate]
(
	@PointAccessGroupGuid UNIQUEIDENTIFIER,
	@PointAccessGroupToUserGroupTempTable [PointAccessGroupToUserGroupDataType] READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

	MERGE map.tblPointAccessGroupToUserGroup ptas
	USING @PointAccessGroupToUserGroupTempTable temp        
	ON temp.[PointAccessGroupGuid] = ptas.[PointAccessGroupGuid]
	AND temp.[PointAccessGroupToUserGroupGuid] = ptas.[PointAccessGroupToUserGroupGuid]
	AND temp.[UserGroupGuid] = ptas.[UserGroupGuid]

    WHEN NOT MATCHED BY TARGET THEN
    INSERT ([PointAccessGroupToUserGroupGuid],
			PointAccessGroupGuid,
			[UserGroupGuid],
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
	)
     VALUES
           (temp.[PointAccessGroupToUserGroupGuid]
		   ,temp.PointAccessGroupGuid
			,temp.[UserGroupGuid]
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy)

    WHEN NOT MATCHED BY SOURCE AND ptas.[PointAccessGroupGuid] = @PointAccessGroupGuid
        THEN DELETE;

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
						+ 'Procedure Name: usp_PointAccessGroupToUserGroupFullUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END