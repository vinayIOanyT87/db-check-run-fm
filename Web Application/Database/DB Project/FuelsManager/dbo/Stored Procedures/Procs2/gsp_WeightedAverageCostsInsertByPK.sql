CREATE PROCEDURE [dbo].[gsp_WeightedAverageCostsInsertByPK]
(
		@WeightedAverageCostGuid uniqueidentifier=NULL OUTPUT
	,	@WacValue float=NULL
	,	@IsManualOverride bit=NULL
	,	@Source nvarchar(64)=NULL
	,	@Notes nvarchar(2048)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@InventoryDate date=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_WeightedAverageCostsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6292767 -05:00
	-- Purpose: Insert into table [dbo].[tblWeightedAverageCosts]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @WeightedAverageCostGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblWeightedAverageCosts] 
		(
			[WeightedAverageCostGuid]
		,	[WacValue]
		,	[IsManualOverride]
		,	[Source]
		,	[Notes]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[InventoryDate]
		,	[SiteGuid]
		,	[ProductGuid]
		)
		VALUES
		(
			@WeightedAverageCostGuid
		,	@WacValue
		,	@IsManualOverride
		,	@Source
		,	@Notes
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@InventoryDate
		,	@SiteGuid
		,	@ProductGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblWeightedAverageCosts]           
		WHERE WeightedAverageCostGuid=@WeightedAverageCostGuid;
	
 
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
						+ 'Procedure Name: gsp_WeightedAverageCostsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
