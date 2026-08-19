using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataImportExportWizard.Constants
{
	public static class Guids
	{
		// General Guids
		public static readonly Guid AllFilterGuid = new Guid("10000000-0000-0000-0000-000000000000");

		// Site Guids
		public static readonly Guid SiteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");		
		public static readonly Guid SiteDefaultGuid = new Guid("00000000-0000-0000-0000-000000000004");	// Default Site Guid for single site key

		// Group Guids
		public static readonly Guid GroupAdminGuid = new Guid("00000000-0000-0000-0000-000000000003");

		// User Guids
		public static readonly Guid UserAdminGuid = new Guid("00000000-0000-0000-0000-000000000002");

		// System Settings Guids
		public static readonly Guid SystemSettingsGuid = new Guid("00000000-0000-0000-0000-000000000005");

		// Accuload Guids
		public static readonly Guid AcculoadGuid = new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}");
		public static readonly Guid AcculoadPortGuid = new Guid("{2070F4BA-651D-4268-9F5A-1EBE0A137141}");
		public static readonly Guid AcculoadCardReaderGuid = new Guid("{0AB8E5B2-986C-4B03-A0C7-243FC6963328}");

		// Contrec Guids
		public static readonly Guid ContrecGuid = new Guid("{59DB8E98-D175-49A8-997B-8D342154B9D7}");
		public static readonly Guid ContrecPortGuid = new Guid("{2B2CCFD9-9EF7-48BB-BEF4-C58C0C43409D}");

		// Daniel Guids
		public static readonly Guid DanielGuid = new Guid("{54F57ECB-6111-4A9A-AFA6-ABC5B3C4FF59}");
		public static readonly Guid DanielPortGuid = new Guid("{265331A0-40D0-4DEC-B614-1A21CDC5CC1F}");

		// OptomuxController Guids
		public static readonly Guid OptomuxControllerGuid = new Guid("{DD940B4F-C212-4361-8FDE-D4061584E4D0}");
		public static readonly Guid OptomuxControllerPortGuid = new Guid("{D1CAA238-8AB9-4E70-A628-49AB61EC5BD1}");

		// SCADA Guids
		public static readonly Guid SCADAGraphicGuid = new Guid("{9E79E49B-5765-4793-AD41-F3EEB156E5D2}");

		// Tank Guids
		public static readonly Guid TankGuid = new Guid("{F075F7A6-0D97-4C94-B8FA-E3F9EB149833}");

		// WeightScale Guids
		public static readonly Guid WeightScaleGuid = new Guid("{FB4C3029-D5C9-4BB8-AC5A-1914858D79D5}");

		// RequestParser Guids
		public static readonly Guid UninitializedSiteGuid = new Guid("{00000000-0000-0000-0000-000000000005}");
		public static readonly Guid UninitializedLoginSiteGuid = new Guid("{00000000-0000-0000-0000-000000000006}");
		public static readonly Guid UninitializedUserGuid = new Guid("{00000000-0000-0000-0000-000000000007}");

        // Enterprise Master Synchronization Server (There can only be one that represents this.  Subsequent ones are configured for each child/sibling instance)
        // public static readonly Guid EnterpriseServerGuid = new Guid("{10000000-1000-1000-1000-100000000000}");
	}
}
