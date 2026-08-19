CREATE PROCEDURE [dbo].[gsp_TrendInsertByPK]
(
		@TrendGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(30)=NULL
	,	@Description nvarchar(50)=NULL
	,	@Mode nvarchar(10)=NULL
	,	@PeriodType nvarchar(7)=NULL
	,	@Period float=NULL
	,	@Start datetimeoffset(7)=NULL
	,	@End datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@PointTemplateGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TrendInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-12-20 08:24:34.8048433 -05:00
	-- Purpose: Insert into table [dbo].[tblTrend]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TrendGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTrend] 
		(
			[TrendGuid]
		,	[ID]
		,	[Description]
		,	[Mode]
		,	[PeriodType]
		,	[Period]	
		,	[Start]
		,	[End]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[PointTemplateGuid]
		)
		VALUES
		(
			@TrendGuid
		,	@ID
		,	@Description
		,	@Mode
		,	@PeriodType	
		,	@Period
		,	@Start
		,	@End
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@PointTemplateGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTrend]           
		WHERE TrendGuid=@TrendGuid;
	
 
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
						+ 'Procedure Name: gsp_TrendInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END