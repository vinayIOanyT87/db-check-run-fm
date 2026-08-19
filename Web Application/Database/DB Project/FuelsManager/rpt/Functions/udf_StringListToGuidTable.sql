
CREATE FUNCTION [rpt].[udf_StringListToGuidTable] 
 ( 
         @StringInput NVARCHAR(MAX)  
 ) 
 RETURNS @OutputTable TABLE ( IdentityGuid uniqueidentifier ) 
 AS 
 /********************************************************************************* 
 ** Description          : This function returns a table populated with a row for each string value in the space separated string 
 ** Assumptions          : None 
 ** Inputs               : @StringInput = the space separated string values 
 ** Outputs              : Single table  
 ** Output Rows          : One row per space separated string value 
 ** Return Values        : None 
 *********************************************************************************/ 
 BEGIN 
     DECLARE @StringValue VARCHAR(50) 
     DECLARE @identityGuid uniqueidentifier;
	 SET @StringInput = RTRIM(LTRIM(@StringInput))
	  
  
     WHILE LEN(@StringInput) > 0 
     BEGIN 
         SET @StringValue = LEFT(@StringInput,  
                            ISNULL(NULLIF(CHARINDEX(',', @StringInput) - 1, -1), 
                            LEN(@StringInput))) 

		 if(rpt.udf_IsStringUniqueidentifier(@StringValue) = 1)
		 begin
			Set @identityGuid = convert(uniqueidentifier,@StringValue);
			INSERT INTO @OutputTable ( IdentityGuid ) 
                            VALUES ( @StringValue ) 
		 end
         SET @StringInput = SUBSTRING(@StringInput, 
                            ISNULL(NULLIF(CHARINDEX(',', @StringInput), 0), 
                            LEN(@StringInput)) + 1, LEN(@StringInput)) 
  
         SET @StringInput = RTRIM(LTRIM(@StringInput)) 
  
         
     END 
  
     RETURN 
 END