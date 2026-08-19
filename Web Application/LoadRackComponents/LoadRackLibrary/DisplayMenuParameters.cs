using System;

namespace LoadRackLibrary
{
	/// <summary>
	/// Summary description for DisplayMenuParameters.
	/// </summary>
	public class DisplayMenuParameters
	{
		public string Caption;
		public string [] Menu;
		public bool ApplyDataDictionary = true;
		public int DefaultItem = 0;
		public int MenuTimeout = 90;
		public bool SaveForCancelProcessing = true;

		public DisplayMenuParameters(){}

		public DisplayMenuParameters(string Caption,string [] Menu,bool ApplyDataDictionary,int DefaultItem,int MenuTimeout)
		{
			this.Caption=Caption;
			this.Menu=Menu;
			this.ApplyDataDictionary=ApplyDataDictionary;
			this.DefaultItem=DefaultItem;
			this.MenuTimeout=MenuTimeout;
		}
	}
}
