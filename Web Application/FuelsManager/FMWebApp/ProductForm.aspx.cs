// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductForm.aspx.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
    using FMCore;
    using global::FMWebApp;

	/// <summary>
	///    Summary description for ProductForm.
	/// </summary>
	public partial class ProductForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		public SiteClass CurrentSite;
		public ProductClass Product = null;
        public List<string> VersionSpecificFields = null;

		protected TextBox IDTextBox;
		protected Label Label1;
		protected Label Label2;
		#endregion

		#region Public Methods and Operators
		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the product form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			var productArrayList = this.Session["ProductArrayList"] as ArrayList;

			if (productArrayList != null)
			{
				this.Product = productArrayList[productArrayList.Count - 1] as ProductClass;
			}

			ProductClass productClass = this.Product;

			if (productClass != null 
				&& (this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
			         && (this.Security.SiteGuid == productClass.SiteGuid || productClass.SiteGuid == Guid.Empty)))
			{
				this.OK.Enabled = enable;
				this.New.Enabled = enable;
			}

			this.Cancel.Enabled = enable;
			this.tcProductTabs.HeaderEnabled = enable;
		}

		public void UpdateData()
		{
			this.ProductAdditivePage.UpdateData();
			this.ProductAlarmsPage.UpdateData();
			this.ProductAuthorizedCustomersPage.UpdateData();
			this.ProductBlendPage.UpdateData();
			this.ProductComponentPage.UpdateData();
			this.ProductGeneralPage.UpdateData();
			this.ProductMessagesPage.UpdateData();
			this.ProductUserDataPage.UpdateData();
			this.ProductVolumeCorrectionPage.UpdateData();
			this.ProductUnitsPage.UpdateData();
            this.ProductGraphicsPage.UpdateData();
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

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						ProductClass productClass = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																		 x =>
																		 x.Get(this.Security, this.QueryEntityGuid)
																	);


						var list = new ArrayList { productClass };

						this.Session["ProductArrayList"] = list;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				var productArrayList = this.Session["ProductArrayList"] as ArrayList;
				
				if (productArrayList == null)
				{
					throw new Exception("ProductArrayList not in session");
				}

				this.Product = productArrayList[productArrayList.Count - 1] as ProductClass;

                this.VersionSpecificFields = this.Session["ProductVersionSpecificFields"] as List<string>;

                if (!this.Page.IsPostBack)
				{
                    this.GetRecordVersioningFields();
                    if (this.Product != null)
					{
						if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
						        || (this.Product.SiteGuid != Guid.Empty &&
                                    this.Security.SiteGuid != this.Product.SiteGuid &&
                                    (this.VersionSpecificFields == null  || this.VersionSpecificFields.Count == 0))
                           )
						{
							this.OK.Enabled = false;
							this.New.Enabled = false;
						}

						//Set the title label with a key field from the bound object appended
						this.ProductTitleLabel.Text = this.GetTitleLabelText(this.ProductTitleLabel.Text, this.Product.ID);
					}
				}

				// Set up the Tabs based upon Type
				this.tpBlendPage.Visible = false;
				this.tpComponentPage.Visible = false;
				this.tpAdditivePage.Visible = false;
				//this.tpAlarmsPage.Visible = false;

				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

				switch (this.Product.ProductType)
				{
					case ProductType.BlendProduct:
						this.tpBlendPage.Visible = true;
						this.tpBlendPage.HeaderText = this.GetTranslatedText(ProductClass.ProductTypeID(this.Product.ProductType));
						break;
					case ProductType.ComponentProduct:
						this.tpComponentPage.Visible = true;
						this.tpComponentPage.HeaderText = this.GetTranslatedText(ProductClass.ProductTypeID(this.Product.ProductType));
						break;
					case ProductType.AdditiveProduct:
						this.tpAdditivePage.Visible = true;
						this.tpAdditivePage.HeaderText = this.GetTranslatedText(ProductClass.ProductTypeID(this.Product.ProductType));
						break;
				}

				this.tpUnitsPage.HeaderText = this.GetTranslatedText("Units");
				this.tpAuthorizedCustomersPage.HeaderText = this.GetTranslatedText("Authorized Customers");
				this.tpVolumeCorrectionPage.HeaderText = this.GetTranslatedText("Volume Correction");

				this.tpMessagesPage.HeaderText = this.GetTranslatedText("Messages");
				this.tpUserDataPage.HeaderText = this.GetTranslatedText("User Data");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}


        private void GetRecordVersioningFields()
        {
            this.VersionSpecificFields = new List<string>();
            bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if (this.Product.IdentityGuid.Equals(Guid.Empty)
                || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)))
            {
                return;
            }
            string flcMode = FieldLevelConfigClass.FLCModeGSOnly;
            if (currentSiteOwnsRecordVersion)
                flcMode = FieldLevelConfigClass.FLCModeVSandGS;

            try
            {
					this.VersionSpecificFields = FMChannelHelper.MakeCall<IEntityToSiteMaps, List<string>>(
												x =>
												x.GetRecordVersioningFields(this.Security, this.Product.EntityType, this.Product.MasterRecordGuid, flcMode)
										);

                    this.Session["ProductVersionSpecificFields"] = this.VersionSpecificFields;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }

            if (this.VersionSpecificFields == null)
            {
                this.VersionSpecificFields = new List<string>();
            }
        }


		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.TransferToOriginatingForm();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.New.Command += this.NewCommand;
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
			ucFMMenuBar.Visible = (Page.Request.GetQueryOrFormValue("Modal") != null) ? false : true;
		}

		private void NewCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (this.Product.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IProducts>(x => x.Modify(this.Security, this.Product));
				}
				else
				{
					FMChannelHelper.MakeCall<IProducts, Guid>(x => x.Add(this.Security,this.Product));
				}

				this.Product.ID = string.Empty;
				this.Product.IdentityGuid = Guid.Empty;

				foreach (ProductMapClass authorizedCompany in this.Product.AuthorizedCustomerCollection)
				{
					authorizedCompany.SpecialInstructions = string.Empty;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("ProductForm.aspx");
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (this.Product.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IProducts>(x => x.Modify(this.Security, this.Product));
					
					try
					{
						if (UsingLoadRack)
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Modify(this.Security, typeof(ProductClass), this.Product.IdentityGuid);
						}
					}
					catch (SocketException socketExcept)
					{
						if (socketExcept.ErrorCode != 10061)
						{
							throw;
						}
					}
				}
				else
				{
					FMChannelHelper.MakeCall<IProducts, Guid>(x => x.Add(this.Security, this.Product));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.TransferToOriginatingForm();
		}

		private void TransferToOriginatingForm()
		{
			var productArrayList = this.Session["ProductArrayList"] as ArrayList;

			if (productArrayList != null)
			{
				productArrayList.RemoveAt(productArrayList.Count - 1);
			
				if (productArrayList.Count == 0)
				{
					this.Session.Remove("ProductArrayList");
				}
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (this.Session["ProductSelectContextArrayList"] == null)
			{
				this.Redirect("ProductsForm.aspx");
			}
			else
			{
				var productSelectContextArrayList = this.Session["ProductSelectContextArrayList"] as ArrayList;

				if (productSelectContextArrayList != null)
				{
					var productSelectContext =
						productSelectContextArrayList[productSelectContextArrayList.Count - 1] as ProductSelectContextClass;

					productSelectContextArrayList.RemoveAt(productSelectContextArrayList.Count - 1);
					
					if (productSelectContextArrayList.Count == 0)
					{
						this.Session.Remove("ProductSelectContextArrayList");
					}

					string transferString = "ProductSelectForm.aspx?";

					if (productSelectContext != null)
					{
						if (productSelectContext.Type != ProductType.MaxProduct)
						{
							transferString += "Type=" + productSelectContext.Type + "&";
						}

						transferString += "All=" + productSelectContext.All + "&";

						transferString += "Unassigned=" + productSelectContext.Unassigned + "&";

						if (productSelectContext.IDLink != null)
						{
							transferString += "IDLink=" + productSelectContext.IDLink + "|" + productSelectContext.IDLinkType + "&";
						}

						if (productSelectContext.Mode != null)
						{
							transferString += "Mode=" + productSelectContext.Mode + "&";
						}

						if (productSelectContext.SearchString != null)
						{
							transferString += "SearchString=" + productSelectContext.SearchString + "&";
						}

		                if (productSelectContext.HideHidden)
		                {
		                    transferString += "HideHidden=" + productSelectContext.HideHidden + "&";
						}
					}

					this.Redirect(transferString);
				}
			}
		}
		#endregion
	}

	public class ProductPageBase : FMUserControlBase
	{
		#region Properties
		protected ProductClass Product
		{
			get { return ((ProductForm)this.Page).Product; }
		}

        protected List<string> VersionSpecificFields
        {
            get { return ((ProductForm)this.Page).VersionSpecificFields; }
        }
		#endregion
	}
}