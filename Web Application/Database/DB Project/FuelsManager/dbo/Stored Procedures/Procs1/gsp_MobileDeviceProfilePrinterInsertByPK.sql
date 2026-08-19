CREATE PROCEDURE [dbo].[gsp_MobileDeviceProfilePrinterInsertByPK]
(
		@MobileDeviceProfilePrinterGUID uniqueidentifier=NULL OUTPUT
	,	@MobileDeviceProfileGUID uniqueidentifier=NULL
	,	@PrinterID nvarchar(30)=NULL
	,	@BaudRate nvarchar(8)=NULL
	,	@COMPort nvarchar(4)=NULL
	,	@DataBits nvarchar(8)=NULL
	,	@StopBits nvarchar(8)=NULL
	,	@UseXonXoff nvarchar(8)=NULL
	,	@XonChar nvarchar(8)=NULL
	,	@XoffChar nvarchar(8)=NULL
	,	@BufferSize nvarchar(8)=NULL
	,	@Parity nvarchar(12)=NULL
	,	@CreatedBy nvarchar(50)=NULL
	,	@UpdatedBy nvarchar(50)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MobileDeviceProfilePrinterInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.3032767 -05:00
	-- Purpose: Insert into table [dbo].[tblMobileDeviceProfilePrinter]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MobileDeviceProfilePrinterGUID=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblMobileDeviceProfilePrinter] 
		(
			[MobileDeviceProfilePrinterGUID]
		,	[MobileDeviceProfileGUID]
		,	[PrinterID]
		,	[BaudRate]
		,	[COMPort]
		,	[DataBits]
		,	[StopBits]
		,	[UseXonXoff]
		,	[XonChar]
		,	[XoffChar]
		,	[BufferSize]
		,	[Parity]
		,	[CreatedBy]
		,	[UpdatedBy]
		,	[CreatedDate]
		,	[UpdatedDate]
		)
		VALUES
		(
			@MobileDeviceProfilePrinterGUID
		,	@MobileDeviceProfileGUID
		,	@PrinterID
		,	@BaudRate
		,	@COMPort
		,	@DataBits
		,	@StopBits
		,	@UseXonXoff
		,	@XonChar
		,	@XoffChar
		,	@BufferSize
		,	@Parity
		,	@CreatedBy
		,	@UpdatedBy
		,	@CreatedDate
		,	@UpdatedDate
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblMobileDeviceProfilePrinter]           
		WHERE MobileDeviceProfilePrinterGUID=@MobileDeviceProfilePrinterGUID;
	
 
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
						+ 'Procedure Name: gsp_MobileDeviceProfilePrinterInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
