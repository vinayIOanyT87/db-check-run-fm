CREATE PROCEDURE [dbo].[usp_PointAccessGroupToAlarmTestFullUpdate]
(
	@PointAccessGroupGuid UNIQUEIDENTIFIER,
	@PointAccessGroupToAlarmTestTempTable [PointAccessGroupToAlarmTestDataType] READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
	SELECT 1


	MERGE [map].[tblPointAccessGroupToAlarmTest] ptas
	USING @PointAccessGroupToAlarmTestTempTable temp        
	ON temp.[PointAccessGroupGuid] = ptas.[PointAccessGroupGuid]
	AND temp.AlarmTestTemplateGuid = ptas.AlarmTestGuid

	WHEN MATCHED
    THEN UPDATE SET ptas.[View] = temp.[View],   
                    ptas.[Acknowledge] = temp.[Acknowledge],
					ptas.UpdatedDate = sysdatetimeoffset(),
					ptas.UpdatedBy = temp.UpdatedBy					  

    WHEN NOT MATCHED BY TARGET THEN
    INSERT (PointAccessGroupToAlarmTestGuid,
			PointAccessGroupGuid,
			AlarmTestGuid,
			[View],
			[Acknowledge],
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
	)
     VALUES
           (temp.[PointAccessGroupToAlarmTestGuid]
		   ,temp.[PointAccessGroupGuid]
			,temp.AlarmTestTemplateGuid
			,temp.[View]
			,temp.[Acknowledge]
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
						+ 'Procedure Name: usp_PointAccessGroupToAlarmTestFullUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END