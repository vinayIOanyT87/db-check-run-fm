using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class DropdownItemCollectionClass : List<DropdownItem> { }

	/// <summary>
	/// Summary description for DropdownItem.
	/// </summary>
	[DataContract]
   [Serializable]
	public class DropdownItem
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the dropdown item class.
		/// </summary>
		public DropdownItem ( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the display text for a dropdown list
		/// control.
		/// </summary>
		[DataMember] public string Text { get; set; }

		/// <summary>
		/// This property sets and gets the text value of a dropdown list
		/// control.
		/// </summary>
		[DataMember] public string TextValue { get; set; }

		#endregion
	}
}
