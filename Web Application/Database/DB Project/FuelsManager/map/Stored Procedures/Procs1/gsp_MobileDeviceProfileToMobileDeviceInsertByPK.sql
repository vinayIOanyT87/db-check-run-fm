CREATE PROCEDURE [map].[gsp_MobileDeviceProfileToMobileDeviceInsertByPK]
(
		@MobileDeviceProfileToMobileDeviceGuid uniqueidentifier=NULL OUTPUT
	,	@MobileDeviceProfileGuid uniqueidentifier=NULL
	,	@AssignedToMobileDeviceGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_MobileDeviceProfileToMobileDeviceInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6952767 -05:00
	-- Purpose: Insert into table [map].[tblMobileDeviceProfileToMobileDevice]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MobileDeviceProfileToMobileDeviceGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblMobileDeviceProfileToMobileDevice] 
		(
			[MobileDeviceProfileToMobileDeviceGuid]
		,	[MobileDeviceProfileGuid]
		,	[AssignedToMobileDeviceGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@MobileDeviceProfileToMobileDeviceGuid
		,	@MobileDeviceProfileGuid
		,	@AssignedToMobileDeviceGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblMobileDeviceProfileToMobileDevice]           
		WHERE MobileDeviceProfileToMobileDeviceGuid=@MobileDeviceProfileToMobileDeviceGuid;
	
 
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
						+ 'Procedure Name: gsp_MobileDeviceProfileToMobileDeviceInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
