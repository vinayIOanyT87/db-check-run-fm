namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetMapsBreadcrumbModel
	{
		#region Public members
		public const string CurrentPosition = "Current Position";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsBreadcrumbModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public List<string> BreadcrumbList { get; private set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initial the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.BreadcrumbList = new List<string> { CurrentPosition };

			for (int nextDay = 1; nextDay <= 60; nextDay++)
			{
				if (nextDay == 1)
				{
					this.BreadcrumbList.Add(nextDay + " day");
				}
				else
				{
					this.BreadcrumbList.Add(nextDay + " days");
				}
			}
		}
		#endregion
	}
}