namespace FuelsManager.MenuBar
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI;

    using FMCore;

	using FuelsManager.FMWebApp;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using global::FMWebApp;

   public partial class FMMenuBar : FMFormBase
	{
		protected string HelpTarget { get; set; }

		protected void Page_Load( object sender, EventArgs e )
		{
			try
			{
				var security = (SecurityClass)Session["Security"];
				if (security == null)
				{
					throw new FMSessionInvalidException();
				}

				var target = this.Request.GetQueryOrFormValue("target");
				var substring = target.Substring(0, target.LastIndexOf('/'));
				if (substring == "../InventoryManagement/Points/PointsDetail" ||
				substring == "../InventoryManagement/PointAccess/PointAccessUserDetail" ||
				substring == "../InventoryManagement/PointAccess/PointAccessUserGroupDetail" ||
				substring == "../InventoryManagement/PointAccess/PointAccessGroupDetail" ||
				substring == "../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetail" ||
				substring == "../InventoryManagement/PointAccess/PointAccess")
					HelpTarget = "?target=" + substring;
				else
				{
					HelpTarget = "?target=" + target;
				}

				if ( string.IsNullOrEmpty( target ) )
				{
					throw new ArgumentNullException( "target" );
				}
            var title = this.Request.GetQueryOrFormValue("title")??string.Empty;

				var iframe = string.Format("<iframe id='iframeContent' src='{0}' style='border: none; overflow-x:hidden' title='{1}'></iframe>", Server.HtmlEncode(target), Server.HtmlEncode(title));

            this.content.Controls.Add( new LiteralControl( iframe ) );
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

        /// <summary>
        /// Override here, for example if you have tabbed pages that need multiple help mappings on a single page
        /// </summary>
        /// <returns>Returns a list of URL keys</returns>
		public override List<string> GetHelpContextKeys()
		{
			List<string> list = new List<string>();
			string baseUrl = base.GetHelpContextKey();
			string firstKey = this.HelpTarget;

			list.Add(baseUrl + firstKey);

			switch (firstKey)
			{
				case "?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetail":
					list.Add(baseUrl + "?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetailTags");
					list.Add(baseUrl + "?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetailModules");
					list.Add(baseUrl + "?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetailAlarms");
					break;
				case "?target=../InventoryManagement/Points/PointsDetail":
					list.Add(baseUrl + "?target=../InventoryManagement/Points/PointsDetailTags");
					list.Add(baseUrl + "?target=../InventoryManagement/Points/PointsDetailModules");
					list.Add(baseUrl + "?target=../InventoryManagement/AlarmEditor/AlarmEditorView");
					break;
				case "?target=../InventoryManagement/PointAccess/PointAccessGroupDetail":
					list.Add(baseUrl + "?target=../InventoryManagement/PointAccess/PointAccessGroupDetailUserGroupTab");
					break;
				case "?target=../InventoryManagement/PointAccess/PointAccessUserGroupDetail":
					list.Add(baseUrl + "?target=../InventoryManagement/PointAccess/PointAccessGroupDetailUsersTab");
					break;
				case "?target=../Config/ConfigurationSettings/ConfigurationSettingsIndex":
					list.Add(baseUrl + "?target=../Config/ConfigurationSettings/ConfigurationSettingsDetail");
					break;
				case "?target=../InventoryManagement/Operate/OperateIndex":
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexPointsTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexGraphicsTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexPointGroupsTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexPointCalculator");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexTrendsTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexPointHistory");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexReportsTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexAlarmsTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexAlarmHistoryTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexMovementSummaryTab");
					list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexMovementHistoryTab");
               list.Add(baseUrl + "?target=../InventoryManagement/Operate/OperateIndexLeakAnalysis");
                    break;
                case "?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView":
                    list.Add(baseUrl + "?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView/UserInfoTab");
                    list.Add(baseUrl + "?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView/UserPermissionGroupTab");
                    list.Add(baseUrl + "?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView/UserAdminAuditTab");
                    break;
                case "?target=../AssetTrackingArea/AssetDeviceConfigurationSummary/DeviceConfigurationSummary":
                    list.Add(baseUrl + "?target=../AssetTrackingArea/AssetDeviceConfiguration/DeviceConfiguration");
                    break;
					case "?target=../InventoryManagement/PointsSummary/PointsSummaryView":
						list.Add(baseUrl + "?target=../InventoryManagement/PointsSummary/AddPoints");
						break;
					case "?target=../AssetTrackingArea/AssetMapConfigurationSummary/MapConfigurationSummary":
							  list.Add(baseUrl + "?target=../AssetTrackingArea/AssetMapConfiguration/MapConfiguration");
							  break;
            }

            return list;
		}

	}
}