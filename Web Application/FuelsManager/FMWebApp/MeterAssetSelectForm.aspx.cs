// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeterAssetSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MeterAssetSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    using FMControls;
    using FMCore;

    /// <summary>
    ///    A form which allows a user to select a tank, equipment, or load arm that have meters assigned to them
    ///    Assets can be filtered by their ID
    /// </summary>
    public partial class MeterAssetSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		/// <summary>
		///    Contains information about how to display the results, such as whether we should show an "All" entry in the result list
		/// </summary>
		protected MeterAssetSelectContextClass MeterAssetSelectContext = null;

		#endregion

		#region Methods

		/// <summary>
		///    When the user presses the "Show All" button, reset the search filter values and refresh the results
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			try
			{
				this.MeterAssetSelectContext.AssetIDFilterValue = string.Empty;
				this.FindTextBox.Text = string.Empty;
				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    This fires when the user presses the Find button.
		///    We determine the value entered in the ID filter box and refresh the results
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(this.FindTextBox.Text))
				{
					this.MeterAssetSelectContext.AssetIDFilterValue = string.Empty;
				}
				else
				{
					this.MeterAssetSelectContext.AssetIDFilterValue = this.FindTextBox.Text.ToUpper();
				}

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    When the user clicks on the select button, save the guid of the selected asset in session
		/// </summary>
		/// <param name="source">not used</param>
		/// <param name="e">contains the row the select button was pressed for</param>
		protected void MeterAssetGrid_RowCommand(object source, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Select"))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);

					// save the Guid of the asset the user selected so we can use it later to filter meters
					if (this.MeterAssetGrid.DataKeys[rowIndex].Values["IdentityGuid"] != null)
					{
						this.Session["MeterAssetGuid"] =
							Guid.Parse(this.MeterAssetGrid.DataKeys[rowIndex].Values["IdentityGuid"].ToString());
					}
					else
					{
						this.Session["MeterAssetGuid"] = Guid.Empty;
					}

					string assetID = this.MeterAssetGrid.DataKeys[rowIndex].Values["ID"].ToString();

					//Get the ID and call the javascript function to return to the parent page
					this.ID = assetID;

					string toolTip = this.ID;

                    string selectString = "Select('" + HttpUtility.JavaScriptStringEncode(this.ID) + "','" + HttpUtility.JavaScriptStringEncode(toolTip) + "');";
					this.ClientScript.RegisterStartupScript(this.GetType(), "SelectScript", selectString, true);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    This fires when a result is bound to the grid.
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">contains the index of the item bound to the grid</param>
		protected void MeterAssetGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				var view = e.Row.FindControl("SelectButton") as FMSelectLinkButton;

				if (view != null)
				{
					view.CommandArgument = e.Row.RowIndex.ToString();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
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

		/// <summary>
		///    When the page loads, get some values from the request and session and refresh the view
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					this.MeterAssetSelectContext = new MeterAssetSelectContextClass();

					bool showAllSelection = false;

					if (this.Request.GetQueryOrFormValue("All") != null && Boolean.TryParse(this.Request.GetQueryOrFormValue("All"), out showAllSelection))
					{
						this.MeterAssetSelectContext.All = showAllSelection;
					}
					else
					{
						this.MeterAssetSelectContext.All = false;
					}

					this.Session["MeterAssetSelectContext"] = this.MeterAssetSelectContext;

					this.UpdateView();
				}
				else
				{
					this.MeterAssetSelectContext = this.Session["MeterAssetSelectContext"] as MeterAssetSelectContextClass;
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
			this.MeterAssetGrid.RowDataBound += new GridViewRowEventHandler(this.MeterAssetGrid_RowDataBound);
		}

		/// <summary>
		///    Refresh the screen with results
		/// </summary>
		private void UpdateView()
		{
			try
			{
				this.FindTextBox.Text = this.MeterAssetSelectContext.AssetIDFilterValue;

				var meterAssets = new List<MeterAssetClass>();

				if (!string.IsNullOrEmpty(this.MeterAssetSelectContext.AssetIDFilterValue))
				{
					//Filter on Asset ID
					meterAssets = FMChannelHelper.MakeCall<IMeters, List<MeterAssetClass>>(
																	 x =>
																	 x.EnumerateAssetsAndFilter(this.Security, this.MeterAssetSelectContext.AssetIDFilterValue)
																);
				}
				else
				{
					//Show all assets
					meterAssets = FMChannelHelper.MakeCall<IMeters, List<MeterAssetClass>>(
																	 x =>
																	 x.EnumerateAssets(this.Security)
																);
				}

				if (this.MeterAssetSelectContext.All)
				{
					//Add an "All" selection
					var allAssets = new MeterAssetClass();
					allAssets.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
					meterAssets.Insert(0, allAssets);
				}

				this.MeterAssetGrid.DataSource = meterAssets;
				this.MeterAssetGrid.DataBind();

				// Provide Data Dictionary Translation for the "Select" column header label
				if (this.MeterAssetGrid.HeaderRow != null)
				{
					if (this.MeterAssetGrid.HeaderRow.Cells.Count > 0)
					{
						this.MeterAssetGrid.HeaderRow.Cells[0].Text = this.GetTranslatedText("Select");
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#endregion
	}

	/// <summary>
	///    Defines the criteria which can be used to limit the results on the meter asset select form
	/// </summary>
	[Serializable]
	public class MeterAssetSelectContextClass
	{
		#region Constants and Fields

		/// <summary>
		///    Represents whether we should show an "All" entry in the result list
		/// </summary>
		public bool All = false;

		/// <summary>
		///    Represents the value typed into the Find box. We use it to filter Assets on their ID
		/// </summary>
		public string AssetIDFilterValue = string.Empty;

		#endregion
	}
}