// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IATACodeMainForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IATACodeMainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    /// <summary>
    ///    Code behind for IATACodeMainForm.
    /// </summary>
    public partial class IATACodeMainForm : FMAutoSubmitFormBase
	{
		#region Public Properties

		/// <summary>
        ///    Gets or sets the IATACode object
		/// </summary>
        public IATACodeClass IATACode { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
        ///    the individual tabs associated to the IATACode form.
		/// </summary>
		/// <param name="enable">
		///    if set to <c>true</c> [enable].
		/// </param>
		public void EnableControls(bool enable)
		{
			var iataCodeArrayList = this.Session["IATACodeArrayList"] as ArrayList;
            if (iataCodeArrayList != null)
			{
                this.IATACode = iataCodeArrayList[iataCodeArrayList.Count - 1] as IATACodeClass;
			}

            IATACodeClass iataCodeClass = this.IATACode;
            if (iataCodeClass != null
			    && (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
                    && (this.Security.SiteGuid == iataCodeClass.SiteGuid || iataCodeClass.SiteGuid == Guid.Empty)))
			{
				this.OK.Enabled = enable;
				this.New.Enabled = enable;
			}

			this.Cancel.Enabled = enable;

			this.tcIATACodeTabs.HeaderEnabled = enable;
		}

		/// <summary>
		///    Updates the data.
		/// </summary>
		public void UpdateData()
		{
			this.IATACodeGeneralPage.UpdateData();
			this.IATACodeUserDataPage.UpdateData();
		}

		#endregion

		#region Methods

		/// <summary>
		///    Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						IATACodeClass iataCode =
							FMChannelHelper.MakeCall<IIATACodes, IATACodeClass>(
                                iataCodes => iataCodes.Get(this.Security, this.QueryEntityGuid));

						var list = new ArrayList { iataCode };
                        this.Session["IATACodeArrayList"] = list;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
        /// <exception cref="System.Exception">IATACodeArrayList not in session</exception>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

                var iataCodeArrayList = this.Session["IATACodeArrayList"] as ArrayList;
                if (iataCodeArrayList == null)
				{
                    throw new Exception("IATACodeArrayList not in session");
				}

                this.IATACode = iataCodeArrayList[iataCodeArrayList.Count - 1] as IATACodeClass;

				if (!this.Page.IsPostBack)
				{
					if (this.IATACode != null
					    && (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
                            || (this.Security.SiteGuid != this.IATACode.SiteGuid && this.IATACode.SiteGuid != Guid.Empty)))
					{
						this.OK.Enabled = false;
						this.New.Enabled = false;
					}

					// Set the title label with a key field from the bound object appended
					if (this.IATACode != null)
					{
                        this.IATACodeTitleLabel.Text = this.GetTitleLabelText(this.IATACodeTitleLabel.Text, this.IATACode.ID);
					}
				}

				// General and Contacts are always enabled
				this.tpGeneralPage.Visible = true;
				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

				this.tpUserDataPage.Visible = true;
				this.tpUserDataPage.HeaderText = this.GetTranslatedText("User Data");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		/// <summary>
		///    Handles the Command event of the Cancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.TransferToOriginatingForm();
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
			this.New.Command += this.NewCommand;
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}

		/// <summary>
		///    Handles the Command event of the New control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void NewCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (this.IATACode.IdentityGuid != Guid.Empty)
				{
                    FMChannelHelper.MakeCall<IIATACodes>(x => x.Modify(this.Security, this.IATACode));
				}
				else
				{
					FMChannelHelper.MakeCall<IIATACodes>(x => x.Add(this.Security, this.IATACode));
				}

				this.IATACode.ID = string.Empty;
                this.IATACode.IdentityGuid = Guid.Empty;
			    this.IATACode.Name = string.Empty;
			    this.IATACode.Country = string.Empty;
			    this.IATACode.TimeZone = string.Empty;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("IATACodeMainForm.aspx");
		}

		/// <summary>
		///    Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (this.IATACode.IdentityGuid != Guid.Empty)
				{
                    FMChannelHelper.MakeCall<IIATACodes>(iataCodes => iataCodes.Modify(this.Security, this.IATACode));
				}
				else
				{
                    FMChannelHelper.MakeCall<IIATACodes>(iataCodes => iataCodes.Add(this.Security, this.IATACode));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.TransferToOriginatingForm();
		}

		/// <summary>
		///    Transfers to originating form.
		/// </summary>
		private void TransferToOriginatingForm()
		{
			var iataCodeArrayList = this.Session["IATACodeArrayList"] as ArrayList;
            if (iataCodeArrayList != null)
			{
                iataCodeArrayList.RemoveAt(iataCodeArrayList.Count - 1);

                if (iataCodeArrayList.Count == 0)
				{
                    this.Session.Remove("IATACodeArrayList");
				}
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
            else if (this.Session["IATACodeSelectContextArrayList"] == null)
			{
				this.Redirect("IATACodesForm.aspx");
			}
			else
			{
                var iataCodeSelectContextArrayList = this.Session["IATACodeSelectContextArrayList"] as ArrayList;
                if (iataCodeSelectContextArrayList != null)
				{
					var iataCodeSelectContext =
                        (IATACodeSelectContextClass)iataCodeSelectContextArrayList[iataCodeSelectContextArrayList.Count - 1];

                    iataCodeSelectContextArrayList.RemoveAt(iataCodeSelectContextArrayList.Count - 1);

                    if (iataCodeSelectContextArrayList.Count == 0)
					{
                        this.Session.Remove("IATACodeSelectContextArrayList");
					}

                    string transferString = "IATACodeSelectForm.aspx?";

                    transferString += "All=" + iataCodeSelectContext.All.ToString() + "&";

                    transferString += "Unassigned=" + iataCodeSelectContext.Unassigned.ToString() + "&";

					if (iataCodeSelectContext.IDLink != null)
					{
                        transferString += "IDLink=" + iataCodeSelectContext.IDLink + "&";
					}

                    if (iataCodeSelectContext.Mode != null)
					{
                        transferString += "Mode=" + iataCodeSelectContext.Mode + "&";
					}

                    if (iataCodeSelectContext.SearchString != null)
					{
                        transferString += "SearchString=" + iataCodeSelectContext.SearchString + "&";
					}

					this.Redirect(transferString);
				}
			}
		}

		#endregion
	}

	/// <summary>
    ///    Page base for gaining access to IATACode object
	/// </summary>
	public class IATACodePageBase : FMUserControlBase
	{
		#region Properties

		/// <summary>
        ///    Gets the IATACode.
		/// </summary>
		/// <value>
        ///    The IATACode.
		/// </value>
		protected IATACodeClass IATACode
		{
			get
			{
				return ((IATACodeMainForm)this.Page).IATACode;
			}
		}

		#endregion
	}
}