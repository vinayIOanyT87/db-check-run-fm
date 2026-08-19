namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	[Serializable]
	public class DrawPropertyMenuRecord
	{
		#region Public data members

		public enum PropertyControlTypes
		{
			Textbox, Dropdown, Divider, FillColor, FillColorSpectrum, FillPattern, FillPatternPalette, TextboxWithButton, ButtonOnly, None
		}

		public enum PropertyControlSubTypes { FillPatternType, LineStyleType, LineArrowType }

		public const string SectionNameSectionControl	= "Section-Controls";
		public const string SectionNameSectionMain		= "Section-MainProperties";
		public const string SectionNameSectionFillColor = "Section-FillColor";
		public const string SectionNameSectionLine		= "Section-LineProps";
		public const string SectionNameSectionText		= "Section-TextProps";
		#endregion

		#region Private data members
		private const string TableRowPrefixId					= "tr-propertiesMenu-";
		private const string TextboxPrefixId					= "textbox-propertiesMenu-";
        private const string LabelPrefixId                      = "label-propertiesMenu-";
        private const string DropdownPrefixId					= "dropdown-propertiesMenu-";
		private const string FillColorPrefixId					= "fillColor-propertiesMenu-";
		private const string FillColorImagePrefixId				= "fillColorImage-propertiesMenu-";
		private const string FillColorSpectrumPrefixId			= "fillColorSpectrum-propertiesMenu-";
		private const string RecentColorTextboxPrefixId			= "recentColor-textbox-propertiesMenu-";
		private const string ManualColorTextboxPrefixId			= "manualColor-textbox-propertiesMenu-";
		private const string ManualColorSamplerTextboxPrefixId	= "manualColorSampler-textbox-propertiesMenu-";
		private const string FillPatternPrefixId				= "fillPattern-propertiesMenu-";
		private const string FillPatternImagePrefixId			= "fillPatternImage-propertiesMenu-";
		private const string FillPatternPalettePrefixId			= "fillPatternPalette-propertiesMenu-";
		private const string FillPatternPaletteScrollPrefixId	= "FillPatternPalette-scroll-";
		private const string CanvasPrefixId						= "canvas-propertiesMenu-";
		private const string DividerPrefixId					= "divider-propertiesMenu-";
		private const string ButtonPrefixId						= "button-propertiesMenu-";
		private const string SectionCollapseImagePrefixId		= "propertryMenu-SectionCollapseImage-";

		private string propertyName;
		private string sectionName;
		private string sectionLabelName;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public DrawPropertyMenuRecord()
		{
			this.Init();
		}
		#endregion

		#region Properties

		public string PropertyName
		{
			get { return this.propertyName; }
			set
			{
				this.propertyName = value;
				this.SetRecentColorIDs();
			}
		}
		public string PropertyLabelName { get; set; }
        public string AlternateLabelName { get; set; }
		public PropertyControlTypes ControlType { get; set; }
		public PropertyControlSubTypes ControlSubType { get; set; }
		public string CanvasId => CanvasPrefixId + this.PropertyName;
		public string TableRowId => TableRowPrefixId + this.PropertyName;
		public string TextBoxId => TextboxPrefixId + this.PropertyName;
	    public string LabelId => LabelPrefixId + this.PropertyName;
        public string DropdownId => DropdownPrefixId + this.PropertyName;
		public string FillColorId => FillColorPrefixId + this.PropertyName;
		public string FillColorImageId => FillColorImagePrefixId + this.PropertyName;
		public string FillColorHex { get; set; }
		public string FillColorSpectrumColumnId => "td-" + FillColorSpectrumPrefixId + this.PropertyName;
		public string SpectrumTextBoxId => "textbox-" + FillColorSpectrumPrefixId + this.PropertyName;
		public string FillPatternId => FillPatternPrefixId + this.PropertyName;
		public string FillPatternImageId => FillPatternImagePrefixId + this.PropertyName;
		public string FillPatternPaletteColumnId => "td-" + FillPatternPalettePrefixId + this.PropertyName;
		public string FillPatternPaletteScrollId => FillPatternPaletteScrollPrefixId + this.PropertyName;
		public List<DrawPropertyMenuPattern> PatternList { get; set; }
		public string ManualColorTextBoxId => ManualColorTextboxPrefixId + this.PropertyName;
		public string ManualColorSamplerTextBoxId => ManualColorSamplerTextboxPrefixId + this.PropertyName;
		public string DividerId => DividerPrefixId + this.propertyName;
		public List<string> RecentColorTextBoxIds { get; private set; }
		public bool Readonly { get; set; }
		public bool Hide { get; set; }
		public string ButtonActionValue { get; set; }
		public string ButtonId => ButtonPrefixId + this.PropertyName;
		public string SectionCollapseImageId => SectionCollapseImagePrefixId + this.sectionName;

		public string SectionNameWithAttr
		{
			get
			{
				if (this.sectionName == string.Empty)
				{
					return string.Empty;
				}

				string sectionNameWithAttr = " DividerSectionName=" + this.sectionName + " ";
				return sectionNameWithAttr;
			}
		}

		public string SectionName
		{
			get
			{
				return this.sectionName;
			}
			set
			{
				this.sectionName = string.Empty;

				if (value != string.Empty)
				{
					this.sectionName = value;
				}
			}
		}

		public string SectionLabelName
		{
			get
			{
				return this.sectionLabelName;
			}
			set
			{
				this.sectionLabelName = value;
			}
		}

		public string HideStr
		{
			get
			{
				if (this.Hide)
				{
					return "none";
				}

				return "block";
			}
		}

		public string HideTableColumnStr
		{
			get
			{
				if (this.Hide)
				{
					return "none";
				}

				return "table-cell";
			}
		}

		public string TextboxReadonly
		{
			get
			{
				if (this.Readonly)
				{
					return " readonly='readonly' ";
				}

				return String.Empty;
			}
		}
		public string TextboxDisabled
		{
			get
			{
				if (this.Readonly)
				{
					return " disabled='disabled' ";
				}

				return String.Empty;
			}
		}

		public List<DrawPropertyMenuDropdown> DropdownItems { get; private set; }
		public string DropdownWidth { get; set; }
		public string TextboxWidth { get; set; }
		public string ButtonText { get; set; }
		public string ButtonWidth { get; set; }
		#endregion

		#region Public methods
		/// <summary>
		/// This method will clear the dropdown list.
		/// </summary>
		public void ClearDropdown()
		{
			if (this.DropdownItems.Count > 0)
			{
				this.DropdownItems.Clear();
			}
		}

		/// <summary>
		/// This method will add a dropdown item to the list.
		/// </summary>
		/// <param name="item">Dropdown item to be added.</param>
		public void DropdownAdd(DrawPropertyMenuDropdown item)
		{
			if (item != null)
			{
				this.DropdownItems.Add(item);
			}
		}

		/// <summary>
		/// This method will insert an item into the dropdown list.
		/// </summary>
		/// <param name="item">Item to insert.</param>
		/// <param name="index">Position to insert.</param>
		public void DropdownInsertAt(DrawPropertyMenuDropdown item, int index)
		{
			if (item != null && index >= 0 && index < this.DropdownItems.Count)
			{
				this.DropdownItems.Insert(index, item);
			}
		}

		/// <summary>
		/// This method will sort the list.
		/// </summary>
		public void Sort()
		{
			this.DropdownItems = this.DropdownItems.OrderBy(item => item.Text).ToList();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the objects to its initial state.
		/// </summary>
		private void Init()
		{
			this.RecentColorTextBoxIds	= new List<string>();
			this.PatternList			= new List<DrawPropertyMenuPattern>();
			this.PropertyName			= "unknown";
			this.PropertyLabelName		= "Unknown";
			this.ControlType			= PropertyControlTypes.None;
			this.Hide					= true;
			this.DropdownItems			= new List<DrawPropertyMenuDropdown>();
			this.DropdownWidth			= "50px";
			this.TextboxWidth			= "80px";
			this.sectionName			= string.Empty;
			this.ButtonText				= "button label";
			this.ButtonWidth			= "80px";
		}

		/// <summary>
		/// This method will create the recent color text box IDs.
		/// </summary>
		private void SetRecentColorIDs()
		{
			this.RecentColorTextBoxIds.Clear();

			for (int nextItem = 1; nextItem < 9; nextItem++)
			{
				string recentColorId = RecentColorTextboxPrefixId + this.propertyName + "-" + nextItem;
				this.RecentColorTextBoxIds.Add(recentColorId);
			}
		}
		#endregion
	}

	#region Draw Property Menu Dropdown Class
	[Serializable]
	public class DrawPropertyMenuDropdown
	{
		#region Constructors
		/// <summary>
		/// This is the default contructor.
		/// </summary>
		public DrawPropertyMenuDropdown()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string Text { get; set; }
		public string DataValueAttribute { get; set; }
		public string ValueAttribute { get; set; }
		public bool Selected { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Text				= string.Empty;
			this.DataValueAttribute = string.Empty;
			this.ValueAttribute		= string.Empty;
			this.Selected			= false;
		}
		#endregion
	}
	#endregion

	#region Draw Property Menu Patterns Class
	[Serializable]
	public class DrawPropertyMenuPattern
	{
		#region Private data members
		public const string CanvasPalettePrefixId = "canvasPalatte-propertiesMenu-";
		public const string ImagePalettePrefixId = "imagePalatte-propertiesMenu-";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default contructor.
		/// </summary>
		public DrawPropertyMenuPattern()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string CanvasTagId { get; set; }
		public int PatternNumber { get; set; }
		public string PatternNumberStr => this.PatternNumber.ToString();

		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.CanvasTagId = string.Empty;
			this.PatternNumber = 1;
		}
		#endregion
	}
	#endregion
}