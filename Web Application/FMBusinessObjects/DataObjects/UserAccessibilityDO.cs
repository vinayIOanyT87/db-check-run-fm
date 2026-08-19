using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class UserAccessibilityDO
	{
		protected Guid userGuid;

		protected bool enabled = false;
		protected bool outlineFocusedControls = true;
		protected bool enablePleaseWaitSound = true;
		protected bool enableSessionTimeoutNotification = true;
		protected int sessionTimeoutNotificationMinute = 5;
		protected bool enableKeyboardForMenu = true;
		protected int pleaseWaitAudioDelay =3;

		public bool Enabled {get{return enabled;}}
		public bool OutlineFocusedControls {get{return outlineFocusedControls;}}
		public bool EnablePleaseWaitSound {get{return enablePleaseWaitSound;}}
		public bool EnableSessionTimeoutNotification {get{return enableSessionTimeoutNotification;}}
		public int SessionTimeoutNotificationMinute {get{return sessionTimeoutNotificationMinute;}}
		public bool EnableKeyboardForMenu {get{return enableKeyboardForMenu;}}
		public int PleaseWaitAudioDelay { get{return pleaseWaitAudioDelay;} }

		public UserAccessibilityDO(SecurityClass security, Guid _userGuid)
		{
			this.userGuid = _userGuid;
			initialize(security);
		}

		protected void initialize(SecurityClass security)
		{

			try
			{
				AccessibilityCollectionClass accessibilities = new AccessibilityCollectionClass();
				accessibilities = FMChannelHelper.MakeCall<IAccessibilities, AccessibilityCollectionClass>(	x =>x.Enumerate(security, userGuid)		);

				foreach (AccessibilityClass accessibility in accessibilities)
				{
					switch (accessibility.SettingKey.ToLower())
					{
						case "enabled":
							bool bVal = false;
							if (bool.TryParse(accessibility.SettingValue, out bVal))
							{
								this.enabled = bVal;
							}
							break;
						case "outlinefocusedcontrols":
							bVal = false;
							if (bool.TryParse(accessibility.SettingValue, out bVal))
							{
								this.outlineFocusedControls = bVal;
							}
							break;
						case "enablepleasewaitsound":
							bVal = false;
							if (bool.TryParse(accessibility.SettingValue, out bVal))
							{
								this.enablePleaseWaitSound = bVal;
							}
							break;
						case "enablesessiontimeoutnotification":
							bVal = false;
							if (bool.TryParse(accessibility.SettingValue, out bVal))
							{
								this.enableSessionTimeoutNotification = bVal;
							}
							break;
						case "sessiontimeoutnotificationminute":
							int iVal = 5;
							if (int.TryParse(accessibility.SettingValue, out iVal))
							{
								this.sessionTimeoutNotificationMinute = iVal;
							}
							break;
						case "enablekeyboardformenu":
							bVal = false;
							if (bool.TryParse(accessibility.SettingValue, out bVal))
							{
								this.enableKeyboardForMenu = bVal;
							}
							break;
						case "pleasewaitaudiodelay":
							iVal = 3;
							if (int.TryParse(accessibility.SettingValue, out iVal))
							{
								this.pleaseWaitAudioDelay = iVal;
							}
							break;
					}
				}
			}
			catch
			{

			}
		}


	}
}
