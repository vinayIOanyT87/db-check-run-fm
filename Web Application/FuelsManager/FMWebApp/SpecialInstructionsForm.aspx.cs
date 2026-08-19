// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialInstructionsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SpecialInstructionsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FMCore;

    /// <summary>
    ///    Summary description for SpecialInstructionsForm.
    /// </summary>
    public partial class SpecialInstructionsForm : FMFormBase
    {
        #region Methods

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            this.Response.Write("<SCRIPT>window.returnValue = false; window.close();</SCRIPT>");
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                ProductMapClass ProductMapForNote = this.GetProductMapForNoteObject();

                if (ProductMapForNote != null)
                {
                    ProductMapForNote.SpecialInstructions = this.SpecialInstructionsText.Text;
                }

                this.Response.Write("<SCRIPT>window.returnValue = true; window.close();</SCRIPT>");
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
                this.GetSecurity();

                if (this.IsPostBack == false)
                {
                    this.SpecialInstructionsText.Attributes.Add("maxLength", "2000");

                    // Get the mode
                    string mode = this.Request.GetQueryOrFormValue("mode");

                    switch (mode)
                    {
                        case "company":
                            this.PrepForCompanyMode();
                            break;

                        case "companygroup":
                            this.PrepForCompanyGroupMode();
                            break;

                        case "product":
                            this.PrepForProductMode(false);
                            break;

                        case "productReadOnly":
                            this.PrepForProductMode(true);
                            break;

                        case "txdetail":
                            this.PrepForTxDetailMode();
                            break;

                        default:
                            throw new Exception("Unknown special instruction mode");
                    }
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private ProductMapClass GetCompanyGroupProductMapForNote()
        {
            var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

            if (CompanyGroup == null)
            {
                throw new Exception("Expected CompanyGroup object in session");
            }

            int ItemIndex = Convert.ToInt32(this.Request.GetQueryOrFormValue("ItemIndex"));

            // Set up the controls
            ProductMapClass ProductMap = CompanyGroup.AuthorizedProductCollection[ItemIndex];

            return ProductMap;
        }

        private ProductMapClass GetCompanyProductMapForNote()
        {
            // Get the property map we should use
            var CompanyArrayList = this.Session["CompanyArrayList"] as ArrayList;
            if (CompanyArrayList == null)
            {
                throw new Exception("CompanyArrayList not in session");
            }

            var Company = CompanyArrayList[CompanyArrayList.Count - 1] as CompanyClass;

            int ItemIndex = Convert.ToInt32(this.Request.GetQueryOrFormValue("ItemIndex"));

            // Set up the controls
            ProductMapClass ProductMap = Company.AuthorizedProductCollection[ItemIndex];

            return ProductMap;
        }

        private ProductMapClass GetProductMapForNoteObject()
        {
            string mode = this.Request.GetQueryOrFormValue("mode");

            switch (mode)
            {
                case "company":
                    return this.GetCompanyProductMapForNote();

                case "companygroup":
                    return this.GetCompanyGroupProductMapForNote();

                case "product":
                    return this.GetProductProductMapForNote();

                case "txdetail":
                    return null;
            }

            throw new Exception("Unknown special instruction mode");
        }

        private ProductMapClass GetProductProductMapForNote()
        {
            var ProductArrayList = this.Session["ProductArrayList"] as ArrayList;

            if (ProductArrayList == null)
            {
                throw new Exception("ProductArrayList not in session");
            }

            var Product = ProductArrayList[ProductArrayList.Count - 1] as ProductClass;

            int ItemIndex = Convert.ToInt32(this.Request.GetQueryOrFormValue("ItemIndex"));

            // Set up the controls
            ProductMapClass ProductMap = Product.AuthorizedCustomerCollection[ItemIndex];

            return ProductMap;
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        private void PrepForCompanyGroupMode()
        {
            var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

            if (CompanyGroup == null)
            {
                throw new Exception("Expected CompanyGroup object in session");
            }

            int ItemIndex = Convert.ToInt32(this.Request.GetQueryOrFormValue("ItemIndex"));

            // Set up the controls
            ProductMapClass ProductMap = CompanyGroup.AuthorizedProductCollection[ItemIndex];

            this.SpecialInstructionsText.Text = ProductMap.SpecialInstructions;
        }

        private void PrepForCompanyMode()
        {
            // Get the property map we should use
            var CompanyArrayList = this.Session["CompanyArrayList"] as ArrayList;
            if (CompanyArrayList == null)
            {
                throw new Exception("CompanyArrayList not in session");
            }

            var Company = CompanyArrayList[CompanyArrayList.Count - 1] as CompanyClass;

            int ItemIndex = Convert.ToInt32(this.Request.GetQueryOrFormValue("ItemIndex"));

            // Set up the controls
            ProductMapClass ProductMap = Company.AuthorizedProductCollection[ItemIndex];

            this.SpecialInstructionsText.Text = ProductMap.SpecialInstructions;
        }

        private void PrepForProductMode(bool readOnlyMode)
        {
            var ProductArrayList = this.Session["ProductArrayList"] as ArrayList;

            if (ProductArrayList == null)
            {
                throw new Exception("ProductArrayList not in session");
            }

            var Product = ProductArrayList[ProductArrayList.Count - 1] as ProductClass;

            int ItemIndex = Convert.ToInt32(this.Request.GetQueryOrFormValue("ItemIndex"));

            // Set up the controls
            ProductMapClass ProductMap = Product.AuthorizedCustomerCollection[ItemIndex];

            this.SpecialInstructionsText.Text = ProductMap.SpecialInstructions;

            if (readOnlyMode)
            {
                this.SpecialInstructionsText.ReadOnly = true;
                this.OKButton.Visible = false;
            }
        }

        private void PrepForTxDetailMode()
        {
            Guid itemGuid = Guid.Parse(this.Request.GetQueryOrFormValue("ItemIdentityGuid"));

            string specialInstr = FMChannelHelper.MakeCall<IProductMaps, string>(
                                                       x =>
                                                       x.GetSpecialInstructions(this.Security, itemGuid)
                                                   );
            this.SpecialInstructionsText.Text = specialInstr;
            this.SpecialInstructionsText.ReadOnly = true;

            this.CancelButton.Visible = false;
        }

        #endregion
    }
}