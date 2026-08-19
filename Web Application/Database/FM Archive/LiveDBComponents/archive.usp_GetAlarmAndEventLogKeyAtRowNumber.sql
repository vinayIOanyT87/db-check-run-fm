/*
       DROP PROCEDURE [archive].[usp_GetAlarmAndEventLogKeyAtRowNumber]

       EXEC [archive].[usp_GetAlarmAndEventLogKeyAtRowNumber] '2015-03-04', 100000
       
*/
CREATE PROCEDURE [archive].[usp_GetAlarmAndEventLogKeyAtRowNumber]
(
	@cutOffDate date,
	@rowNumber int
)
AS
BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored procedure: [fmcdc].[usp_GetAlarmAndEventLogKeyAtRowNumber]
       -- Author: Hansraj Bapoo
       -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
       -- Purpose: Retrieves the AlarmAndEventLogKey (SequenceNumber) of the AlarmAndEventLog record that is 
       --          located at a given row number of Alarm and Event Log records, from a given cut-off date.
       -- Notes:
	   -- 1. @cutOffDate: Cut-off date for the set of records from which to retrieve the Key value.
       -- 2. @rowNumber: Row number at which to retrieve the AlarmAndEventLogKey
       ------------------------------------------------------------------------------------------------------
       SET NOCOUNT ON;
       BEGIN TRY

              SELECT CONVERT(BigInt, x.SequenceNumber) FROM
              (
                     SELECT ROW_NUMBER() OVER (ORDER BY SequenceNumber ASC) As RowNum, SequenceNumber FROM dbo.tblAlarmAndEventLog 
					 WHERE UpdatedDate <= @cutOffDate
              ) x
              WHERE x.RowNum =@rowNumber

                                  
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
                                         + 'Procedure Name: [fmcdc].[usp_GetAlarmAndEventLogKeyAtRowNumber]' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,16,1);      
       END CATCH    
       
END



GO


