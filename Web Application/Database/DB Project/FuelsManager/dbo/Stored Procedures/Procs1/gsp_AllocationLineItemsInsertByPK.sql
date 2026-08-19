CREATE PROCEDURE [dbo].[gsp_AllocationLineItemsInsertByPK]
(
		@AllocationLineItemGuid uniqueidentifier=NULL OUTPUT
	,	@Limit float=NULL
	,	@Next float=NULL
	,	@ResetMultiple int=NULL
	,	@ResetDate datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@LookupAllocationTypeIndex int=NULL
	,	@LookupResetMethodIndex int=NULL
	,	@LookupResetPeriodIndex int=NULL
	,	@AllocationGuid uniqueidentifier=NULL
	,	@AssignedProductGuid uniqueidentifier=NULL
	,	@AssignedApplicationStringGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AllocationLineItemsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0372767 -05:00
	-- Purpose: Insert into table [dbo].[tblAllocationLineItems]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AllocationLineItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAllocationLineItems] 
		(
			[AllocationLineItemGuid]
		,	[Limit]
		,	[Next]
		,	[ResetMultiple]
		,	[ResetDate]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[LookupAllocationTypeIndex]
		,	[LookupResetMethodIndex]
		,	[LookupResetPeriodIndex]
		,	[AllocationGuid]
		,	[AssignedProductGuid]
		,	[AssignedApplicationStringGuid]
		)
		VALUES
		(
			@AllocationLineItemGuid
		,	@Limit
		,	@Next
		,	@ResetMultiple
		,	@ResetDate
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@LookupAllocationTypeIndex
		,	@LookupResetMethodIndex
		,	@LookupResetPeriodIndex
		,	@AllocationGuid
		,	@AssignedProductGuid
		,	@AssignedApplicationStringGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAllocationLineItems]           
		WHERE AllocationLineItemGuid=@AllocationLineItemGuid;
	
 
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
						+ 'Procedure Name: gsp_AllocationLineItemsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
