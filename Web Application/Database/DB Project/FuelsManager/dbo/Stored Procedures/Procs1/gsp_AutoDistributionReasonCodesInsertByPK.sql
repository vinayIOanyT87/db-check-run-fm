CREATE PROCEDURE [dbo].[gsp_AutoDistributionReasonCodesInsertByPK]
(
		@AutoDistributionReasonCodeGuid uniqueidentifier=NULL OUTPUT
	,	@SiteGuid uniqueidentifier=NULL
	,	@ReasonCode nvarchar(50)=NULL
	,	@Description nvarchar(255)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy nvarchar(50)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy nvarchar(50)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AutoDistributionReasonCodesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0842767 -05:00
	-- Purpose: Insert into table [dbo].[tblAutoDistributionReasonCodes]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AutoDistributionReasonCodeGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAutoDistributionReasonCodes] 
		(
			[AutoDistributionReasonCodeGuid]
		,	[SiteGuid]
		,	[ReasonCode]
		,	[Description]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@AutoDistributionReasonCodeGuid
		,	@SiteGuid
		,	@ReasonCode
		,	@Description
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAutoDistributionReasonCodes]           
		WHERE AutoDistributionReasonCodeGuid=@AutoDistributionReasonCodeGuid;
	
 
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
						+ 'Procedure Name: gsp_AutoDistributionReasonCodesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
