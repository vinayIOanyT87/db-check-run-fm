// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyGroupGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyGroupGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
	/// Code behind for CompanyGroupGeneralPage.
	/// </summary>
	public partial class CompanyGroupGeneralPage : FMUserControlBase
	{
		#region Properties

		/// <summary>
		/// Gets the javascript startup.
		/// </summary>
		/// <value>
		/// The javascript startup.
		/// </value>
		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('CompanyGroupGeneralPage_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('CompanyGroupGeneralPage_UnassignButton');
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

		/// <summary>
		/// Updates the data.
		/// </summary>
		public void UpdateData()
		{
			var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

			companyGroup.ID = this.Name.Text;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Handles the TextChanged event of the AssignEntitiesTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void AssignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

				string[] ids = this.AssignEntitiesTextBox.Text.Split('|');
				this.AssignEntitiesTextBox.Text = string.Empty;

				FMChannelHelper.MakeCall<ICompanies>(
					companies =>
						{
							foreach (string id in ids)
							{
								if (id == "|")
								{
									continue;
								}

								CompanyClass company = companies.Get(this.Security, companies.GetIdentityGuid(this.Security, id), false);

                                CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP);
							    companyMap.AssignedID = company.ID;
							    companyMap.AssignedGuid = company.MasterRecordGuid;
							    companyMap.AssignedName = company.Name;
							    companyMap.AssignedAddress = company.Address1;
							    companyMap.AssignedCity = company.City;
							    companyMap.AssignedState = company.State;

								companyGroup.AssignedCompanyCollection.Add(companyMap);
							}
						});

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];
					this.Name.Text = companyGroup.ID;

					this.UpdateView();
				}

				this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "CompanyGroupGeneralPageScriptBlock", this.JavascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the TextChanged event of the UnassignEntitiesTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void UnassignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

				string[] ids = this.UnassignEntitiesTextBox.Text.Split('|');
				this.UnassignEntitiesTextBox.Text = string.Empty;

				foreach (string id in ids)
				{
					if (id == "|")
					{
						continue;
					}

					int index = 0;
					foreach (CompanyMapClass companyMap in companyGroup.AssignedCompanyCollection)
					{
						if (companyMap.AssignedID == id)
						{
							companyGroup.AssignedCompanyCollection.Remove(index);
							break;
						}

						index++;
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the ItemDataBound event of the AssignedEntitiesDataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridItemEventArgs" /> instance containing the event data.</param>
		private void AssignedEntitiesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

				var idLabel = (Label)e.Item.FindControl("IDLabel");

				CompanyMapClass companyMap = companyGroup.AssignedCompanyCollection[e.Item.DataSetIndex];
				idLabel.Text = companyMap.AssignedID;
				idLabel.ToolTip = companyMap.AssignedToolTip;
			}
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the AssignedEntitiesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridPageChangedEventArgs" /> instance containing the event data.</param>
		private void AssignedEntitiesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AssignedEntitiesDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AssignedEntitiesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignedEntitiesDataGrid.PageIndexChanged += this.AssignedEntitiesDataGridPageIndexChanged;
			this.AssignedEntitiesDataGrid.ItemDataBound += this.AssignedEntitiesDataGridItemDataBound;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

			companyGroup.AssignedCompanyCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNED);
			this.AssignedEntitiesDataGrid.DataSource = companyGroup.AssignedCompanyCollection;

            var collectionBase = this.AssignedEntitiesDataGrid.DataSource as List<CompanyMapClass>;
			if (collectionBase != null)
			{
				int count = collectionBase.Count;
				if ((count - 1) / this.AssignedEntitiesDataGrid.PageSize < this.AssignedEntitiesDataGrid.CurrentPageIndex)
				{
					this.AssignedEntitiesDataGrid.CurrentPageIndex = (count - 1) / this.AssignedEntitiesDataGrid.PageSize;
				}
			}

			this.AssignedEntitiesDataGrid.DataBind();
		}

		#endregion
	}
}