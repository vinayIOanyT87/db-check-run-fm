CREATE PROCEDURE [dbo].[usp_PointAccessGroupToPointTagFullUpdate]
(
	@PointAccessGroupGuid UNIQUEIDENTIFIER,
	@PointAccessGroupToPointTagTempTable [PointAccessGroupToPointTagDataType] READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
	SELECT 1


	MERGE [map].[tblPointAccessGroupToPointTag] ptpat
	USING @PointAccessGroupToPointTagTempTable temp        
	ON temp.[PointAccessGroupGuid] = ptpat.[PointAccessGroupGuid]
	AND temp.TagGuid = ptpat.TagGuid

	WHEN MATCHED
    THEN UPDATE SET	ptpat.[View] = temp.[View],   
							ptpat.[Modify] = temp.[Modify],
							ptpat.[ExceedRange] = temp.[ExceedRange],
							ptpat.[Override] = temp.[Override],
							ptpat.UpdatedDate = sysdatetimeoffset(),
							ptpat.UpdatedBy = temp.UpdatedBy					  

    WHEN NOT MATCHED BY TARGET THEN
    INSERT (PointAccessGroupToPointTagGuid,
			PointAccessGroupGuid,
			TagGuid,
			[View],
			[Modify],
			[ExceedRange],
			[Override],
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
	  )
     VALUES
     (
			temp.[PointAccessGroupToPointTagGuid]
			,temp.[PointAccessGroupGuid]
			,temp.TagGuid
			,temp.[View]
			,temp.[Modify]
			,temp.[ExceedRange]
			,temp.[Override]
			,sysdatetimeoffset()
			,temp.UpdatedBy
			,sysdatetimeoffset()
			,temp.UpdatedBy)

    WHEN NOT MATCHED BY SOURCE AND ptpat.[PointAccessGroupGuid] = @PointAccessGroupGuid
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
						+ 'Procedure Name: usp_PointAccessGroupToPointTagFullUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END