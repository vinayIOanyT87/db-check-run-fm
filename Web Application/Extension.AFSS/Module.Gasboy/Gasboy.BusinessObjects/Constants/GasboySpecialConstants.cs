using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    public class GasboySpecialConstants
    {
        public static readonly string NoRestrictionGroupRuleName = "No Restriction";

        public static readonly int NoRestrictionGroupRuleCode = 200000000;

		// Default Root Fleet Related Constants
		public static readonly Guid DefaultFleetGuid = new Guid("00000009-0000-0000-0000-000000000000");

		public static readonly string DefaultFleetName = "Default";

		public static readonly int DefaultFleetID = 900000002;

		public static readonly int DefaultFleetCode = 1;

		// Default Positive/Whitelist Department Related Constants

		public static readonly Guid DefaultDepartmentGuid = new Guid("00000002-0000-0000-0000-000000000000");

		public static readonly string DefaultDepartmentName = "Default";

		public static readonly int DefaultDepartmentID = 900000002;

		public static readonly int DefaultDepartmentCode = 9998;

		public static readonly int DefaultDepartmentType = 1;

		// Default Negative/Blacklist Department Related Constants
		public static readonly Guid BlacklistDepartmentGuid = new Guid("00000001-0000-0000-0000-000000000000");

		public static readonly string DefaultBlackListDepartmentName = "Blacklist";

		public static readonly int DefaultBlackListDepartmentID = 900000003;

		public static readonly int DefaultBlackListDepartmentCode = 9999;

		public static readonly int BlacklistDepartmentType = 2;

	}
}
