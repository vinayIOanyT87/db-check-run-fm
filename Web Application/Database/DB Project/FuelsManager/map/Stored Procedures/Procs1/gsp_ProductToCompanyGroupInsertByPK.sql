CREATE PROCEDURE [map].[gsp_ProductToCompanyGroupInsertByPK]
(
		@ProductToCompanyGroupGuid uniqueidentifier=NULL OUTPUT
	,	@ProductGuid uniqueidentifier=NULL
	,	@AssignedToApplicationStringGuid uniqueidentifier=NULL
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
	,	@SpecialInstructionNote nvarchar(2000)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_ProductToCompanyGroupInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7172767 -05:00
	-- Purpose: Insert into table [map].[tblProductToCompanyGroup]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ProductToCompanyGroupGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblProductToCompanyGroup] 
		(
			[ProductToCompanyGroupGuid]
		,	[ProductGuid]
		,	[AssignedToApplicationStringGuid]
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
		,	[SpecialInstructionNote]
		)
		VALUES
		(
			@ProductToCompanyGroupGuid
		,	@ProductGuid
		,	@AssignedToApplicationStringGuid
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
		,	@SpecialInstructionNote
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblProductToCompanyGroup]           
		WHERE ProductToCompanyGroupGuid=@ProductToCompanyGroupGuid;
	
 
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
						+ 'Procedure Name: gsp_ProductToCompanyGroupInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
