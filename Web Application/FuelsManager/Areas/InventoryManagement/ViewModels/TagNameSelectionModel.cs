using System;
using System.Collections.Generic;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	[Serializable]
	public class TagNameSelectionModel
	{
		public List<KeyValuePair<string, string>> TagNames { get; set; }
		public TagNameSelectionModel()
		{
			this.TagNames = new List<KeyValuePair<string, string>>();

		}
	}
}