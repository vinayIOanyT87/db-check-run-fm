CREATE PROCEDURE [dbo].[usp_AnimationAddModify]
(
	@AnimationTempTable AnimationDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblAnimation at
USING @AnimationTempTable    temp        ON temp.[AnimationGuid] = at.[AnimationGuid]
              			 							                        
WHEN MATCHED AND	@EnableModify = 1 AND (
						temp.ID <> at.ID 
						OR temp.SiteGuid <> at.SiteGuid 
						OR temp.AnimationTestGroupList <> CAST(at.AnimationTestGroupList as nvarchar(max))		
					)THEN
    UPDATE SET	ID = temp.ID  
				,SiteGuid = temp.SiteGuid  
				,AnimationTestGroupList = temp.AnimationTestGroupList
				,UpdatedBy = temp.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 

WHEN NOT MATCHED AND @EnableAdd = 1 THEN
    INSERT 
	 (
			[AnimationGuid],
			[ID],
			[SiteGuid],
			[AnimationTestGroupList],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy]
	)
	VALUES
	(
		temp.AnimationGuid
		,temp.ID
		,temp.SiteGuid
		,temp.AnimationTestGroupList
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
	);

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
						+ 'Procedure Name: usp_AnimationAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 