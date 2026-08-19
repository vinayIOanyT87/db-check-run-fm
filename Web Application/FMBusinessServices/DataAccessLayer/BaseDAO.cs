namespace FMBusinessServices.DataAccessLayer
{
	using FMBusinessObjects.DataObjects;

	internal static class BaseDAO
	{
		internal static string SQLUpdateLock( bool bInTransaction )
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
		    return string.Empty;
		}
	}
}
