CREATE PROCEDURE [dbo].[gsp_ReserveLevelsInsertByPK]
(
		@ReserveLevelGuid uniqueidentifier=NULL OUTPUT
	,	@MinimumLevel float=NULL
	,	@WarningLevel float=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ReserveLevelsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4172767 -05:00
	-- Purpose: Insert into table [dbo].[tblReserveLevels]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ReserveLevelGuid=NEWID();
 
		INSERT INTO [dbo].[tblReserveLevels] 
		(
			[ReserveLevelGuid]
		,	[MinimumLevel]
		,	[WarningLevel]
		,	[SiteGuid]
		,	[ProductGuid]
		)
		VALUES
		(
			@ReserveLevelGuid
		,	@MinimumLevel
		,	@WarningLevel
		,	@SiteGuid
		,	@ProductGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblReserveLevels]           
		WHERE ReserveLevelGuid=@ReserveLevelGuid;
	
 
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
						+ 'Procedure Name: gsp_ReserveLevelsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
