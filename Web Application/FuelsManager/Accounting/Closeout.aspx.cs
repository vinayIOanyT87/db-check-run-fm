// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Closeout.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  The purpose of the Closeout page is to allow the user the ability 
//  to create a closeout transaction for a given manager and product.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

    public partial class Closeout : AccountingWebFormView
	{
		#region Constants and Fields
		public const string CloseoutPageTransition = "CloseoutPageTransition";

		protected FMLabel CloseoutTitleLabel;
		protected ListViewDataSet Grid = null;

		private const bool DoNotIgnore = false;
		private const bool Ignore = true;
		private AccountingSite accountingSite;
		private CloseoutPageTransition closeoutPageTrans;
		#endregion

		#region Methods
		protected void CloseoutFormPageSizeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.InitializeDataGrid();
			}
			catch (Exception exept)
			{
				this.ErrorHandler(exept);
			}
		}

        /// <summary>
        /// When the user clicks at the bottom of the grid to change the page, change the page.
        /// </summary>
        /// <param name="source">The parameter is not used.</param>
        /// <param name="e">Contains the page the user wants to view</param>
        protected void CloseoutDataGrid_OnPageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            try
            {
                this.CloseoutDataGrid.CurrentPageIndex = e.NewPageIndex;
                this.InitializeDataGrid();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

		/// <summary>
		///     This method will handle the ManagerTextBox_TextChanged event and set the
		///     selected item in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ManagerTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(this.ManagerTextBox.Text))
				{
                    this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] = this.ManagerTextBox.Text;				
				}
			}
			catch ( Exception exept )
			{
				this.ErrorHandler( exept );
			}
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			this.CurrentSiteGuid = Guids.SiteAdminGuid;
			this.Initialize();
			base.OnInit(e);
		}

		/// <summary>
		///     This is the main entry point for the closeout page.  It is called by IIS.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.accountingSite =
					FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
						x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

				this.closeoutPageTrans = (CloseoutPageTransition)this.Session[CloseoutPageTransition];

				// Remove from session as it effects ProductSelectForm behavior
				this.Session.Remove("TransactionDetailTransaction");

				if (this.Page.IsPostBack == false)
				{
					// Set initial GrossNetFlag setting
					if (this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] == null)
					{
						this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] =
							this.accountingSite.CurrentSite.QuantityDisplayDefault;
					}

					this.QuantityDropDownList.SelectedIndex = (int)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];

					this.BindPageData();
					this.SetDefaultDates();

					this.InitializeDataGrid();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///     This method will handle the ProductTextBox_TextChanged event and set the
		///     selected item in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ProductTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty( this.ProductTextBox.Text))
				{
				    this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] = this.ProductTextBox.Text;
				}
			}
			catch ( Exception exept )
			{
				this.ErrorHandler( exept );
			}
		}

		/// <summary>
		///     This method will handle the Quantity selection event and set the
		///     selected item in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void QuantityDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session.Add( PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION, this.QuantityDropDownList.SelectedIndex );
			}
			catch ( Exception exept )
			{
				this.ErrorHandler( exept );
			}
		}

		/// <summary>
		///     This method will handle the refresh bring pressed event. It will use the TO/FROM
		///     date fields, manager, and product to request a list of previous closeouts. All
		///     the data must be present or an error dialog is displayed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RefreshBtnOnClick(object sender, EventArgs e)
		{
			try
			{
				this.InitializeDataGrid();
			}
			catch ( Exception exept )
			{
				this.ErrorHandler( exept );
			}
		}

		/// <summary>
		///     This method will bind all the data to the page.
		/// </summary>
		private void BindPageData()
		{
		    var ledgerProductSelection = this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string;
            var ledgerManagerSelection = this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] as string;
		    
			CompanyCollectionClass companyCollection =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
					x => x.EnumerateByRole(this.security, COMPANY_ROLE.MANAGER, false, true));

		    if (!string.IsNullOrEmpty(ledgerManagerSelection)
		        && companyCollection.Find(company => company.ID == ledgerManagerSelection) != null)
		    {
                this.ManagerTextBox.Text = ledgerManagerSelection;
		    }
			else if (companyCollection.Count > 0)
			{
				this.ManagerTextBox.Text = companyCollection[0].ID;
			}
			else
			{
				this.ManagerTextBox.Text = string.Empty;
			}

			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
						x.Get(
							this.Security,
							this.Security.SiteGuid,
							getMemberSites: false,
							getSchedulesAndProcessVariables: false,
							bGetAssociatedAliases: true));

			ProductCollectionClass productCollection =
				FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.security));

		    if (!string.IsNullOrEmpty(ledgerProductSelection)
		        && productCollection.Find(product => product.ID == ledgerProductSelection 
													&& !product.InhibitAccounting
													&& (site.EnableAdditiveAccounting 
														|| product.ProductType != ProductType.AdditiveProduct)) != null)
		    {
                this.ProductTextBox.Text = ledgerProductSelection;
		    }
		    else
		    {
                foreach (ProductClass product in productCollection)
                {
                    if (product.InhibitAccounting)
                    {
                        continue;
                    }

                    if (!site.EnableAdditiveAccounting && product.ProductType == ProductType.AdditiveProduct)
                    {
                        continue;
                    }

                    this.ProductTextBox.Text = product.ID;
                    break;
                }		
		    }
		}

		/// <summary>
		/// This method will display an error dialog informing the user of an error.
		/// </summary>
		private void DisplayErrorDialog(string errorMessage, bool ignoreDataDict)
		{
			string errMsg = GetDataDictionaryValueByKey(this.security.SiteGuid, "An Error has occurred") + "!";

			if (!string.IsNullOrEmpty(errorMessage))
			{
				if (ignoreDataDict)
				{
					errMsg = errorMessage;
				}
				else
				{
					errMsg = GetDataDictionaryValueByKey(this.security.SiteGuid, errorMessage) + "!";
				}
			}

			this.RenderErrorMessage(errMsg);
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		///     This method will load the closeout grid with the data retrieve from the
		///     database. It will contain the previous closeouts for a date range, manager
		///     and product.
		/// </summary>
		private void InitializeDataGrid()
		{
			var closeoutListSR = new CloseoutListSR
			                     {
				                     Security = this.security,
				                     Site = this.security.SiteID,
				                     GetPreviousAndSubsequentCloseouts = false
			                     };

			// Verify there is a manager.
			if (string.IsNullOrEmpty(this.ManagerTextBox.Text))
			{
				this.DisplayErrorDialog("Must select manager", DoNotIgnore);
				return;
			}

			this.closeoutPageTrans = new CloseoutPageTransition();

            closeoutListSR.ManagerGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetMasterRecordGuid(this.Security, this.ManagerTextBox.Text));
            this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] = this.ManagerTextBox.Text;

			// Verify there is a product.
			if (string.IsNullOrEmpty(this.ProductTextBox.Text))
			{
				this.DisplayErrorDialog("Must select product", DoNotIgnore);
				return;
			}

			closeoutListSR.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuidFromID(this.Security, this.ProductTextBox.Text));
			closeoutListSR.CurrentSiteGuid = this.Security.SiteGuid;
            this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] = this.ProductTextBox.Text;

			// Verify there is a TO/FROM date.
			DateTime toNewDate;
			DateTime fromNewDate;
			try
			{
				toNewDate = this.ToDate.CurrentValue.Date;
				fromNewDate = this.FromDate.CurrentValue.Date;
				closeoutListSR.StartDate = fromNewDate;
				closeoutListSR.EndDate = toNewDate;

				if (fromNewDate > toNewDate)
				{
					this.DisplayErrorDialog("TO Date must be more recent than FROM Date", DoNotIgnore);
					return;
				}
			}
			catch (Exception)
			{
				string message = GetDataDictionaryValueByKey(this.security.SiteGuid, "Invalid date format, use:");

				message = message + " " + this.accountingSite.CurrentSite.GetDateTimeFormatInfo().LongDatePattern;
				this.DisplayErrorDialog(message, Ignore);

				return;
			}

			// If all the data is present, then retrieve the previous closeouts. If not, then
			// display an error dialog.
			CloseoutListDO closeoutListDO;
			try
			{
				this.closeoutPageTrans.FromDate = fromNewDate;
				this.closeoutPageTrans.ToDate = toNewDate;

				closeoutListDO = FMChannelHelper.MakeCall<ICloseoutListProcessor, CloseoutListDO>(x => x.Process(closeoutListSR));

				if (closeoutListDO == null)
				{
					this.DisplayErrorDialog("Error in retrieving previous closeouts", DoNotIgnore);
					return;
				}
			}
			catch (Exception)
			{
				this.DisplayErrorDialog("Error in retrieving previous closeouts", DoNotIgnore);
				return;
			}

			this.Session.Add(CloseoutPageTransition, this.closeoutPageTrans);

			//ListViews will need to know if the product is a Volume or AdditiveVolume, because they are formatted differently.
			ProductClass product =
				FMChannelHelper.MakeCall<IProducts, ProductClass>(
					x => x.Get(this.security, x.GetIdentityGuid(this.security, this.ProductTextBox.Text)));

			byte volumeDecimalPlaces;
			byte massDecimalPlaces;

			if (product.ProductType == ProductType.AdditiveProduct)
			{
				if (product.VolumeUnits == 0)
				{
					volumeDecimalPlaces = this.accountingSite.CurrentSite._AdditiveVolumeDecimalPlaces;
				}
				else
				{
					volumeDecimalPlaces = product.VolumeDecimalPlaces;
				}
			}
			else
			{
				if (product.VolumeUnits == 0)
				{
					volumeDecimalPlaces = this.accountingSite.CurrentSite._VolumeDecimalPlaces;
				}
				else
				{
					volumeDecimalPlaces = product.VolumeDecimalPlaces;
				}
			}

			if (product.MassUnits == 0)
			{
				massDecimalPlaces = this.accountingSite.CurrentSite._MassDecimalPlaces;
			}
			else
			{
				massDecimalPlaces = product.MassDecimalPlaces;
			}

			Guid typeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.CLOSEOUT);
			this.Grid = new ListViewDataSet(this.security, LISTVIEW_TYPE.STANDARD, typeGuid, this.accountingSite);
			this.Grid.SetDataGrid(this.CloseoutDataGrid);
			this.Grid.DateFormatInfo = this.accountingSite.CurrentSite.GetDateTimeFormatInfo();
			this.CloseoutDataGrid.AllowSorting = false;

			this.CloseoutFormPageSizeDropDown.SetPageSize(this.CloseoutDataGrid, closeoutListDO.CloseoutList.Count);

			var quantityDisplay = (QuantityDisplay)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];

			this.Grid.BindData(	closeoutListDO.CloseoutList,
								quantityDisplay,
								volumeDecimalPlaces,
								massDecimalPlaces,
								product.LoadByWeight);
		}

		/// <summary>
		///     This method will retrieve the FROM closeout date to default the text field.
		///     If it does not exist, then set to the earliest system date.
		/// </summary>
		private void SetDefaultDates()
		{
			if (this.closeoutPageTrans != null)
			{
				this.FromDate.Text = this.accountingSite.FormatDate(this.closeoutPageTrans.FromDate);
				this.ToDate.Text = this.accountingSite.FormatDate(this.closeoutPageTrans.ToDate);
				return;
			}

			var closeoutListSR = new CloseoutListSR { Security = this.security, Site = this.security.SiteID };

			DateTime toNewDate = DateTime.Today;
			DateTime fromNewDate = DateTimeOffset.MinValue.Date.AddYears(1900);

			closeoutListSR.ManagerGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetMasterRecordGuid(this.Security, this.ManagerTextBox.Text));
			closeoutListSR.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuidFromID(this.Security, this.ProductTextBox.Text));
			closeoutListSR.StartDate = fromNewDate;
			closeoutListSR.EndDate = toNewDate;
            closeoutListSR.CurrentSiteGuid = this.Security.SiteGuid;

			try
			{
				CloseoutListDO closeoutListDO =
					FMChannelHelper.MakeCall<ICloseoutListProcessor, CloseoutListDO>(x => x.Process(closeoutListSR));

				if (closeoutListDO == null)
				{
					this.DisplayErrorDialog("Error in retrieving previous closeouts", DoNotIgnore);
				}
				else
				{
					DateTimeOffset compareDate = DateTimeOffset.Now;
					foreach (CloseoutDO closeoutDO in closeoutListDO.CloseoutList)
					{
						if (closeoutDO.CloseoutDate < compareDate)
						{
							compareDate = closeoutDO.CloseoutDate;
						}
					}

					try
					{
						this.ToDate.Text = this.accountingSite.FormatDate(toNewDate);
						this.FromDate.Text = this.accountingSite.FormatDate(compareDate);
					}
					catch (Exception)
					{
						string message = GetDataDictionaryValueByKey(this.security.SiteGuid, "Invalid date format, use:");

						message = message + " " + this.accountingSite.CurrentSite.GetDateTimeFormatInfo().LongDatePattern;
						this.DisplayErrorDialog(message, Ignore);
					}
				}
			}
			catch (Exception)
			{
				this.DisplayErrorDialog("Error in retrieving previous closeouts", DoNotIgnore);
			}
		}
		#endregion
	}
}