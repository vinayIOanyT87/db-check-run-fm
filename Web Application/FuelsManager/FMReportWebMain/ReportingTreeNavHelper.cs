

namespace FuelsManager.FMReportWebMain
{
	using FMBusinessObjects.DataObjects;

	public class ReportingTreeNavHelper
	{


		/// <summary>
		/// This method will determine if the user has report configuration permissions. If so,
		/// the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasConfigurationPermissions ( SecurityClass security )
		{
			return security.HasRight(RIGHT.MODIFY_REPORTS);
		}

		/// <summary>
		/// This method will determine if the user has report view permissions. If so,
		/// the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasViewPermissions ( SecurityClass security )
		{
			return security.HasRight( RIGHT.VIEW_REPORTS );
		}

		/// <summary>
		/// This method will return true if there is a valid hardware key for Enterprise Reports.
		/// Otherwise, it will return false. The key is located in the upper word of a 32 bit word 
		/// and the value is 0x10.
		/// </summary>
		/// <returns></returns>
		public bool HasHardwareKey ( uint options )
		{
			bool hasKey = (options & 0x100000) != 0;

			return hasKey;
		}
	}
}
