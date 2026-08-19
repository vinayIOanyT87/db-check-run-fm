/*
	function used by another function to compare version for dacpac processing
*/
CREATE Function [dbo].[udf_VersionNthPart]( @version as nvarchar(max), @part as int) returns int as
Begin

Declare
    @ret as int = null,
    @start as int = 1,
    @end as int = 0,
    @partsFound as int = 0,
    @terminate as bit = 0

  if @version is not null
  Begin
    Set @ret = 0
    while @partsFound < @part
    Begin
      Set @end = charindex('.', @version, @start)
      If @end = 0 -- did not find the dot. Either it was last part or the part was missing.
      begin
        if @part - @partsFound > 1 -- also this isn't the last part so it must bail early.
        begin
            set @terminate = 1
        end
        Set @partsFound = @part
        SET @end = len(@version) + 1; -- get the full length so that it can grab the whole of the final part.
      end
      else
      begin
        SET @partsFound = @partsFound + 1
      end
      If @partsFound = @part and @terminate = 0
      begin
            Set @ret = Convert(int, substring(@version, @start, @end - @start))
      end
      Else
      begin
            Set @start = @end + 1
      end
    End
  End
  return @ret
End
GO