CREATE PROCEDURE [map].[gsp_TrendPenToDetailTrendInsertByPK]
		@TrendPenToDetailTrendGuid uniqueidentifier=NULL OUTPUT
	,	@PointTemplateTagGuid uniqueidentifier=NULL
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
	-- Stored procedure: [map].[gsp_TrendPenToDetailTrendInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-12-20 08:24:34.8048433 -05:00
	-- Purpose: Insert into table [map].[tblTrendPenToDetailTrend]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		IF @TrendPenToDetailTrendGuid = '00000000-0000-0000-0000-000000000000'
        BEGIN 
			SET @TrendPenToDetailTrendGuid=NEWID();
		END
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblTrendPenToDetailTrend] 
		(
			[TrendPenToDetailTrendGuid]
		,	[PointTemplateTagGuid]
		,	[TrendGuid]
		,	[PenColor]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@TrendPenToDetailTrendGuid
		,	@PointTemplateTagGuid
		,	@TrendGuid
		,	@PenColor
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblTrendPenToDetailTrend]           
		WHERE TrendPenToDetailTrendGuid=@TrendPenToDetailTrendGuid;
	
 
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
						+ 'Procedure Name: gsp_TrendPenToDetailTrendInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END