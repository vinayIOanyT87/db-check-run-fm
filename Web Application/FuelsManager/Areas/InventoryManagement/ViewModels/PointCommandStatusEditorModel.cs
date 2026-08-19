namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using FMBusinessObjects.DataObjects;
	using System.Collections.Generic;

	[Serializable]
	public class EditorPointCommandStatusEntry
	{
		public EditorPointCommandStatusEntry()
		{
		}

		public EditorPointCommandStatusEntry(string key, int value)
		{
			this.KeyEntry = key;
			this.ValueEntry = value;
		}

		public string KeyEntry { get; set; }

		public int ValueEntry { get; set; }
	}

	[Serializable]
	public class EditorPointCommandStatusList
	{
		public List<EditorPointCommandStatusEntry> PointCommandStatusEntries { get; set; }
		public string CommandStatusListID { get; set; }
		public Guid CommandStatusListGuid { get; set; }
		public EditorPointCommandStatusList()
		{
			this.PointCommandStatusEntries = new List<EditorPointCommandStatusEntry>();
		}
	}


	[Serializable]
	public class PointCommandStatusEditorModel
	{
		public Guid PointTemplateGuid { get; set; }
		public string PointTemplateId { get; set; }
		public PointCommandStatus PointCommandStatus { get; set; }
		public List<EditorPointCommandStatusList> EditorEntries { get; set; }
		public bool HasModifyRight;
		public SiteClass Site { get; set; }

		public PointCommandStatusEditorModel()
		{
			this.EditorEntries = new List<EditorPointCommandStatusList>();
			this.PointCommandStatus = new PointCommandStatus();
		}

		public PointCommandStatusEditorModel(PointTemplate pointTemplate, SiteClass site)
		{
			this.Site = site;
			this.PointCommandStatus = pointTemplate.PointCommandStatus;

			if (this.PointCommandStatus == null)
			{
				this.PointCommandStatus = new PointCommandStatus();
			}

			this.EditorEntries = new List<EditorPointCommandStatusList>();

			this.PointTemplateGuid = pointTemplate.IdentityGuid;
			this.PointTemplateId = pointTemplate.ID;

			for (int index = 0; index < this.PointCommandStatus.CommandStatusLists.Count; index++)
			{
				if (this.PointCommandStatus.CommandStatusLists[index] != null)
				{

					this.EditorEntries.Add(new EditorPointCommandStatusList());

					this.EditorEntries[index].CommandStatusListID = this.PointCommandStatus.CommandStatusLists[index].ID;
					this.EditorEntries[index].CommandStatusListGuid = this.PointCommandStatus.CommandStatusLists[index].CommandStatusListGuid;
					this.PointCommandStatus.CommandStatusLists[index].CommandStatusList.ForEach(
						s => this.EditorEntries[index].PointCommandStatusEntries.Add(new EditorPointCommandStatusEntry(s.Key, s.Value)));

				}
			}
		}
	}
}