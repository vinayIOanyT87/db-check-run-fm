// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductAdditivePage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductAdditivePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for ProductAdditivePage.
	/// </summary>
	public partial class ProductAdditivePage : ProductPageBase
	{
		#region Public Methods and Operators
		public void UpdateData()
		{
		}
		#endregion

		#region Methods
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
				if (this.Product.ProductType != ProductType.AdditiveProduct)
				{
					return;
				}

				if (! this.Page.IsPostBack)
				{
					this.UpdateAdditiveProfilesView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AdditiveProfilesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.AdditiveProfilesDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.AdditiveProfilesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateAdditiveProfilesView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		private ICollection EnumerateAdditiveProfiles()
		{
			ProductMapCollectionClass additiveProfileMaps = null;

			if (this.Product.IdentityGuid != Guid.Empty)
			{
				additiveProfileMaps = FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(this.Security, this.Product.MasterRecordGuid, PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
																);
			}

			var additiveProfileDataTable = new DataTable();

			additiveProfileDataTable.Columns.Add("Index", typeof(Int32));
			additiveProfileDataTable.Columns.Add("ID", typeof(string));
			additiveProfileDataTable.Columns.Add("Rate", typeof(string));
			additiveProfileDataTable.Columns.Add("CycleVolume", typeof(string));
			additiveProfileDataTable.Columns.Add("Tolerance", typeof(string));

			if (additiveProfileMaps != null)
			{
				for (int iItem = 0; iItem < additiveProfileMaps.Count; iItem++)
				{
					DataRow additiveProfileDataRow = additiveProfileDataTable.NewRow();

					ProductMapClass additiveProfileMap = additiveProfileMaps[iItem];
					additiveProfileDataRow["Index"] = iItem;
					additiveProfileDataRow["ID"] = additiveProfileMap.AssignedToID;
					additiveProfileDataRow["Rate"] = additiveProfileMap.AdditiveRate;
					additiveProfileDataRow["CycleVolume"] = additiveProfileMap.AdditiveCycleVolume;
					additiveProfileDataRow["Tolerance"] = additiveProfileMap.Tolerance;

					additiveProfileDataTable.Rows.Add(additiveProfileDataRow);
				}
			}

			var additiveProfileDataView = new DataView(additiveProfileDataTable);
			return additiveProfileDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// if we are editing do not allow a page change
			if (this.AdditiveProfilesDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AdditiveProfilesDataGrid.PageIndexChanged += this.AdditiveProfilesDataGridPageIndexChanged;
		}

		private void UpdateAdditiveProfilesView()
		{
			this.AdditiveProfilesDataGrid.DataSource = this.EnumerateAdditiveProfiles();
			this.AdditiveProfilesDataGrid.DataBind();
		}
		#endregion
	}
}