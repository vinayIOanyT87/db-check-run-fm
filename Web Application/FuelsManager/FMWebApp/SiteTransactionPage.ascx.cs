/******************************************************************************
	FILE NAME:		SiteTransactionPage.ascx.cs
	PURPOSE:		Implementation of SiteTransactionPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		---------	-----------------	-------------------------------------------
		2006-10-24	Richard Panachida	Fixed data dictionary. Some labels are not inhieriting
										from FMControls (CSI 3405).

		2007-02-01	W.Gray				7.1.0.6 - Added logic to format Transaction Numbers based upon the
												length of the ending number (CSI 4059)
												
		2007-07-30	I.Orndorff			1.0.0.2 - Changed ExceptionBOLPrinterDropDownList from 
												  System.Web.UI.WebControls.DropDownList to 
												  FMControls.FMDropDownList. This fixes CSI #4670.
												  
		2008-04-22	I.Orndorff			7.4.0.0 - Modified "Page_Load()" to set the default BOL exception 
												  printer even if it can't be enumerated. This fixes CSI #5777.
												  
		07-15-2008	V. Thompson			ADF
										Added InvoiceStartNumber, InvoiceEndNumber and InvoiceNextNumber controls

 		2012-04-24  B. Main				8.0.5.0 - Cannot discover local printers when running in Azure.  When in Azure 
												ExceptionBOLPrinter drop down list will contain one option, {None}
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Drawing;
	using System.Globalization;
	using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
	///		Summary description for SiteTransactionPage.
	/// </summary>
	public partial class SiteTransactionPage : FMUserControlBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
                var site = (SiteClass)this.Session["Site"];
                var localsecurity = new SecurityClass
                {
                    UserID = this.Security.UserID,
                    UserGuid = this.Security.UserGuid,
                    Password = this.Security.Password,
                    Token = this.Security.Token,
                    SiteID = this.Security.SiteID,
                    SiteGuid = this.Security.SiteGuid,
                    LoginSiteID = this.Security.LoginSiteID,
                    LoginSiteGuid = this.Security.LoginSiteGuid
                };

                // copy relevant memeber into local security
                localsecurity.CloneRights(this.Security);

                if (!this.Page.IsPostBack) 
				{
				    this.PrintBrokenBlendsCheckBox.Checked					= !site.InhibitBOLWithBrokenBlends;
				    this.PrintImproperAdditizationCheckBox.Checked		    = !site.InhibitBOLWithImproperAdditization;
				    this.PrintOverweightBOLCheckBox.Checked				    = !site.InhibitOverweightBOL;

					// ExceptionBOLPrinterDropDownList
					ListItem newPrinterItem;

					try
				    {
				        string[] installedPrinters = FMBusinessObjects.UtilityObjects.ReportServicePrintService.EnumeratePrinters("Site Transaction BOL");
				        int index = 1;
				        foreach (string printer in installedPrinters)
				        {
				            newPrinterItem = new ListItem(printer, index.ToString());
				            foreach (ListItem existingPrinterItem in this.ExceptionBOLPrinterDropDownList.Items)
				            {
				                if (string.Compare(existingPrinterItem.Text, newPrinterItem.Text, StringComparison.Ordinal) > 0)
				                {
				                    int insert = this.ExceptionBOLPrinterDropDownList.Items.IndexOf(existingPrinterItem);
				                    this.ExceptionBOLPrinterDropDownList.Items.Insert(insert, newPrinterItem);
				                    if (site.ExceptionBOLPrinter == newPrinterItem.Text) this.ExceptionBOLPrinterDropDownList.SelectedIndex = insert;
				                    newPrinterItem = null;
				                    break;
				                }
				            }

				            if (newPrinterItem != null)
				            {
				                this.ExceptionBOLPrinterDropDownList.Items.Add(newPrinterItem);
				                if (site.ExceptionBOLPrinter == newPrinterItem.Text) this.ExceptionBOLPrinterDropDownList.SelectedIndex = this.ExceptionBOLPrinterDropDownList.Items.Count - 1;
				            }

				            index++;
				        }
				    }
				    catch (System.Net.Sockets.SocketException socketExcept)
				    {
				        if (socketExcept.ErrorCode != 10061)
				            throw;

				        if (site.ExceptionBOLPrinter != "{None}" &&
				            site.ExceptionBOLPrinter.Length > 0)
				        {
				            ListItem printerItemFromDb = new ListItem(site.ExceptionBOLPrinter, "1");
				            this.ExceptionBOLPrinterDropDownList.Items.Add(printerItemFromDb);
				            this.ExceptionBOLPrinterDropDownList.SelectedIndex = this.ExceptionBOLPrinterDropDownList.Items.Count - 1;
				        }
				    }

				    //in Azure {None} is only choice for printer
					newPrinterItem = new ListItem(this.GetTranslatedText("{None}"), "0");
				    this.ExceptionBOLPrinterDropDownList.Items.Insert(0, newPrinterItem);

				    this.EnableAutomaticPrintingCheckBox.Checked	= site.EnableAutomaticBOLPrinting;

				    this.AutomaticBOLEndNumberTextBox.Text			= site.AutomaticBOLEndNumber;
					int length = this.AutomaticBOLEndNumberTextBox.Text.Length;
				    this.AutomaticBOLStartNumberTextBox.Text			= site._AutomaticBOLStartNumber.ToString("D"+length.ToString(CultureInfo.InvariantCulture));
				    this.AutomaticBOLNextNumberTextBox.Text			= site._AutomaticBOLNextNumber.ToString("D"+length.ToString(CultureInfo.InvariantCulture));
				    this.SeparateManualBOLNumberingCheckBox.Checked= site.SeparateManualBOLNumbering;
				    this.SeparateManualBOLNumberingCheckBox_CheckedChanged(null,null);
				    this.TransactionEndNumberTextBox.Text				= site._TransactionEndNumber.ToString();
					length= this.TransactionEndNumberTextBox.Text.Length;
				    this.TransactionStartNumberTextBox.Text			= site._TransactionStartNumber.ToString("D"+length.ToString(CultureInfo.InvariantCulture));
				    this.TransactionNextNumberTextBox.Text			= site._TransactionNextNumber.ToString("D"+length.ToString(CultureInfo.InvariantCulture));
				    this.OrderEndNumberTextBox.Text						= site.OrderEndNumber.ToString(CultureInfo.InvariantCulture);
					length= this.OrderEndNumberTextBox.Text.Length;
				    this.OrderStartNumberTextBox.Text					= site._OrderStartNumber.ToString("D"+length.ToString(CultureInfo.InvariantCulture));
				    this.OrderNextNumberTextBox.Text					= site._OrderNextNumber.ToString("D"+length.ToString(CultureInfo.InvariantCulture));
					// vt 07-15-2008
                    this.EnableAdditiveAccountingCheckBox.Checked = site.EnableAdditiveAccounting;
                    this.EnforceSingleOwner.Checked = site.EnforceSingleOwner;

                    this.NumberPrefixTextBox.Text						= site.NumberPrefix;
                    this.EnableBOLPDFArchivingCheckbox.Checked          = site.EnableBOLPDFArchiving;
                    this.BOLPDFArchivingPathTextBox.Text                = site.BOLPDFArchivingPath;

					// Populate OpenTransactionWindowDropDownList
                   // This setting is designed to specify a number of months to keep open.  Zero is valid and 
                   // specifies that the closeout date should be today.  This is used for CloseoutSiteProcessor 
                   // to set the closeout date.
					for(int month=0;month < 13;month++)
					{
						ListItem newMonthItem=new ListItem(month.ToString(),month.ToString());
					    this.OpenTransactionWindowDropDownList.Items.Add(newMonthItem);
						if(site._OpenTransactionWindow == month) this.OpenTransactionWindowDropDownList.SelectedIndex= this.OpenTransactionWindowDropDownList.Items.Count-1;
					}

                    var item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
                    this.InventoryTransactionDropDownList.Items.Add(item);
                    item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
                    this.AdjustmentTransactionDropDownList.Items.Add(item);

                    // Force the SiteGuid to the Site.IdentityGuid such that enumerations will
                    // be in the correct context.
                    if (site.IdentityGuid != Guid.Empty)
                    {
                        localsecurity.SiteGuid = site.IdentityGuid;
                        localsecurity.SiteID = site.ID;

                        var transactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
                            x => x.EnumerateByTransTypeID(localsecurity, TransactionTypes.T14_PhysicalInventory));

                        foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
                        {
                            item = new ListItem(transactionAlias.ID, transactionAlias.MasterRecordGuid.ToString());
                            this.InventoryTransactionDropDownList.Items.Add(item);
                            if (transactionAlias.MasterRecordGuid == site.InventoryTransactionAliasGuid)
                            {
                                this.InventoryTransactionDropDownList.SelectedIndex = this.InventoryTransactionDropDownList.Items.Count - 1;
                            }
                        }

                        transactionAliasCollection =
                            FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
                                x => x.EnumerateByTransTypeID(localsecurity, TransactionTypes.T1_PrimaryAdjustment));

                        foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
                        {
                            item = new ListItem(transactionAlias.ID, transactionAlias.MasterRecordGuid.ToString());
                            this.AdjustmentTransactionDropDownList.Items.Add(item);
                            if (transactionAlias.MasterRecordGuid == site.AdjustmentTransactionAliasGuid)
                            {
                                this.AdjustmentTransactionDropDownList.SelectedIndex = this.AdjustmentTransactionDropDownList.Items.Count - 1;
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

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void UpdateData()
		{
		    SiteClass site = (SiteClass)this.Session["Site"];

		    site.InhibitBOLWithBrokenBlends = !this.PrintBrokenBlendsCheckBox.Checked;
		    site.InhibitBOLWithImproperAdditization = !this.PrintImproperAdditizationCheckBox.Checked;
		    site.InhibitOverweightBOL = !this.PrintOverweightBOLCheckBox.Checked;

		    if (this.ExceptionBOLPrinterDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
		    {
		        site.ExceptionBOLPrinter = "{None}";
		    }
		    else
		    {
		        site.ExceptionBOLPrinter = this.ExceptionBOLPrinterDropDownList.SelectedItem.Text;
		    }

		    site.EnableAutomaticBOLPrinting = this.EnableAutomaticPrintingCheckBox.Checked;
		    site.AutomaticBOLStartNumber = this.AutomaticBOLStartNumberTextBox.Text;
		    site.AutomaticBOLEndNumber = this.AutomaticBOLEndNumberTextBox.Text;
		    site.AutomaticBOLNextNumber = this.AutomaticBOLNextNumberTextBox.Text;
		    site.SeparateManualBOLNumbering = this.SeparateManualBOLNumberingCheckBox.Checked;
            site.EnableBOLPDFArchiving      = this.EnableBOLPDFArchivingCheckbox.Checked;
            site.BOLPDFArchivingPath        = this.BOLPDFArchivingPathTextBox.Text;


		    if (site.SeparateManualBOLNumbering)
		    {
		        site.ManualBOLStartNumber = this.ManualBOLStartNumberTextBox.Text;
		        site.ManualBOLEndNumber = this.ManualBOLEndNumberTextBox.Text;
		        site.ManualBOLNextNumber = this.ManualBOLNextNumberTextBox.Text;
		    }

		    site.TransactionStartNumber = this.TransactionStartNumberTextBox.Text;
		    site.TransactionEndNumber = this.TransactionEndNumberTextBox.Text;
		    site.TransactionNextNumber = this.TransactionNextNumberTextBox.Text;

		    site.OrderStartNumber = this.OrderStartNumberTextBox.Text;
		    site.OrderEndNumber = this.OrderEndNumberTextBox.Text;
		    site.OrderNextNumber = this.OrderNextNumberTextBox.Text;

		    // vt 07-15-2008
		    site.NumberPrefix = this.NumberPrefixTextBox.Text;
            site.EnforceSingleOwner = this.EnforceSingleOwner.Checked;

            if (this.OpenTransactionWindowDropDownList.SelectedIndex != -1)
		    {
		        site.OpenTransactionWindow = this.OpenTransactionWindowDropDownList.SelectedValue;
		    }

		    site.EnableAdditiveAccounting = this.EnableAdditiveAccountingCheckBox.Checked;
		    site.InventoryTransactionAliasGuid = new Guid(this.InventoryTransactionDropDownList.SelectedValue);
		    if (this.InventoryTransactionDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
		    {
		        site.InventoryTransactionAliasID = "{None}";
		    }
		    else
		    {
		        site.InventoryTransactionAliasID = this.InventoryTransactionDropDownList.SelectedItem.Text;
		    }
		    site.AdjustmentTransactionAliasGuid = new Guid(this.AdjustmentTransactionDropDownList.SelectedValue);
		    if (this.AdjustmentTransactionDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
		    {
		        site.AdjustmentTransactionAliasID = "{None}";
		    }
		    else
		    {
		        site.AdjustmentTransactionAliasID = this.AdjustmentTransactionDropDownList.SelectedItem.Text;
		    }
        }

        // ReSharper disable once InconsistentNaming
        protected void SeparateManualBOLNumberingCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			SiteClass	site=(SiteClass)this.Session["Site"];

            this.ManualBOLStartNumberTextBox.Enabled= this.SeparateManualBOLNumberingCheckBox.Checked;
            this.ManualBOLEndNumberTextBox.Enabled= this.SeparateManualBOLNumberingCheckBox.Checked;
            this.ManualBOLNextNumberTextBox.Enabled= this.SeparateManualBOLNumberingCheckBox.Checked;

			if(!this.SeparateManualBOLNumberingCheckBox.Checked)
			{
			    this.ManualBOLStartNumberTextBox.BackColor	= Color.LightGray;
			    this.ManualBOLEndNumberTextBox.BackColor		= Color.LightGray;
			    this.ManualBOLNextNumberTextBox.BackColor	= Color.LightGray;
			    this.ManualBOLStartNumberTextBox.Text			= "";
			    this.ManualBOLEndNumberTextBox.Text			= "";
			    this.ManualBOLNextNumberTextBox.Text			= "";
			}
			else
			{
			    this.ManualBOLStartNumberTextBox.BackColor = Color.White;
			    this.ManualBOLEndNumberTextBox.BackColor   = Color.White;
			    this.ManualBOLNextNumberTextBox.BackColor  = Color.White;
			    this.ManualBOLEndNumberTextBox.Text        = site.ManualBOLEndNumber;
            int length                            = this.ManualBOLEndNumberTextBox.Text.Length;
			    this.ManualBOLStartNumberTextBox.Text      = site._ManualBOLStartNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
			    this.ManualBOLNextNumberTextBox.Text       = site._ManualBOLNextNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
			}
		}

        // ReSharper disable once InconsistentNaming
		protected void AutomaticBOLEndNumberTextBox_TextChanged(object sender, EventArgs e)
		{
         try
         {
            SiteClass site = (SiteClass)this.Session["Site"];

            site.AutomaticBOLStartNumber        = this.AutomaticBOLStartNumberTextBox.Text;
            site.AutomaticBOLEndNumber          = this.AutomaticBOLEndNumberTextBox.Text;
            site.AutomaticBOLNextNumber         = this.AutomaticBOLNextNumberTextBox.Text;
             this.AutomaticBOLEndNumberTextBox.Text   = site.AutomaticBOLEndNumber;
            int length                          = this.AutomaticBOLEndNumberTextBox.Text.Length;
             this.AutomaticBOLStartNumberTextBox.Text = site._AutomaticBOLStartNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
             this.AutomaticBOLNextNumberTextBox.Text  = site._AutomaticBOLNextNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
		}

        // ReSharper disable once InconsistentNaming
		protected void ManualBOLEndNumberTextBox_TextChanged(object sender, EventArgs e)
		{
         try
         {
            SiteClass site = (SiteClass)this.Session["Site"];

            site.ManualBOLStartNumber        = this.ManualBOLStartNumberTextBox.Text;
            site.ManualBOLEndNumber          = this.ManualBOLEndNumberTextBox.Text;
            site.ManualBOLNextNumber         = this.ManualBOLNextNumberTextBox.Text;
             this.ManualBOLEndNumberTextBox.Text   = site.ManualBOLEndNumber;
            int length                       = this.ManualBOLEndNumberTextBox.Text.Length;
             this.ManualBOLStartNumberTextBox.Text = site._ManualBOLStartNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
             this.ManualBOLNextNumberTextBox.Text  = site._ManualBOLNextNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
		}

        // ReSharper disable once InconsistentNaming
		protected void TransactionEndNumberTextBox_TextChanged(object sender, EventArgs e)
		{
         try
         {
            SiteClass site = (SiteClass)this.Session["Site"];

            site.TransactionStartNumber        = this.TransactionStartNumberTextBox.Text;
            site.TransactionEndNumber          = this.TransactionEndNumberTextBox.Text;
            site.TransactionNextNumber         = this.TransactionNextNumberTextBox.Text;
             this.TransactionEndNumberTextBox.Text   = site.TransactionEndNumber;
            int length                         = this.TransactionEndNumberTextBox.Text.Length;
             this.TransactionStartNumberTextBox.Text = site._TransactionStartNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
             this.TransactionNextNumberTextBox.Text  = site._TransactionNextNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
		}

        // ReSharper disable once InconsistentNaming
		protected void OrderEndNumberTextBox_TextChanged(object sender, EventArgs e)
		{
         try
         {
            SiteClass site = (SiteClass)this.Session["Site"];

            site.OrderStartNumber        = this.OrderStartNumberTextBox.Text;
            site.OrderEndNumber          = this.OrderEndNumberTextBox.Text;
            site.OrderNextNumber         = this.OrderNextNumberTextBox.Text;
             this.OrderEndNumberTextBox.Text   = site.OrderEndNumber;
            int length                   = this.OrderEndNumberTextBox.Text.Length;
             this.OrderStartNumberTextBox.Text = site._OrderStartNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
             this.OrderNextNumberTextBox.Text  = site._OrderNextNumber.ToString("D" + length.ToString(CultureInfo.InvariantCulture));
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
		}
	}
}
