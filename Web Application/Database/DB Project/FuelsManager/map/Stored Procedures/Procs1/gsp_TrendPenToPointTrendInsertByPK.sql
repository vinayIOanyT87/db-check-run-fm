CREATE PROCEDURE [map].[gsp_TrendPenToPointTrendInsertByPK]
		@TrendPenToPointTrendGuid uniqueidentifier=NULL OUTPUT
	,	@PointTagGuid uniqueidentifier=NULL
	,	@TrendGuid uniqueidentifier=NULL
	,	@PenColor nvarchar(30)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_TrendPenToPointTrendInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-12-20 08:24:34.8048433 -05:00
	-- Purpose: Insert into table [map].[tblTrendPenToPointTrend]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		IF @TrendPenToPointTrendGuid = '00000000-0000-0000-0000-000000000000'
        BEGIN 
			SET @TrendPenToPointTrendGuid=NEWID();
		END
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblTrendPenToPointTrend] 
		(
			[TrendPenToPointTrendGuid]
		,	[PointTagGuid]
		,	[TrendGuid]
		,	[PenColor]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@TrendPenToPointTrendGuid
		,	@PointTagGuid
		,	@TrendGuid
		,	@PenColor
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblTrendPenToPointTrend]           
		WHERE TrendPenToPointTrendGuid=@TrendPenToPointTrendGuid;
	
 
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
						+ 'Procedure Name: gsp_TrendPenToPointTrendInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END