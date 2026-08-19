// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductComponentPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductComponentPage type.
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
	///    Summary description for ProductBlendPage.
	/// </summary>
	public partial class ProductComponentPage : ProductPageBase
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
				if (this.Product.ProductType != ProductType.ComponentProduct)
				{
					return;
				}

				if (! this.Page.IsPostBack)
				{
					this.UpdateBlendsView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void BlendsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.BlendsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.BlendsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateBlendsView();
		}

		private ICollection EnumerateBlends()
		{
			ProductMapCollectionClass blendCollection = null;

			if (this.Product.IdentityGuid != Guid.Empty)
			{
                //Use the MasterRecordGuid to retrieve the list of Blends for which the Product is used as a Component. The Blend Component list and proportion is not subject to Record Versioning.
				blendCollection = FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
											x => x.EnumerateByAssignedGuidAndType(this.Security, 
																					this.Product.MasterRecordGuid, 
																					PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP)
											);
			}

			var blendDataTable = new DataTable();

			blendDataTable.Columns.Add("Index", typeof(Int32));
			blendDataTable.Columns.Add("ID", typeof(string));
			blendDataTable.Columns.Add("Percent", typeof(string));

			if (blendCollection != null)
			{
				int item = 0;
				
				foreach (ProductMapClass blend in blendCollection)
				{
					DataRow blendDataRow = blendDataTable.NewRow();

					blendDataRow["Index"] = item;
					blendDataRow["ID"] = blend.AssignedToID;
					blendDataRow["Percent"] = blend.BlendPercentage;

					blendDataTable.Rows.Add(blendDataRow);
					item++;
				}
			}

			var blendDataView = new DataView(blendDataTable);
			return blendDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.BlendsDataGrid.PageIndexChanged += this.BlendsDataGridPageIndexChanged;
		}

		private void UpdateBlendsView()
		{
			this.BlendsDataGrid.DataSource = this.EnumerateBlends();
			this.BlendsDataGrid.DataBind();
		}
		#endregion
	}
}