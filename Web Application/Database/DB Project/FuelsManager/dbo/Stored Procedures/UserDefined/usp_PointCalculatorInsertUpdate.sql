CREATE PROCEDURE [dbo].[usp_PointCalculatorInsertUpdate] 
(
	@CalculatorRunHeader dbo.PointCalculatorRunDataType READONLY,
	@CalculatorRunDetails dbo.PointCalculatorRunDetailsDataType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	-- Select an existing RunId for an update or create a new one
	DECLARE @RunId uniqueidentifier = ISNULL((SELECT PointCalculatorRunId FROM tblPointCalculatorRuns WHERE Token = (SELECT Token FROM @CalculatorRunHeader)), NEWID())
	
	BEGIN TRY

		MERGE tblPointCalculatorRuns AS target
		USING (
			SELECT 
			SiteId,
			PointId,
			CalculationMode,
			UserId,
			SiteGuid,
			PointGuid,
			UserGuid,
			Token
			FROM @CalculatorRunHeader
			) AS source
		ON source.SiteGuid = target.SiteGuid
			AND source.UserGuid = target.UserGuid
			AND source.Token = target.Token
		WHEN MATCHED THEN UPDATE SET
			CalculationMode = source.CalculationMode,
			PointId = source.PointId,
			PointGuid = source.PointGuid
		WHEN NOT MATCHED THEN INSERT 
		(
			PointCalculatorRunId,
			SiteId,
			PointId,
			CalculationMode,
			UserId,
			SiteGuid,
			PointGuid,
			UserGuid,
			Token
		)
		VALUES
		(
			@RunId,
			source.SiteId,
			source.PointId,
			source.CalculationMode,
			source.UserId,
			source.SiteGuid,
			source.PointGuid,
			source.UserGuid,
			source.Token
		);

		------------------
		--- Details Table
		------------------
		MERGE tblPointCalculatorRunDetails AS target
		USING (
			SELECT 
			TagName,
			Units,
			Acronym,
			BeginValue,
			EndValue,
			DiffValue,
			DisplayOrder
			FROM @CalculatorRunDetails
			) AS source
		ON @RunId = target.PointCalculatorRunId
			AND source.TagName = target.TagName
		WHEN MATCHED THEN UPDATE SET
			Units = source.Units,
			BeginValue = source.BeginValue,
			EndValue = source.EndValue,
			DiffValue = source.DiffValue
		WHEN NOT MATCHED BY TARGET THEN INSERT
		(
			PointCalculatorRunId,
			TagName,
			Units,
			Acronym,
			BeginValue,
			EndValue,
			DiffValue,
			DisplayOrder
		)
		VALUES
		(
			@RunId,
			source.TagName,
			source.Units,
			source.Acronym,
			source.BeginValue,
			source.EndValue,
			source.DiffValue,
			source.DisplayOrder
		)
		WHEN NOT MATCHED BY SOURCE AND target.PointCalculatorRunId = @RunId THEN DELETE;

		-- Return the RunId
		SELECT @RunId

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
		         
		BEGIN     
			SET @_ErrProcName= ERROR_PROCEDURE();        
			SET @_ErrLineNumber = ERROR_LINE();            
			SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
							+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
							+ 'Procedure Name: dbo.usp_PointCalculatorInsertUpdate' + CHAR(13)+CHAR(10)                  
							+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
			RAISERROR(@_ErrMessage, 16, 1);
		END      
	END CATCH    
END