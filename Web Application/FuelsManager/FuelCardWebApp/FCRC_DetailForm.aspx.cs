// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FCRC_DetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FuelCardWebApp
{
	using System;
	using System.Collections;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
    /// A user control which all fuel card detail tabs can derive from. 
    /// Used to provide access to the Fuel Card we're viewing or editing.
    /// </summary>
    public class FuelCardPageBase : FMUserControlBase
    {
        #region Properties

        protected FuelCardClass FuelCard
        {
            get
            {
                return ((FCRC_DetailForm)this.Page).FuelCard;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// Allows a user to view, add, and modify a Fuel Cards. Contains user control tabs which display fields 
    /// associated with Fuel Cards
    /// </summary>
    public partial class FCRC_DetailForm : FMAutoSubmitFormBase
	{
		#region Public Properties
		public FuelCardClass FuelCard { get; set; }
        #endregion Public Properties

		#region Public Methods and Operators
		/// <summary>
		/// Updates the data.
		/// </summary>
		public void UpdateData()
		{
			this.FCRC_GeneralPage.UpdateData();
			this.FCRC_EquipmentPage.UpdateData();
			this.FCRC_UserDataPage.UpdateData();
		}
        #endregion Public Methods and Operators

        #region Methods

        /// <summary>
		/// This method handles the cancel event. It will return back to the calling page.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void CancelCommand(object sender, EventArgs e)
		{
			this.TransferToOriginatingForm();
		}

		/// <summary>
		/// Event handler for new
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void NewCommand(object sender, EventArgs e)
		{
			if (this.CommitData())
			{
				this.FuelCard.ID = string.Empty;
				this.FuelCard.IdentityGuid = Guid.Empty;
				this.FuelCard.EquipmentCollection = new EquipmentCollectionClass();

				this.Redirect("FCRC_DetailForm.aspx");
			}
		}

		/// <summary>
		/// This method handles the OK button event. It will save the data and return
		/// to the calling page.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OkCommand(object sender, EventArgs e)
		{
			if (this.CommitData())
			{
				this.TransferToOriginatingForm();
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
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						FuelCardClass fuelCard =
							FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(x => x.Get(this.Security, this.QueryEntityGuid, true));

						var list = new ArrayList { fuelCard };
						this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] = list;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
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
				this.Session.Remove("Status");

				this.GetSecurity();

				var fuelCardArrayList = this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] as ArrayList;
				if (fuelCardArrayList == null)
				{
					throw new Exception("FuelCardArrayList not in session");
				}

				this.FuelCard = fuelCardArrayList[fuelCardArrayList.Count - 1] as FuelCardClass;

				if (this.Page.IsPostBack == false)
				{
					if (this.FuelCard == null)
					{
					    var site =
					        FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetUsingGuid(this.Security, this.Security.SiteGuid));

                        this.FuelCard = new FuelCardClass(site);
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA)
						|| (this.Security.SiteGuid != this.FuelCard.SiteGuid && this.FuelCard.SiteGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
						this.New.Enabled = false;
					}

					// Set the title label with a key field from the bound object appended
					this.FuelCardTitleLabel.Text = this.GetTitleLabelText(this.FuelCardTitleLabel.Text, this.FuelCard.ID);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Commits the data.
		/// </summary>
		/// <returns>true if successful</returns>
		private bool CommitData()
		{
			try
			{
				this.UpdateData();

				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return false;
				}

                if (this.FuelCard == null)
                {
                    return true;
                }

				if (this.FuelCard.ID == string.Empty)
				{
					string translatedStr = this.GetTranslatedText("Fuel Card ID is a required field.");
					var except = new Exception(translatedStr);
					this.ErrorHandler(except);
					return false;
				}

				if (string.IsNullOrEmpty(this.FuelCard.ShipToID))
				{
					string translatedStr = this.GetTranslatedText("Ship To is a required field.");
					var except = new Exception(translatedStr);
					this.ErrorHandler(except);
					return false;
				}

                if (string.IsNullOrEmpty(this.FuelCard.ManagerID))
                {
                    string translatedStr = this.GetTranslatedText("Manager is a required field.");
                    var except = new Exception(translatedStr);
                    this.ErrorHandler(except);
                    return false;
                }

                if (string.IsNullOrEmpty(this.FuelCard.OwnerID))
                {
                    string translatedStr = this.GetTranslatedText("Owner is a required field.");
                    var except = new Exception(translatedStr);
                    this.ErrorHandler(except);
                    return false;
                }
                
                if (this.FuelCard.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IFuelCards>(x => x.Modify(this.Security, this.FuelCard));
				}
				else
				{
					FMChannelHelper.MakeCall<IFuelCards>(x => x.Add(this.Security, this.FuelCard));
				}

				return true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return false;
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// This method will determine which form to transaction back to.
		/// </summary>
		private void TransferToOriginatingForm()
		{
			if (this.IsFromQueryWriter)
			{
				var fuelCardArrayList = this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] as ArrayList;
				if (fuelCardArrayList != null)
				{
					fuelCardArrayList.RemoveAt(fuelCardArrayList.Count - 1);

					if (fuelCardArrayList.Count == 0)
					{
						this.Session.Remove(PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST);
					}
				}
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (this.Page.Session[PageSessionKeyConstants.FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST] != null)
			{
				this.Redirect("../FMWebApp/FuelCardSelectForm.aspx?Unassigned=true");
			}
			else
			{
				this.Redirect("FCRC_SummaryForm.aspx");
			}
        }
        #endregion Methods
    }
}