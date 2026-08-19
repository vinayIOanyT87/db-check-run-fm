namespace FuelsManager.Accounting
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Summary description for Import.
	/// </summary>
	public partial class Import : AccountingWebFormView
	{

		ImportExportListDO importDO;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
			    ImportSR sr = new ImportSR { Site = this.CurrentSiteGuid.ToString(), Security = this.security };

			    this.importDO = FMChannelHelper.MakeCall<IImportProcessor, ImportExportListDO>(x => x.Process(sr));

				if (this.IsPostBack == false)
				{
					this.ImportDropDown.DataSource = this.importDO.ImportExportList;
					this.ImportDropDown.DataTextField = "DisplayName";
					this.ImportDropDown.DataBind();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
			base.Initialize();
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void GoButtonClick(object sender, EventArgs e)
		{
			ImportExportListItemDO item =
				this.importDO.ImportExportList[this.ImportDropDown.SelectedIndex];

		    ImportExportPluginSR sr = new ImportExportPluginSR
		                              {
		                                  Site = this.CurrentSiteGuid.ToString(),
		                                  Security = this.security
		                              };

		    ImportExportPluginDO pluginDO = FMChannelHelper.MakeCall<IImportExportPluginProcessor, ImportExportPluginDO>(
															x =>
															x.Process(sr)
														);


			string url = "";
			foreach (ImportExportPluginItemDO plugin in pluginDO.PluginList)
			{
				if (plugin.PluginType == item.PluginType)
				{
					url = plugin.RunURL + "?Site=" + this.CurrentSiteGuid.ToString() + "&Name=" + item.DisplayName;
				}
			}
			if (url.Length > 0)
			{
				this.Redirect(url);
			}
			else
			{
				throw new Exception("Improperly configured Import.");
			}
		}
	}
}
