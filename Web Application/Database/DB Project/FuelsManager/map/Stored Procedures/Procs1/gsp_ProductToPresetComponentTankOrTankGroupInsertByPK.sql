CREATE PROCEDURE [map].[gsp_ProductToPresetComponentTankOrTankGroupInsertByPK]
(
		@ProductToPresetComponentTankOrTankGroupGuid uniqueidentifier=NULL OUTPUT
	,	@ProductGuid uniqueidentifier=NULL
	,	@AssignedToLoadArmGuid uniqueidentifier=NULL
	,	@Sequence int=NULL
	,	@BlendPercentage float=NULL
	,	@AdditiveRate float=NULL
	,	@Ratio float=NULL
	,	@AdditiveCycleVolume float=NULL
	,	@Tolerance float=NULL
	,	@PresetNumber int=NULL
	,	@AdditiveProfileGuid uniqueidentifier=NULL
	,	@TankGuid uniqueidentifier=NULL
	,	@TankGroupApplicationStringGuid uniqueidentifier=NULL
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
	,	@AssignedToMeterGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_ProductToPresetComponentTankOrTankGroupInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7222767 -05:00
	-- Purpose: Insert into table [map].[tblProductToPresetComponentTankOrTankGroup]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ProductToPresetComponentTankOrTankGroupGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblProductToPresetComponentTankOrTankGroup] 
		(
			[ProductToPresetComponentTankOrTankGroupGuid]
		,	[ProductGuid]
		,	[AssignedToLoadArmGuid]
		,	[Sequence]
		,	[BlendPercentage]
		,	[AdditiveRate]
		,	[Ratio]
		,	[AdditiveCycleVolume]
		,	[Tolerance]
		,	[PresetNumber]
		,	[AdditiveProfileGuid]
		,	[TankGuid]
		,	[TankGroupApplicationStringGuid]
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
		,	[AssignedToMeterGuid]
		)
		VALUES
		(
			@ProductToPresetComponentTankOrTankGroupGuid
		,	@ProductGuid
		,	@AssignedToLoadArmGuid
		,	@Sequence
		,	@BlendPercentage
		,	@AdditiveRate
		,	@Ratio
		,	@AdditiveCycleVolume
		,	@Tolerance
		,	@PresetNumber
		,	@AdditiveProfileGuid
		,	@TankGuid
		,	@TankGroupApplicationStringGuid
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
		,	@AssignedToMeterGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblProductToPresetComponentTankOrTankGroup]           
		WHERE ProductToPresetComponentTankOrTankGroupGuid=@ProductToPresetComponentTankOrTankGroupGuid;
	
 
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
						+ 'Procedure Name: gsp_ProductToPresetComponentTankOrTankGroupInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
