CREATE PROCEDURE [map].[gsp_ProductToLedgerViewInsertByPK]
(
		@ProductToLedgerViewGuid uniqueidentifier=NULL OUTPUT
	,	@ProductGuid uniqueidentifier=NULL
	,	@AssignedToListViewGuid uniqueidentifier=NULL
	,	@Sequence int=NULL
	,	@BlendPercentage float=NULL
	,	@AdditiveRate float=NULL
	,	@Ratio float=NULL
	,	@AdditiveCycleVolume float=NULL
	,	@Tolerance float=NULL
	,	@PresetNumber int=NULL
	,	@AdditiveProfileGuid uniqueidentifier=NULL
	,	@TankGuid uniqueidentifier=NULL
	,	@MeterID nvarchar(20)=NULL
	,	@ShipToProductID nvarchar(30)=NULL
	,	@ShipToProductCode nvarchar(15)=NULL
	,	@ShipToLoadRackDisplayText nvarchar(10)=NULL
	,	@UnavailableInventoryGross float=NULL
	,	@UnavailableInventoryNet float=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_ProductToLedgerViewInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7192767 -05:00
	-- Purpose: Insert into table [map].[tblProductToLedgerView]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ProductToLedgerViewGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblProductToLedgerView] 
		(
			[ProductToLedgerViewGuid]
		,	[ProductGuid]
		,	[AssignedToListViewGuid]
		,	[Sequence]
		,	[BlendPercentage]
		,	[AdditiveRate]
		,	[Ratio]
		,	[AdditiveCycleVolume]
		,	[Tolerance]
		,	[PresetNumber]
		,	[AdditiveProfileGuid]
		,	[TankGuid]
		,	[MeterID]
		,	[ShipToProductID]
		,	[ShipToProductCode]
		,	[ShipToLoadRackDisplayText]
		,	[UnavailableInventoryGross]
		,	[UnavailableInventoryNet]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@ProductToLedgerViewGuid
		,	@ProductGuid
		,	@AssignedToListViewGuid
		,	@Sequence
		,	@BlendPercentage
		,	@AdditiveRate
		,	@Ratio
		,	@AdditiveCycleVolume
		,	@Tolerance
		,	@PresetNumber
		,	@AdditiveProfileGuid
		,	@TankGuid
		,	@MeterID
		,	@ShipToProductID
		,	@ShipToProductCode
		,	@ShipToLoadRackDisplayText
		,	@UnavailableInventoryGross
		,	@UnavailableInventoryNet
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblProductToLedgerView]           
		WHERE ProductToLedgerViewGuid=@ProductToLedgerViewGuid;
	
 
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
						+ 'Procedure Name: gsp_ProductToLedgerViewInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
