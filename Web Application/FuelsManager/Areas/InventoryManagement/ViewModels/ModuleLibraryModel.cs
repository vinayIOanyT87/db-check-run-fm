using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using FMBusinessObjects.DataObjects;
	using System.Web.Mvc;

	[Serializable]
	public class ModuleLibraryEntry
	{
		public Guid SiteGuid;

		public string ModuleName;

		public bool Standard;

		public Guid ModuleGuid;

		public string ModuleScript;
	}

	[Serializable]
	public class ModuleLibraryModel
	{
		public Guid SiteGuid;

		public List<ModuleLibraryEntry> ModuleList;

		public bool ReadOnly;

		public ModuleLibraryModel()
		{
			this.ModuleList = new List<ModuleLibraryEntry>();
		}

		public MvcHtmlString GuideOpenerScript { get; set;}

		public ModuleLibraryModel(Guid siteGuid, Dictionary<Guid, Module> moduleDictionary)
		{
			this.SiteGuid = siteGuid;
			this.ModuleList = new List<ModuleLibraryEntry>();
			foreach (var module in moduleDictionary.Values)
			{
				this.ModuleList.Add(new ModuleLibraryEntry {SiteGuid = module.SiteGuid, ModuleName = module.ID, ModuleGuid = module.IdentityGuid, Standard = module.Standard, ModuleScript = module.ModuleScript});
			}
		}
	}
}
