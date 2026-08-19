CREATE PROCEDURE [dbo].[usp_PointTemplatePropertyInsertByPK]
(
              @PointTemplatePropertyGuid uniqueidentifier=NULL OUTPUT
       ,      @ID nvarchar(50)=NULL
	    ,	     @ValueType nvarchar(max)=NULL
	    ,	     @Value xml=NULL
       ,      @CreatedDate datetimeoffset(7)=NULL
       ,      @CreatedBy udtUserID=NULL
       ,      @UpdatedDate datetimeoffset(7)=NULL
       ,      @UpdatedBy udtUserID=NULL
       ,      @PointTemplateGuid uniqueidentifier=NULL
       ,      @_ClusterIdx bigint=NULL OUTPUT
       ,      @_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored procedure: [dbo].[usp_PointTemplatePropertyInsertByPK] 
       -- Author: Warrem Gray
       -- Version/Date: 1.0.001 / 2016-02-06 16:55:13.9872767 -05:00
       -- Purpose: Insert into table [dbo].[tblPointTemplateProperty]
       -- Notes:
       ------------------------------------------------------------------------------------------------------
       SET NOCOUNT ON;
       BEGIN TRY

				IF ( @PointTemplatePropertyGuid IS NULL )
				BEGIN 
             SET @PointTemplatePropertyGuid=NEWID();
				END
             SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())

             INSERT INTO [dbo].[tblPointTemplateProperty] 
             (
                    [PointTemplatePropertyGuid]
             ,      [ID]
			    ,		  [ValueType]
			    ,		  [Value]
             ,      [CreatedDate]
             ,      [CreatedBy]
             ,      [UpdatedDate]
             ,      [UpdatedBy]
             ,      [PointTemplateGuid]
             )
             VALUES
             (
                    @PointTemplatePropertyGuid
             ,      @ID
			    ,		  @ValueType
             ,      @Value
             ,      @CreatedDate
             ,      @CreatedBy
             ,      @UpdatedDate
             ,      @UpdatedBy
             ,      @PointTemplateGuid
             )

             SELECT @_RowVersion = _RowVersion,@_ClusterIdx = _ClusterIdx        
             FROM [dbo].[tblPointTemplateProperty]           
             WHERE PointTemplatePropertyGuid=@PointTemplatePropertyGuid;

			 INSERT INTO [dbo].[tblPointProperty] 
             (
                    [PointPropertyGuid]
             ,      [ID]
			    ,		  [ValueType]
			    ,		  [Value]
             ,      [CreatedDate]
             ,      [CreatedBy]
             ,      [UpdatedDate]
             ,      [UpdatedBy]
             ,      [PointTemplatePropertyGuid]
			    ,		  [PointGuid]
             )
             SELECT
                     NEWID()
             ,      @ID
			    ,		  @ValueType
             ,      dbo.udf_UpdateXmlForDataContractSerializer(@ValueType, @Value)
             ,      @CreatedDate
             ,      @CreatedBy
             ,      @UpdatedDate
             ,      @UpdatedBy
             ,      @PointTemplatePropertyGuid
			 ,		PointGuid
			 FROM tblPoint WHERE PointTemplateGuid = @PointTemplateGuid
        

       END TRY
       BEGIN CATCH        
             DECLARE      @_ErrMessage NVARCHAR(2048)      
                           , @_ErrNumber INT           
                           , @_ErrProcName NVARCHAR(126)           
                           , @_ErrLineNumber INT;            
             SET @_ErrMessage = ERROR_MESSAGE();        
             SET @_ErrNumber = ERROR_NUMBER();        
             SET @_ErrProcName= ERROR_PROCEDURE();        
             SET @_ErrLineNumber = ERROR_LINE();            
             SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
                                        + 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
                                        + 'Procedure Name: usp_PointTemplatePropertyInsertByPK' + CHAR(13)+CHAR(10)                  
                                        + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
             RAISERROR(@_ErrMessage,18,1);      
       END CATCH    
END
GO


