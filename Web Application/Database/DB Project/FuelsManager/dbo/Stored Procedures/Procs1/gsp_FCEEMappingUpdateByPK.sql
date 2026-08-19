CREATE PROCEDURE [dbo].[gsp_FCEEMappingUpdateByPK]
(
	@FCEEMappingGuid uniqueidentifier,
	@FCEDeviceGuid uniqueidentifier=NULL,
	@MsgType int=NULL,
	@Index int=NULL,
	@Device int=NULL,
	@TagSelection int=NULL,
	@PointGuid uniqueidentifier=NULL,
	@CreatedDate datetimeoffset(7)=NULL,
	@CreatedBy udtUserID=NULL,
	@UpdatedDate datetimeoffset(7)=NULL,
	@UpdatedBy udtUserID=NULL,
	@_RowVersion timestamp=NULL OUTPUT,
	@NullOverrideFCEDeviceGuid BIT=0,
	@NullOverrideMsgType BIT=0,
	@NullOVerrideIndex BIT=0,
	@NullOverridePointGuid BIT=0,
	@NullOverrideUpdatedDate BIT=0,
	@PointID nvarchar(35)=NULL,
	@ID nvarchar(35)=NULL,
	@SiteGuid uniqueidentifier 
)

AS
BEGIN

	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_FCEEMappingUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	-- Purpose: Update into table [dbo].[tblFCEEMapping]
	-- Notes:
	------------------------------------------------------------------------------------------------------

SET NOCOUNT ON;
BEGIN TRY
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblFCEEMapping] WHERE FCEEMappingGuid=@FCEEMappingGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END

		UPDATE [dbo].[tblFCEEMapping] SET
			[FCEDeviceGuid]=(CASE ISNULL(@NullOverrideFCEDeviceGuid,0) WHEN 1 THEN @FCEDeviceGuid ELSE ISNULL(@FCEDeviceGuid,[FCEDeviceGuid]) END)
		,	[MsgType]=(CASE ISNULL(@NullOverrideMsgType,0) WHEN 1 THEN @MsgType ELSE ISNULL(@MsgType,[MsgType]) END)
		,	[Index]=(CASE ISNULL(@NullOverrideIndex,0) WHEN 1 THEN @Index ELSE ISNULL(@Index,[Index]) END)
		,	[Device]=@Device
		,	[TagSelection]=@TagSelection
		,	[PointGuid]=(CASE ISNULL(@NullOverridePointGuid,0) WHEN 1 THEN @PointGuid ELSE ISNULL(@PointGuid,[PointGuid]) END)
		,	[UpdatedDate]=(CASE ISNULL(@NullOverrideUpdatedDate,0) WHEN 1 THEN @UpdatedDate ELSE ISNULL(@UpdatedDate,[UpdatedDate]) END)
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		WHERE FCEEMappingGuid=@FCEEMappingGuid

		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblFCEEMapping]           
		WHERE FCEEMappingGuid=@FCEEMappingGuid;

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
						+ 'Procedure Name: [gsp_FCEEMappingUpdateByPK]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
GO