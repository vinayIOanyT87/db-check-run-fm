CREATE PROCEDURE [dbo].[usp_PointAccessGroupToExposedSettingFullUpdate]
(
	@PointAccessGroupGuid UNIQUEIDENTIFIER,
	@PointAccessGroupToExposedSettingTempTable [PointAccessGroupToExposedSettingDataType] READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
	SELECT 1


	MERGE [map].[tblPointAccessGroupToExposedPropertySetting] ptas
	USING @PointAccessGroupToExposedSettingTempTable temp        
	ON temp.[PointAccessGroupGuid] = ptas.[PointAccessGroupGuid]
	AND temp.[ExposedSettingGuid] = ptas.[PointSettingGuid]
	AND temp.[PropertyID] = ptas.[PropertyID]
	AND temp.[PointAccessGroupToExposedSettingGuid] = ptas.[PointAccessGroupToExposedSettingGuid]
	AND temp.[ValueType] = 1

	WHEN MATCHED
    THEN UPDATE SET ptas.[View] = temp.[View],   
                    ptas.[Modify] = temp.[Modify],
					ptas.UpdatedDate = sysdatetimeoffset(),
					ptas.UpdatedBy = temp.UpdatedBy					  

    WHEN NOT MATCHED BY TARGET AND temp.[ValueType] = 1 THEN
    INSERT (PointAccessGroupToExposedSettingGuid,
			PointAccessGroupGuid,
			PointSettingGuid,
			PropertyID,
			[View],
			[Modify],
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
	)
     VALUES
           (temp.[PointAccessGroupToExposedSettingGuid]
		   ,temp.[PointAccessGroupGuid]
			,temp.[ExposedSettingGuid]
			,temp.[PropertyID]
			,temp.[View]
			,temp.[Modify]
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy)

    WHEN NOT MATCHED BY SOURCE AND ptas.[PointAccessGroupGuid] = @PointAccessGroupGuid
        THEN DELETE;

	MERGE [map].[tblPointAccessGroupToExposedPointSetting] ptas
	USING @PointAccessGroupToExposedSettingTempTable temp        
	ON temp.[PointAccessGroupGuid] = ptas.[PointAccessGroupGuid]
	AND temp.[ExposedSettingGuid] = ptas.[PointSettingGuid]
	AND temp.[PropertyID] = ptas.[PropertyID]
	AND temp.[PointAccessGroupToExposedSettingGuid] = ptas.[PointAccessGroupToExposedSettingGuid]
	AND temp.[ValueType] = 2

	WHEN MATCHED
    THEN UPDATE SET ptas.[View] = temp.[View],   
                    ptas.[Modify] = temp.[Modify],
					ptas.UpdatedDate = sysdatetimeoffset(),
					ptas.UpdatedBy = temp.UpdatedBy					  

    WHEN NOT MATCHED BY TARGET AND temp.[ValueType] = 2 THEN
    INSERT (PointAccessGroupToExposedSettingGuid,
			PointAccessGroupGuid,
			PointSettingGuid,
			PropertyID,
			[View],
			[Modify],
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
	)
     VALUES
           (temp.[PointAccessGroupToExposedSettingGuid]
		   ,temp.[PointAccessGroupGuid]
			,temp.[ExposedSettingGuid]
			,temp.[PropertyID]
			,temp.[View]
			,temp.[Modify]
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
						+ 'Procedure Name: usp_PointAccessGroupToExposedSettingFullUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END