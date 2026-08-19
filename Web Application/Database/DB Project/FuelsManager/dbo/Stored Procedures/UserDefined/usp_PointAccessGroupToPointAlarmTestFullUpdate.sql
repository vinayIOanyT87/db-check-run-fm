CREATE PROCEDURE [dbo].[usp_PointAccessGroupToPointAlarmTestFullUpdate]
(
	@PointAccessGroupGuid UNIQUEIDENTIFIER,
	@PointAccessGroupToPointAlarmTestTempTable [PointAccessGroupToPointAlarmTestDataType] READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
	SELECT 1


	MERGE [map].[tblPointAccessGroupToPointAlarmTest] ptpat
	USING @PointAccessGroupToPointAlarmTestTempTable temp        
	ON temp.[PointAccessGroupGuid] = ptpat.[PointAccessGroupGuid]
	AND temp.AlarmTestGuid = ptpat.AlarmTestGuid

	WHEN MATCHED
    THEN UPDATE SET ptpat.[View] = temp.[View],   
                    ptpat.[Acknowledge] = temp.[Acknowledge],
										ptpat.UpdatedDate = sysdatetimeoffset(),
										ptpat.UpdatedBy = temp.UpdatedBy					  

    WHEN NOT MATCHED BY TARGET THEN
    INSERT (PointAccessGroupToPointAlarmTestGuid,
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
       (temp.[PointAccessGroupToPointAlarmTestGuid]
		   ,temp.[PointAccessGroupGuid]
			 ,temp.AlarmTestGuid
			 ,temp.[View]
			 ,temp.[Acknowledge]
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
						+ 'Procedure Name: usp_PointAccessGroupToPointAlarmTestFullUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END