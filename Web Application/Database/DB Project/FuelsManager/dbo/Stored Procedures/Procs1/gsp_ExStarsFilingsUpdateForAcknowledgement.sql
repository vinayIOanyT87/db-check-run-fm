CREATE PROCEDURE [dbo].[gsp_ExStarsFilingsUpdateForAcknowledgement]
  @TransSetControlNumber		NVARCHAR(9)
, @FilingStatus					NVARCHAR(30)
, @Acknowledgement				NVARCHAR(max)
, @AckEasyRead					NVARCHAR(max)
, @UpdatedBy					[dbo].[udtUserID]
AS
BEGIN
	UPDATE [dbo].[tblExStarsFilings] SET  
	  [UpdatedDate]=GETDATE() 
	, [ResponseLoaded]=GETDATE() 
	, [FilingStatus]=@FilingStatus	
	, [Acknowledgement]=@Acknowledgement
	, [AckEasyRead]=@AckEasyRead
	, [UpdatedBy]=@UpdatedBy
	WHERE [TransSetControlNumber]=@TransSetControlNumber
END