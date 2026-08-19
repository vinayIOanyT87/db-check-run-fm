namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	[Serializable]
	public class AssetMapsDropdownModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsDropdownModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string SelectedEquipment { get; set; }
		public string SelectedDeliveryLocation { get; set; }
		public string SelectedFacility { get; set; }
		public string SelectedBreadcrumb { get; set; }
		public string SelectedBreadcrumbIndex { get; set; }
		public string SelectedTank { get; set; }

		public List<MapMenuItemClass> BreadcrumbDropdownList { get; set; }
		public List<MapMenuItemClass> EquipmentDropdownList { get; set; }
		public List<MapMenuItemClass> FacilityDropdownList { get; set; }
		public List<MapMenuItemClass> DeliveryLocationDropdownList { get; set; }
		public List<MapMenuItemClass> TankDropdownList { get; set; }
		public int EquipmentMenuExpanded { get; set; }
		public int FacilityMenuExpanded { get; set; }
		public int DeliveryLocationMenuExpanded { get; set; }
		public int TankMenuExpanded { get; set; }
		public int BreadcrumbMenuExpanded { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.SelectedEquipment				= string.Empty;
			this.SelectedDeliveryLocation		= string.Empty;
			this.SelectedFacility				= string.Empty;
			this.SelectedBreadcrumb				= string.Empty;
			this.SelectedBreadcrumbIndex		= "0";
			this.SelectedTank					= string.Empty;
			this.BreadcrumbDropdownList			= new List<MapMenuItemClass>();
			this.EquipmentDropdownList			= new List<MapMenuItemClass>();
			this.FacilityDropdownList			= new List<MapMenuItemClass>();
			this.DeliveryLocationDropdownList	= new List<MapMenuItemClass>();
			this.TankDropdownList				= new List<MapMenuItemClass>();
			this.EquipmentMenuExpanded			= 0; // 0 = collapse (up arrow); 1 = expanded (down arrow)
			this.FacilityMenuExpanded			= 0; // 0 = collapse (up arrow); 1 = expanded (down arrow)
			this.DeliveryLocationMenuExpanded	= 0; // 0 = collapse (up arrow); 1 = expanded (down arrow)
			this.TankMenuExpanded				= 0; // 0 = collapse (up arrow); 1 = expanded (down arrow)
			this.BreadcrumbMenuExpanded			= 0; // 0 = collapse (up arrow); 1 = expanded (down arrow)
		}
		#endregion
	}

	#region public class MapMenuItemClass
	/// <summary>
	/// This class is used for the java script to set
	/// values.
	/// </summary>
	[Serializable]
	public class MapMenuItemClass
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MapMenuItemClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string Text { get; set; }
		public string Value { get; set; }
		public bool Checked { get; set; }

		public int CheckedInt
		{
			get
			{
				return this.Checked ? 1 : 0;
			}
			set
			{
				this.Checked = value == 1;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Text		= string.Empty;
			this.Value		= string.Empty;
			this.Checked	= false;
		}
		#endregion
	}
	#endregion
}