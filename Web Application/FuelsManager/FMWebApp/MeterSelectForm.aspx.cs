// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeterSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MeterSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;
    using FMCore;

    /// <summary>
    ///    A form which allows a user to select a list of available meters in the system.
    ///    Meters can be filtered by their ID, and can also be filtered by the asset to which they belong
    /// </summary>
    public partial class MeterSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		/// <summary>
		///    Contains information about how to display the results, such as whether we should show an "All" entry in the result list
		/// </summary>
		protected MeterSelectContextClass MeterSelectContext = null;

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
				this.MeterSelectContext.MeterIDFilterValue = string.Empty;
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
					this.MeterSelectContext.MeterIDFilterValue = string.Empty;
				}
				else
				{
					this.MeterSelectContext.MeterIDFilterValue = this.FindTextBox.Text.ToUpper();
				}

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    When the user clicks on the select button, call the javascript to return to the parent page
		/// </summary>
		/// <param name="source">not used</param>
		/// <param name="e">contains the row index the select button was pressed for</param>
		protected void MeterGrid_RowCommand(object source, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Select"))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					string meterID = this.MeterGrid.DataKeys[rowIndex].Values["ID"].ToString();

					//Get the ID and call the javascript function to return to the parent page
					this.ID = meterID;

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
		/// <param name="e">contains the item bound to the grid</param>
		protected void MeterGrid_RowDataBound(object sender, GridViewRowEventArgs e)
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
					this.MeterSelectContext = new MeterSelectContextClass();

					bool filterOnAsset = false;
					Guid meterAssetGuid = Guid.Empty;

					if (this.Session["MeterAssetGuid"] != null && this.Request.GetQueryOrFormValue("FilterOnAsset") != null
						 && Guid.TryParse(this.Session["MeterAssetGuid"].ToString(), out meterAssetGuid)
						 && Boolean.TryParse(this.Request.GetQueryOrFormValue("FilterOnAsset"), out filterOnAsset) && filterOnAsset)
					{
						this.MeterSelectContext.AssetGuid = meterAssetGuid;
					}
					else
					{
						this.MeterSelectContext.AssetGuid = Guid.Empty;
					}

					bool showAllSelection = false;

					if (this.Request.GetQueryOrFormValue("All") != null && Boolean.TryParse(this.Request.GetQueryOrFormValue("All"), out showAllSelection))
					{
						this.MeterSelectContext.All = showAllSelection;
					}
					else
					{
						this.MeterSelectContext.All = false;
					}

					bool showEmptyRow = false;

					if (this.Request.GetQueryOrFormValue("ShowEmptyRow") != null
						 && Boolean.TryParse(this.Request.GetQueryOrFormValue("ShowEmptyRow"), out showEmptyRow))
					{
						this.MeterSelectContext.ShowEmptyRow = showEmptyRow;
					}
					else
					{
						this.MeterSelectContext.ShowEmptyRow = false;
					}

					this.Session["MeterSelectContext"] = this.MeterSelectContext;

					this.UpdateView();
				}
				else
				{
					this.MeterSelectContext = this.Session["MeterSelectContext"] as MeterSelectContextClass;
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
			this.MeterGrid.RowDataBound += new GridViewRowEventHandler(this.MeterGrid_RowDataBound);
		}

		/// <summary>
		///    Refresh the screen with results
		/// </summary>
		private void UpdateView()
		{
			try
			{
				this.FindTextBox.Text = this.MeterSelectContext.MeterIDFilterValue;

				var meters = new List<MeterClass>();

				if (this.MeterSelectContext.AssetGuid != Guid.Empty
					 && !string.IsNullOrEmpty(this.MeterSelectContext.MeterIDFilterValue))
				{
					// Filter on Asset and ID
					meters = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
							x =>
							x.EnumerateByAssetGuidAndFilter(this.Security, this.MeterSelectContext.AssetGuid,
																		this.MeterSelectContext.MeterIDFilterValue)
					);
				}
				else if (this.MeterSelectContext.AssetGuid != Guid.Empty)
				{
					// Filter on Asset
					meters = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
																	 x =>
																	 x.EnumerateByAssetGuid(this.Security, this.MeterSelectContext.AssetGuid)
																);

				}
				else if (!string.IsNullOrEmpty(this.MeterSelectContext.MeterIDFilterValue))
				{
					// Filter on ID
					meters = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
																	 x =>
																	 x.EnumerateAndFilter(this.Security, this.MeterSelectContext.MeterIDFilterValue)
																);
				}
				else
				{
					// Show all meters
					meters = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
				}

				meters = meters.OrderBy(m=>m.ID).ToList();

				if (this.MeterSelectContext.All)
				{
					// If it is allowed, add an "All" selection
					var allMeters = new MeterClass();
					allMeters.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
					meters.Insert(0, allMeters);
				}

				if (this.MeterSelectContext.ShowEmptyRow)
				{
					// If it is allowed, add a blank entry to let the user deselect a meter
					var blankMeter = new MeterClass();
					blankMeter.ID = string.Empty;
					meters.Insert(0, blankMeter);
				}

				this.MeterGrid.DataSource = meters;
				this.MeterGrid.DataBind();

				// Provide Data Dictionary Translation for the "Select" column header label
				if (this.MeterGrid.HeaderRow != null)
				{
					if (this.MeterGrid.HeaderRow.Cells.Count > 0)
					{
						this.MeterGrid.HeaderRow.Cells[0].Text = this.GetTranslatedText("Select");
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
	///    Defines the criteria which can be used to limit the results on the meter select form
	/// </summary>
	[Serializable]
	public class MeterSelectContextClass
	{
		#region Constants and Fields

		/// <summary>
		///    Represents whether we should show an "All" entry in the result list
		/// </summary>
		public bool All = false;

		/// <summary>
		///    If provided, indicates an Asset (tank, equipment, load arm) to limit the meter results for. Only meters
		///    assigned to that asset will be shown
		/// </summary>
		public Guid AssetGuid = Guid.Empty;

		/// <summary>
		///    Represents the value typed into the Find box. We use it to filter Meters on their ID
		/// </summary>
		public string MeterIDFilterValue = string.Empty;

		/// <summary>
		///    Represents whether we should show a blank entry to the list. This allows a user to deselect a meter where one is not required,
		///    for example, from the transaction detail form
		/// </summary>
		public bool ShowEmptyRow = false;

		#endregion
	}
}