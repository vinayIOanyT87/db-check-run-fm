namespace FuelsManager.Accounting
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Summary description for ImportExportConfiguration.
	/// </summary>
	public partial class ImportExportConfiguration : AccountingWebFormView
	{
		protected ImportExportListDO ListDO;
		protected ImportExportPluginDO PluginDO;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Initialize();

				if (!this.IsPostBack)
				{
					this.ApplyDataDictionary();
				}

				this.DataGrid1.EditCommand += this.DataGrid1EditCommand;
				this.DataGrid1.DeleteCommand += this.DataGrid1DeleteCommand;
				this.DataGrid1.UpdateCommand += this.DataGrid1UpdateCommand;
				this.DataGrid1.CancelCommand += this.DataGrid1CancelCommand;

				//Get Plugin list
				{
					var sr = new ImportExportPluginSR { Site = this.CurrentSiteGuid.ToString(), Security = this.security };

					this.PluginDO = FMChannelHelper.MakeCall<IImportExportPluginProcessor, ImportExportPluginDO>(x => x.Process(sr));
				}

				this.ListDO = (ImportExportListDO)this.Session["AddList"];

				if (this.ListDO == null)
				{
					var sr = new ImportExportConfigSR
					{
						Site = this.CurrentSiteGuid.ToString(),
						Security = this.security
					};

					this.ListDO = FMChannelHelper.MakeCall<IImportExportConfigProcessor, ImportExportListDO>(x => x.Process(sr));

					if (this.IsPostBack == false)
					{
						this.UpdateDisplay();
					}
				}
				else
				{
					this.DataGrid1.EditItemIndex = this.ListDO.ImportExportList.Count - 1;
					this.DataGrid1.SelectedIndex = this.ListDO.ImportExportList.Count - 1;
				}

				this.SetAddButtonState(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ApplyDataDictionary()
		{
			// Determine if the data dictionary shall be used.  True indicates that it
			// shall be used and false otherwise.
			bool useDataDictionary = false;


			if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
			{
				useDataDictionary = true;
			}

			if (useDataDictionary)
			{
				FMChannelHelper.MakeCall<IDataDictionariesClass>(products => this.SetColumnHeaderText(products));

				this.AddButton.Text = FMChannelHelper.MakeCall<IDataDictionariesClass,string>(
					x =>
					x.Get(security.SiteGuid, "Add")
				);

			}
		}

		private void SetColumnHeaderText(IDataDictionariesClass dict)
		{
			foreach (DataGridColumn column in this.DataGrid1.Columns)
			{
				column.HeaderText = dict.Get(this.security.SiteGuid, column.HeaderText);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			base.OnInit(e);
		}
		#endregion

		protected void UpdateDisplay()
		{
			this.DataGrid1.DataSource = this.ListDO.ImportExportList;
			this.DataGrid1.DataBind();
		}

		protected int GetPluginTypeIndex(string pluginType)
		{
			foreach (ImportExportPluginItemDO plugin in this.PluginDO.PluginList)
			{
				if (plugin.PluginType == pluginType)
				{
					return this.PluginDO.PluginList.IndexOf(plugin);
				}
			}
			return -1;
		}

		protected void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				var item = new ImportExportListItemDO
				           {
					           Site = this.CurrentSiteGuid.ToString(),
					           DisplayName = string.Empty,
					           ExportAllowed = false,
					           ImportAllowed = false,
					           PluginType = "Standard XML",
					           LastExported = "never",
					           Configured = false
				           };

				this.ListDO.ImportExportList.Add(item);

				this.DataGrid1.EditItemIndex = this.ListDO.ImportExportList.Count - 1;
				this.DataGrid1.SelectedIndex = this.ListDO.ImportExportList.Count - 1;

				this.Session.Add("AddList", this.ListDO);
				this.UpdateDisplay();
				this.SetAddButtonState(false);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DataGrid1EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var dataGrid = (DataGrid)source;
				dataGrid.EditItemIndex = e.Item.DataSetIndex;

				this.UpdateDisplay();
				this.SetAddButtonState(false);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DataGrid1DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.ListDO.ImportExportList.RemoveAt(e.Item.ItemIndex);

				var sr = new ImportExportConfigSR
				         {
					         Site = this.CurrentSiteGuid.ToString(),
					         ImportExportList = this.ListDO,
					         Security = this.security
				         };

				FMChannelHelper.MakeCall<IImportExportConfigProcessor>(x => x.Process(sr));

				sr.ImportExportList = null;
				this.ListDO = FMChannelHelper.MakeCall<IImportExportConfigProcessor, ImportExportListDO>(x => x.Process(sr));

				this.UpdateDisplay();
				this.SetAddButtonState(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DataGrid1UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Session.Remove("AddList");

				var dataGrid = (DataGrid)source;
				ImportExportListItemDO item = this.ListDO.ImportExportList[dataGrid.EditItemIndex];

				var displayNameControl = (TextBox)e.Item.FindControl("DisplayName");
				item.DisplayName = displayNameControl.Text;

				var typeControl = (DropDownList)e.Item.FindControl("PluginTypeDropDown");
				item.PluginType = typeControl.SelectedValue;

				dataGrid.EditItemIndex = -1;

				var sr = new ImportExportConfigSR
				         {
					         Site = this.CurrentSiteGuid.ToString(),
					         ImportExportList = this.ListDO,
					         Security = this.security
				         };

				FMChannelHelper.MakeCall<IImportExportConfigProcessor>(x => x.Process(sr));

				item = this.ListDO.ImportExportList[e.Item.DataSetIndex];

				foreach (ImportExportPluginItemDO plugin in this.PluginDO.PluginList)
				{
					if (plugin.PluginType == item.PluginType)
					{
						string url = plugin.ConfigURL;
						var localSecurity = (SecurityClass)this.Page.Session["Security"];
						this.Redirect(url + "?Site=" + localSecurity.LoginSiteGuid + "&Name=" + item.DisplayName);
						return;
					}
				}

				this.UpdateDisplay();
				this.SetAddButtonState(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DataGrid1CancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var dataGrid = (DataGrid)source;

				// If we were adding a new item when Cancel was selected, we need to remove the new item
				if (this.Session["AddList"] != null)
				{
					ImportExportListItemDO item = this.ListDO.ImportExportList[dataGrid.EditItemIndex];
					this.ListDO.ImportExportList.Remove(item);
				}

				dataGrid.EditItemIndex = -1;
				this.Session.Remove("AddList");

				this.UpdateDisplay();
				this.SetAddButtonState(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SetAddButtonState(bool bDesiredState)
		{
			if (this.PluginDO.PluginList.Count <= 0)
			{
				this.AddButton.Enabled = false;
			}
			else
			{
				this.AddButton.Enabled = bDesiredState;
			}
		}
	}
}
