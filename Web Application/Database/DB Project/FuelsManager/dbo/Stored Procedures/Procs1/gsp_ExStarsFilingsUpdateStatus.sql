CREATE PROCEDURE [dbo].[gsp_ExStarsFilingsUpdateStatus]
  @ExStarsFilingsGuid		UNIQUEIDENTIFIER
, @FilingStatus				NVARCHAR(30)
, @UpdatedBy				[dbo].[udtUserID]
AS
BEGIN
	UPDATE [dbo].[tblExStarsFilings] SET  
	  [UpdatedDate]=GETDATE() 
	, [FilingStatus]=@FilingStatus	
	, [UpdatedBy]=@UpdatedBy
	WHERE ExStarsFilingsGuid=@ExStarsFilingsGuid
END