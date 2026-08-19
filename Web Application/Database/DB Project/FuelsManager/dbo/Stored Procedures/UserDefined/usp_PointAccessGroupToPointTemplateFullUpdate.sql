CREATE PROCEDURE [dbo].[usp_PointAccessGroupToPointTemplateFullUpdate]
(
	@PointAccessGroupGuid UNIQUEIDENTIFIER,
	@PointAccessGroupToPointTemplateTempTable [PointAccessGroupToPointTemplateDataType] READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

	MERGE map.tblPointAccessGroupToPointTemplate ptas
	USING @PointAccessGroupToPointTemplateTempTable    temp        
	ON temp.[PointAccessGroupGuid] = ptas.[PointAccessGroupGuid]
	AND temp.[PointAccessGroupToPointTemplateGuid] = ptas.[PointAccessGroupToPointTemplateGuid]
	AND temp.[PointTemplateGuid] = ptas.[PointTemplateGuid]           

    WHEN NOT MATCHED BY TARGET THEN
    INSERT (PointAccessGroupToPointTemplateGuid,
			PointAccessGroupGuid,
			PointTemplateGuid,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
	)
     VALUES
           (temp.PointAccessGroupToPointTemplateGuid
		   ,temp.PointAccessGroupGuid
			,temp.PointTemplateGuid
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
						+ 'Procedure Name: usp_PointAccessGroupToPointTemplateAddDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END