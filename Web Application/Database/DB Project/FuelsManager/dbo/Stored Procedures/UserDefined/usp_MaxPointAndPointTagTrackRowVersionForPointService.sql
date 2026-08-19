CREATE PROCEDURE [dbo].[usp_MaxPointAndPointTagTrackRowVersionForPointService] 
(
	@Hostname nvarchar(256)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_MaxPointAndPointTagTrackRowVersionForPointService] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.0 / 2015-08-25 14:21:10.4470770 -04:00
	-- Purpose: Get Latest max row version for all point and tag changes for a given hostname
	-- Note: Bug in deletes require all FMPointServices to update
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT MAX(RowVersion) AS RowVersion FROM
		(
			SELECT MAX(UpdatedRowVersion) AS RowVersion FROM track.tblPoint p1
			INNER JOIN map.tblPointToPointService m1
			ON  p1.PK_PointGuid = m1.PointGuid
			INNER JOIN tblPointService s
			ON m1.PointServiceGuid = s.PointServiceGuid
			WHERE s.Hostname = @Hostname AND UpdatedRowVersion <  MIN_ACTIVE_ROWVERSION()
			UNION 
			SELECT MAX(InsertedRowVersion) AS RowVersion FROM track.tblPoint p1
			INNER JOIN map.tblPointToPointService m1
			ON  p1.PK_PointGuid = m1.PointGuid
			INNER JOIN tblPointService s
			ON m1.PointServiceGuid = s.PointServiceGuid
			WHERE s.Hostname = @Hostname AND InsertedRowVersion <  MIN_ACTIVE_ROWVERSION()
			UNION 
			SELECT MAX(UpdatedRowVersion) AS RowVersion FROM track.tblPointTag tt
			INNER JOIN tblPointTag t
			ON tt.PK_PointTagGuid = t.PointTagGuid
			INNER JOIN map.tblPointToPointService m1
			ON  t.PointGuid = m1.PointGuid
			INNER JOIN tblPointService s
			ON m1.PointServiceGuid = s.PointServiceGuid
			WHERE s.Hostname = @Hostname AND UpdatedRowVersion <  MIN_ACTIVE_ROWVERSION()
			UNION 
			SELECT MAX(InsertedRowVersion) AS RowVersion FROM track.tblPointTag tt
			INNER JOIN tblPointTag t
			ON tt.PK_PointTagGuid = t.PointTagGuid
			INNER JOIN map.tblPointToPointService m1
			ON  t.PointGuid = m1.PointGuid
			INNER JOIN tblPointService s
			ON m1.PointServiceGuid = s.PointServiceGuid
			WHERE s.Hostname = @Hostname AND InsertedRowVersion <  MIN_ACTIVE_ROWVERSION()
		) RowVersions
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
						+ 'Procedure Name: [dbo].usp_MaxPointAndPointTagTrackRowVersionForPointService' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END