namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMCore;

	using global::FMWebApp;

	/// <summary>
	/// Allows a user to choose a product
	/// </summary>
	public partial class AssetTrackingDeviceSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		private AssetTrackingDeviceSelectContextClass deviceSelectContext;
		private string selectThisItemText;
		private const string SessionDeviceSelectContext		= "AssetTrackingDeviceSelectForm.SessionDeviceSelectContext";
		private const string TargetDeviceConfigurationPage	= "../AssetTrackingArea/AssetDeviceConfiguration/DeviceConfiguration?deviceConfigurationGuid=";
		private const string NavigateToMvcPopup				= "../FMWebApp/MvcPopupContainer.aspx?target=";
		#endregion

		#region Methods
		/// <summary>
		/// This method handles the find all button on click event. It will find
		/// the device ID in the collection.
		/// </summary>
		/// <param name="sender">The calling module.</param>
		/// <param name="e">The event arguments.</param>
		protected void FindAllBtnOnClick(object sender, EventArgs e)
		{
			this.deviceSelectContext.SearchString = null;
			this.FindTextBox.Text = string.Empty;
			this.UpdateView();
		}

		/// <summary>
		/// This method handles the find button on click event. It will find
		/// the device ID in the collection.
		/// </summary>
		/// <param name="sender">The calling module.</param>
		/// <param name="e">The event arguments.</param>
		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.FindTextBox.Text))
			{
				this.deviceSelectContext.SearchString = null;
			}
			else
			{
				this.deviceSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
			}

			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// This method handles the page load event.
		/// </summary>
		/// <param name="sender">The calling module.</param>
		/// <param name="e">The event arguments.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.selectThisItemText = this.GetTranslatedText("Select this item");

				if (this.Page.IsPostBack == false)
				{
					this.deviceSelectContext = new AssetTrackingDeviceSelectContextClass();

					if (this.Request.GetQueryOrFormValue("All") != null)
					{
						this.deviceSelectContext.All = Convert.ToBoolean(this.Request.GetQueryOrFormValue("All"));
					}

					if (this.Request.GetQueryOrFormValue("Null") != null)
					{
						this.deviceSelectContext.Null = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Null"));
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
						this.deviceSelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
					}

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.deviceSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) || this.deviceSelectContext.Mode == "Unassign")
					{
						this.AddButton1.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Request.GetQueryOrFormValue("SearchString") != null)
					{
						this.deviceSelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
						this.FindTextBox.Text = this.deviceSelectContext.SearchString;
					}

					this.Session[SessionDeviceSelectContext] = this.deviceSelectContext;
					this.UpdateView();
				}
				else
				{
					this.deviceSelectContext = this.Session[SessionDeviceSelectContext] as AssetTrackingDeviceSelectContextClass;

					// Determine action
					bool btnPressed = (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("FindBtn")) == false
									   || string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("ShowAllBtn")) == false
									   || string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("AddButton1")) == false
									   || string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("AddButton2")) == false);

					if (btnPressed == false)
					{
						// default action is find
						this.FindBtnOnClick(sender, e);
					}
				}

				if (this.deviceSelectContext.Mode != null)
				{
					var findControl = (HtmlForm)this.FindControl("DeviceSelectForm");

					var okButton = new HtmlInputButton();
					okButton.Attributes.Add("value", this.GetTranslatedText("OK"));
					okButton.Attributes.Add("id", "OkButton");
					okButton.Attributes.Add("class", "formfieldtitle");
					okButton.Attributes.Add("onclick", "MultipleSelect()");
					okButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");

					findControl.Controls.Add(okButton);

					var cancelButton = new HtmlInputButton();
					cancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
					cancelButton.Attributes.Add("id", "CancelButton");
					cancelButton.Attributes.Add("class", "formfieldtitle");
					cancelButton.Attributes.Add("onclick", "NoSelect()");
					cancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");

					findControl.Controls.Add(cancelButton);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will add a new asset tracking device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			if (this.deviceSelectContext == null)
			{
				this.deviceSelectContext = this.Page.Session[SessionDeviceSelectContext] as AssetTrackingDeviceSelectContextClass
				                           ?? new AssetTrackingDeviceSelectContextClass();
			}

			this.deviceSelectContext.EditType = "ADD";
			this.deviceSelectContext.CallingPage = "SelectionPage";
			this.Page.Session[SessionDeviceSelectContext] = this.deviceSelectContext;
			this.Redirect(NavigateToMvcPopup + TargetDeviceConfigurationPage + Guid.Empty);
		}

		/// <summary>
		/// This method will remove all asset tracking devices that do not match the find string.
		/// </summary>
		/// <param name="deviceCollection">The collection to filter.</param>
		private List<AssetTrackingDeviceClass> FilterOnFind(List<AssetTrackingDeviceClass> deviceCollection)
		{
			if (deviceCollection == null)
			{
				return new List<AssetTrackingDeviceClass>();
			}

			if (string.IsNullOrEmpty(this.deviceSelectContext.SearchString) == false)
			{
				var filteredDeviceCollection = new List<AssetTrackingDeviceClass>();

				foreach (AssetTrackingDeviceClass device in deviceCollection)
				{
					if (device.DeviceId.ToUpper().Equals(this.deviceSelectContext.SearchString))
					{
						filteredDeviceCollection.Add(device);
					}
				}

				return filteredDeviceCollection;
			}

			return deviceCollection;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command						+= this.AddButtonCommand;
			this.AssetTrackDeviceDataGrid.EditCommand	+= this.AssetTrackingDeviceDataGridEditCommand;
			this.AssetTrackDeviceDataGrid.DeleteCommand += this.AssetTrackingDeviceDataGridDeleteCommand;
			this.AssetTrackDeviceDataGrid.ItemDataBound += this.AssetTrackingDeviveDataGridItemDataBound;
			this.AddButton1.Command						+= this.AddButtonCommand;
		}

		/// <summary>
		/// This method will handle the delete event. It will delete an asset tracking device.
		/// </summary>
		/// <param name="source">The calling module.</param>
		/// <param name="e">The event arguments</param>
		private void AssetTrackingDeviceDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get identity guid
				TableCell identityGuidCell = e.Item.Cells[3];//bds
				FMChannelHelper.MakeCall<IAssetTrackingDevices>(
																x =>
																x.Purge(this.Security, Guid.Parse(identityGuidCell.Text))
																);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will handle the Edit Command event.
		/// </summary>
		/// <param name="source">The calling grid.</param>
		/// <param name="e">The event arguments.</param>
		private void AssetTrackingDeviceDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			Guid editGuid;

			try
			{
				if (this.deviceSelectContext == null)
				{
					this.deviceSelectContext = this.Page.Session[SessionDeviceSelectContext] as AssetTrackingDeviceSelectContextClass
											   ?? new AssetTrackingDeviceSelectContextClass();
				}

				TableCell identityGuidCell = e.Item.Cells[3];//bds

				this.deviceSelectContext.EditType = "EDIT";
				this.deviceSelectContext.CallingPage = "SelectionPage";
				this.deviceSelectContext.DeviceGuidToEdit = Guid.Parse(identityGuidCell.Text);
				editGuid = this.deviceSelectContext.DeviceGuidToEdit;

				this.Page.Session[SessionDeviceSelectContext] = this.deviceSelectContext;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect(NavigateToMvcPopup + TargetDeviceConfigurationPage + editGuid);
		}

		/// <summary>
		///    This method create all the links for the asset tracking device list and places them
		///    on the client side.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetTrackingDeviveDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
					if (this.deviceSelectContext.Mode != null)
					{
						e.Item.Cells[0].Text = this.GetTranslatedText(this.deviceSelectContext.Mode);
					}
					else
					{
						e.Item.Cells[0].Text = this.GetTranslatedText("Select");
					}
				}
			}
			else
			{
				if (this.deviceSelectContext.Mode != null)
				{
					var select = new HtmlInputCheckBox();
					select.ID = "Select";
					e.Item.Cells[0].Controls.Add(select);

					e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(" ", "&nbsp;");//bds
				}
				else
				{
					string deviceId = string.Empty;

					// Leave hard space zero length string
					if (e.Item.Cells[4].Text != "&nbsp;")//bds
					{
						deviceId = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
					}

					string toolTip = ((e.Item.Cells[5].Text != "&nbsp;") ? e.Item.Cells[5].Text + ", " : "")//bds
									 + ((e.Item.Cells[6].Text != "&nbsp;") ? e.Item.Cells[6].Text + ", " : "");//bds

					var select = new HtmlAnchor();
					select.ID = "Select";
					select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(deviceId) + "','" + HttpUtility.JavaScriptStringEncode(toolTip) + "')");
					select.InnerHtml = "<img src=\"../FMWebApp/Images/Select.gif\" border=\"0\" align=\"absmiddle\" alt='"
									   + HttpUtility.HtmlEncode(this.selectThisItemText) + "'>";

					e.Item.Cells[0].Controls.Add(select);
				}

				var siteGuid				= Guid.Parse(e.Item.Cells[2].Text);//bds
				var assetTrackingDeviceGuid = Guid.Parse(e.Item.Cells[3].Text);//bds
				var deleteButton			= (LinkButton)e.Item.FindControl("DeleteLinkBtn");
				var editButton				= (LinkButton)e.Item.FindControl("EditLinkBtn");

				if (deleteButton != null)
				{
					deleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) && this.Security.SiteGuid == siteGuid
					                        && assetTrackingDeviceGuid != Guid.Empty && this.deviceSelectContext.Mode != "Unassign");
				}

				if (editButton != null)
				{
					editButton.Enabled = ((this.deviceSelectContext.Mode != "Unassign") && (assetTrackingDeviceGuid != Guid.Empty)
					                      && (this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES)
					                          || this.Security.HasRight(RIGHT.VIEW_MAPS)));
				}
			}
		}

		/// <summary>
		/// This method will update the asset tracking device selection form.
		/// </summary>
		private void UpdateView()
		{
			this.FindTextBox.Text = this.deviceSelectContext.SearchString;

			var assetTrackingDeviceCollection = 
					FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(x => x.EnumerateAllUnassignedActiveDevices(this.Security));

			// Filter the list based on the find search text.
			List<AssetTrackingDeviceClass> filterDeviceCollection = this.FilterOnFind(assetTrackingDeviceCollection);

			if (this.deviceSelectContext.Unassigned)
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass
				                          {
					                          DeviceId = HttpUtility.HtmlEncode( this.GetTranslatedText("{Unassigned}"))
				                          };

				filterDeviceCollection.Insert(0, assetTrackingDevice);
			}

			var deviceDataTable = new DataTable();
			deviceDataTable.Columns.Add("SiteGuid", typeof(Guid));
			deviceDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			deviceDataTable.Columns.Add("DeviceId", typeof(string));
			deviceDataTable.Columns.Add("Description", typeof(string));
			deviceDataTable.Columns.Add("ModelNumber", typeof(string));
			deviceDataTable.Columns.Add("SerialNumber", typeof(string));
			deviceDataTable.Columns.Add("Active", typeof(string));

			foreach (AssetTrackingDeviceClass device in filterDeviceCollection)
			{
				var deviceDataRow = deviceDataTable.NewRow();

				deviceDataRow["SiteGuid"]		= device.SiteGuid;
				deviceDataRow["IdentityGuid"]	= device.AssetTrackingDeviceGuid;
				deviceDataRow["DeviceId"]		= device.DeviceId;
				deviceDataRow["Description"]	= device.Description;
				deviceDataRow["ModelNumber"]	= device.ModelNumber;
				deviceDataRow["SerialNumber"]	= device.SerialNumber;
				deviceDataRow["Active"]			= device.Active ? "Active" : "Inactive";

				deviceDataTable.Rows.Add(deviceDataRow);
			}

			this.AssetTrackDeviceDataGrid.DataSource = new DataView(deviceDataTable);
			this.AssetTrackDeviceDataGrid.DataBind();
		}
		#endregion
	}

	[Serializable]
	public class AssetTrackingDeviceSelectContextClass
	{
		#region Constants and Fields
		public bool All					= false;
		public string Mode				= null;
		public bool Null				= false;
		public string SearchString		= null;
		public bool Unassigned			= false;
		public Guid DeviceGuidToEdit	= Guid.Empty;
		public string EditType			= string.Empty;
		public string CallingPage		= string.Empty;
		#endregion
	}

}