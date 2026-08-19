
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyEquipmentPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyEquipmentPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for CompanyEquipmentPage.
	/// </summary>
	public partial class CompanyEquipmentPage : CompanyPageBase
	{
		#region Constants and Fields

		protected EquipmentCollectionClass EquipmentCollection;

		#endregion

		#region Enums

		#endregion

		#region Properties

		private string javascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('CompanyEquipmentPage_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('CompanyEquipmentPage_UnassignButton');
					if(UnassignButton != null)
						UnassignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Unassign") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Public Methods and Operators


		public void UpdateData()
		{
		}

		#endregion

		#region Methods

		protected void AssignEquipmentTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] IDs = this.AssignEquipmentTextBox.Text.Split(new[] { '|' });
				this.AssignEquipmentTextBox.Text = "";


				foreach (string ID in IDs)
				{
					if (ID == "|")
					{
						continue;
					}

					EquipmentClass Equipment =
						FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	x =>
																	x.Get(this.Security, x.GetIdentityGuid(this.Security, ID))
															);

					this.Company.EquipmentCollection.Add(Equipment);
				}

				this.Company.EquipmentCollection.Sort();

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private CompanyClass GetCompanyClass(SecurityClass securityClass, Guid identityGuid)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(securityClass, identityGuid)
																);
		}

		private Guid GetIdentityGuid(SecurityClass securityClass, string ID)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(
								x =>
								x.GetIdentityGuid(this.Security, ID)
						);
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{

				if (this.Page.IsPostBack == false)
				{
					for (EQUIPMENT_TYPE Type=EQUIPMENT_TYPE.TRAILER_TYPE; Type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; Type++)
					{
						var Item = new ListItem(EquipmentTypeClass.TypeID(Type), Type.ToString());
						this.TypeDropDownList.Items.Add(Item);
						if (Type == EQUIPMENT_TYPE.AIRCRAFT_TYPE)
							this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
                        this.SetFieldAccessibilityForChildRecordVersion();
					}

					this.UpdateView();
				}

				this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "CompanyEquipmentPageScriptBlock", this.javascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		protected void UnassignEquipmentTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] IDs = this.UnassignEquipmentTextBox.Text.Split(new[] { '|' });
				this.UnassignEquipmentTextBox.Text = "";

				foreach (string ID in IDs)
				{
					if (ID == "|")
					{
						continue;
					}

					int Index = 0;
					foreach (EquipmentClass Equipment in this.Company.EquipmentCollection)
					{
						if (Equipment.ID == ID)
						{
							this.Company.EquipmentCollection.RemoveAt(Index);
							break;
						}
						Index++;
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AssignedEquipmentDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var IDLabel = (Label)e.Item.FindControl("IDLabel");

				EquipmentClass Equipment = this.EquipmentCollection[e.Item.DataSetIndex];
				IDLabel.Text = Equipment.ID;
				IDLabel.ToolTip = Equipment.EquipmentToolTip;
			}
		}

		private void AssignedEquipmentDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AssignedEquipmentDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.AssignedEquipmentDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignedEquipmentDataGrid.PageIndexChanged += this.AssignedEquipmentDataGrid_PageIndexChanged;
			this.AssignedEquipmentDataGrid.ItemDataBound += this.AssignedEquipmentDataGrid_ItemDataBound;
		}

		private void UpdateView()
		{
			if (this.TypeDropDownList.SelectedValue == "")
			{
				return;
			}

			EQUIPMENT_TYPE type;

			EQUIPMENT_TYPE.TryParse(this.TypeDropDownList.SelectedValue, out type);

			this.EquipmentCollection = new EquipmentCollectionClass();

			foreach (EquipmentClass Equipment in this.Company.EquipmentCollection)
			{
				if (type != Equipment.Type)
				{
					continue;
				}

				this.EquipmentCollection.Add(Equipment);
			}

			this.AssignedEquipmentDataGrid.DataSource = this.EquipmentCollection;

			int Count = (this.AssignedEquipmentDataGrid.DataSource as EquipmentCollectionClass).Count;

			if ((Count - 1) / this.AssignedEquipmentDataGrid.PageSize < this.AssignedEquipmentDataGrid.CurrentPageIndex)
			{
				this.AssignedEquipmentDataGrid.CurrentPageIndex = (Count - 1) / this.AssignedEquipmentDataGrid.PageSize;
			}

			this.AssignedEquipmentDataGrid.DataBind();
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }
            if ((!CompanyEquipmentPage_AssignButton.Disabled) && this.VersionSpecificFields.Contains("Equipment"))
                CompanyEquipmentPage_AssignButton.Disabled = false;
            else
                CompanyEquipmentPage_AssignButton.Disabled = true;
            if ((!CompanyEquipmentPage_UnassignButton.Disabled) && this.VersionSpecificFields.Contains("Equipment"))
                CompanyEquipmentPage_UnassignButton.Disabled = false;
            else
                CompanyEquipmentPage_UnassignButton.Disabled = true;
        }


		#endregion
	}
}