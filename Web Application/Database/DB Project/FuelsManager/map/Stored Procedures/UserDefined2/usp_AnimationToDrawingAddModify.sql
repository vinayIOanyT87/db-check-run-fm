CREATE PROCEDURE [map].[usp_AnimationToDrawingAddModify]
(
	@MapAnimationToDrawingTempTable MapAnimationToDrawingDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT,
	@EnableDelete BIT = 0
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE map.tblAnimationToDrawing at
USING @MapAnimationToDrawingTempTable    temp        
		ON temp.AnimationGuid = at.AnimationGuid
		AND temp.DrawingGuid = at.DrawingGuid               			 							                        
WHEN MATCHED AND @EnableModify = 1 THEN
    UPDATE SET	AnimationGuid = temp.AnimationGuid  
				,DrawingGuid = temp.DrawingGuid  
				,UpdatedBy = temp.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 

WHEN NOT MATCHED BY TARGET AND @EnableAdd = 1 THEN
    INSERT 
	 (
			[AnimationToDrawingGuid],
			[AnimationGuid],
			[DrawingGuid],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy]
	)
	VALUES
	(
		temp.AnimationToDrawingGuid
		,temp.AnimationGuid
		,temp.DrawingGuid
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
	);

	-- perform a delete of all map records for the animations listed where the animation map row is not in the list
	IF ( SELECT COUNT(*) FROM @MapAnimationToDrawingTempTable) > 0
	BEGIN 
		DELETE ad
		FROM map.tblAnimationToDrawing ad
		WHERE ad.DrawingGuid IN ( SELECT DrawingGuid FROM @MapAnimationToDrawingTempTable )
		AND NOT EXISTS ( SELECT 1 FROM @MapAnimationToDrawingTempTable t WHERE t.AnimationGuid = ad.AnimationGuid AND t.DrawingGuid = ad.DrawingGuid )
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
						+ 'Procedure Name: usp_AnimationToDrawingAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END