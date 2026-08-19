// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupCompaniesPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupCompaniesPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FMControls;

    /// <summary>
	///    Summary description for GroupCompaniesPage.
	/// </summary>
	public partial class GroupCompaniesPage : FMUserControlBase
	{
		#region Constants and Fields

		protected FMLabel Label5;

		#endregion

		#region Properties

		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('GroupCompaniesPage_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('GroupCompaniesPage_UnassignButton');
					if(UnassignButton != null)
						UnassignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Unassign") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion


		#region Methods

		protected void AssignCompaniesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				var group = this.Session[GroupForm.SESSION_KEY_GROUP] as GroupClass;

				string[] companyIDs = this.AssignCompaniesTextBox.Text.Split('|');
				this.AssignCompaniesTextBox.Text = "";

				// First remove {All} from the target collection
				this.RemoveAllCompanyMap(group);

				foreach (string companyID in companyIDs)
				{
					// Skip separators
					if (companyID != "|" && companyID != "")
					{
						var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);

						// Decode the string coming back from the client side
						string sCompanyID = this.Server.HtmlDecode(companyID);
						sCompanyID = sCompanyID.Replace((char)0xa0, ' '); // HTML decode leaves non-breaking spaces in the strings, which are distinct from space to the database.  Convert to regular spaces

						if (sCompanyID == "{All}")
						{
							companyMap.AssignedID = "{All}";
							group?.CompanyMapCollection.Clear();
							group?.CompanyMapCollection.Add(companyMap);

							// Stop processing since we only want to have the {All} option in the list
							break;
						}

						CompanyClass carrier =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								companies => companies.Get(this.Security, companies.GetIdentityGuid(this.Security, sCompanyID), false));

						companyMap.AssignedID = carrier.ID;
						companyMap.AssignedGuid = carrier.MasterRecordGuid;
						companyMap.AssignedName = carrier.Name;
						companyMap.AssignedAddress = carrier.Address1;
						companyMap.AssignedCity = carrier.City;
						companyMap.AssignedState = carrier.State;

						this.InsertInSortedOrder(group, companyMap);
					}
				}

				this.UpdateAssignedCompaniesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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
				if (this.IsPostBack == false)
				{
					this.AssignedCompaniesDataGrid.CurrentPageIndex = 0;
					this.UpdateAssignedCompaniesView();
				}

				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "GroupCompaniesScriptBlock", this.JavascriptStartup);

				// Disable a the controls and buttons on the page
				this.DisablePageControls();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will disable all the fields on the General page based on the 
		/// GroupForm.DisableAllControls flag.
		/// </summary>
		private void DisablePageControls()
		{
			if (GroupForm.DisableAllControls)
			{
				this.DisableButtonFlag.Value = "TRUE";
			}
		}

		protected void UnassignCompaniesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
            GroupClass group = this.Session[GroupForm.SESSION_KEY_GROUP] as GroupClass;

				string[] companyIDs = this.UnassignCompaniesTextBox.Text.Split('|');
				this.UnassignCompaniesTextBox.Text = "";

				foreach (string companyID in companyIDs)
				{
					if (companyID != "|")
					{
						// Decode the string coming back from the client side
						string sCompanyID = this.Server.HtmlDecode(companyID);
						sCompanyID = sCompanyID.Replace((char)0xa0, ' '); // HTML decode leaves non-breaking spaces in the strings, which are distinct from space to the database.  Convert to regular spaces
						int index = 0;
					    if (group != null)
					    {
					        foreach (CompanyMapClass companyMap in group.CompanyMapCollection)
					        {
					            if (companyMap.AssignedID == sCompanyID)
					            {
					                group.CompanyMapCollection.Remove(index);
					                break;
					            }

					            ++index;
					        }
					    }
					}
				}

				this.UpdateAssignedCompaniesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AssignedCompaniesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.AssignedCompaniesDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.AssignedCompaniesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateAssignedCompaniesView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		private void AuthorizedCompaniesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex != -1)
				{
               GroupClass group = this.Session[GroupForm.SESSION_KEY_GROUP] as GroupClass;

					if (group != null)
					{
						var idLabel = e.Item.FindControl("IDLabel") as Label;

						if (idLabel != null)
						{
							int index = (this.AssignedCompaniesDataGrid.PageSize * this.AssignedCompaniesDataGrid.CurrentPageIndex)
							            + e.Item.ItemIndex;

							CompanyMapClass companyMap = group.CompanyMapCollection[index];

							if (companyMap != null)
							{
								idLabel.Text = this.Server.HtmlEncode(companyMap.AssignedID);
								idLabel.ToolTip = companyMap.AssignedToolTip;
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignedCompaniesDataGrid.PageIndexChanged += this.AssignedCompaniesDataGridPageIndexChanged;
			this.AssignedCompaniesDataGrid.ItemDataBound += this.AuthorizedCompaniesDataGridItemDataBound;
		}

		private void InsertInSortedOrder(GroupClass groupClass, CompanyMapClass newCompanyMap)
		{
			int index = 0;

			foreach (CompanyMapClass companyMap in groupClass.CompanyMapCollection)
			{
				if (string.Compare(companyMap.AssignedID, newCompanyMap.AssignedID, StringComparison.Ordinal) > 0)
				{
					groupClass.CompanyMapCollection.Insert(index, newCompanyMap);
					return;
				}

				++index;
			}

			groupClass.CompanyMapCollection.Add(newCompanyMap);
		}

		private void RemoveAllCompanyMap(GroupClass group)
		{
			for (int nLoop = 0; nLoop < group.CompanyMapCollection.Count; ++nLoop)
			{
				if (group.CompanyMapCollection[nLoop].AssignedID == "{All}")
				{
					group.CompanyMapCollection.Clear();
					break;
				}
			}
		}

		private void UpdateAssignedCompaniesView()
		{
         GroupClass group = this.Session[GroupForm.SESSION_KEY_GROUP] as GroupClass;

			if (group != null)
			{
				this.AssignedCompaniesDataGrid.DataSource = group.CompanyMapCollection;

				int count = group.CompanyMapCollection.Count;
				if ((count - 1) / this.AssignedCompaniesDataGrid.PageSize < this.AssignedCompaniesDataGrid.CurrentPageIndex)
				{
					this.AssignedCompaniesDataGrid.CurrentPageIndex = (count - 1) / this.AssignedCompaniesDataGrid.PageSize;
				}

				this.AssignedCompaniesDataGrid.DataBind();
			}
		}

		#endregion
	}
}