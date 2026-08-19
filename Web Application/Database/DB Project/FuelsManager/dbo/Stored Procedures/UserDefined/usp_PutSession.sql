CREATE PROCEDURE [dbo].[usp_PutSession]
(
       @SessionGuid UNIQUEIDENTIFIER=NULL,
	   @SessionXml XML=NULL,
	   @UpdatedBy udtUserID=NULL
)
AS
BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored Procedure: [dbo].[usp_PutSession] 
       -- Author: Shawn Marlin
       -- Version/Date: 1.0.0 / 2015-08-25 14:21:10.4470770 -04:00
       -- Purpose: Insert or Update Sessions
       ------------------------------------------------------------------------------------------------------
       BEGIN TRY     
              --DETERMINE IF IT IS TIME TO DO SCHEDULING
              DECLARE @CURRENT_TIME DATETIMEOFFSET(7)
              SET @CURRENT_TIME = SYSDATETIMEOFFSET()
  

			  IF EXISTS (SELECT * FROM tblOpcUaSession WHERE SessionGuid = @SessionGuid)
					BEGIN
						UPDATE tblOpcUaSession Set SerializedSession = @SessionXml, UpdatedBy = @UpdatedBy, UpdatedDate = @CURRENT_TIME WHERE SessionGuid = @SessionGuid
					END
					ELSE
					BEGIN
					   INSERT INTO tblOpcUaSession (SerializedSession, CreatedDate, CreatedBy, UpdatedDate,UpdatedBy, SessionGuid)
					   VALUES (@SessionXml,@CURRENT_TIME,@UpdatedBy,@CURRENT_TIME,@UpdatedBy,@SessionGuid)
					END
 
       END TRY
       BEGIN CATCH  
              DECLARE       @_ErrMessage NVARCHAR(2048)      
                           , @_ErrNumber INT           
                           , @_ErrProcName NVARCHAR(126)           
                           , @_ErrLineNumber INT;            
              SET @_ErrMessage = ERROR_MESSAGE();        
              SET @_ErrNumber = ERROR_NUMBER();        
              SET @_ErrProcName= ERROR_PROCEDURE();        
              SET @_ErrLineNumber = ERROR_LINE();            
              SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
                                         + 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
                                         + 'Procedure Name: [dbo].usp_PutSession' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,18,1);      
       END CATCH    
       
END
