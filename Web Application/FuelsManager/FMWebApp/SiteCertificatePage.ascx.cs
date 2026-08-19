// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteCertificatePage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	public partial class SiteCertificatePage : FMUserControlBase
	{
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var siteClass = (SiteClass)Session["Site"];

				if (!Page.IsPostBack)
				{
					var addList = new List<ListItem>();

					// Populate AssignedCertificatesListBox
					foreach (ApplicationStringClass assignedApplicationString in siteClass.SiteCertificateCollection)
					{
						var assignedSiteItem = new ListItem(assignedApplicationString.ID, assignedApplicationString.IdentityGuid.ToString());
						addList.Add(assignedSiteItem);
					}

					// Sort the list and then added it to the list box
					addList = addList.OrderBy(li => li.Text).ToList();
					this.AssignedCertificatesListBox.Items.AddRange(addList.ToArray());

					// Populate UnassignedCertificatesListBox
					X509Store store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);

					store.Open(OpenFlags.ReadOnly);

					addList = new List<ListItem>();
					foreach (X509Certificate2 mCert in store.Certificates)
					{
						if (mCert.SubjectName.Name == null)
						{
							continue;
						}

						string[] subStrings = mCert.SubjectName.Name.Split(new char[] { ',' });
						if (subStrings.Length == 0)
						{
							continue;
						}

						var issuedTo = subStrings[0].Replace("CN=","");

						if (null == this.AssignedCertificatesListBox.Items.FindByText(issuedTo))
						{
							var unassignedSiteItem = new ListItem(issuedTo, issuedTo);
							addList.Add(unassignedSiteItem);
						}
					}

					// Sort the list and then added it to the list box
					addList = addList.OrderBy(li => li.Text).ToList();
					this.UnassignedCertificatesListBox.Items.AddRange(addList.ToArray());
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignCertificatesButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.UnassignCertificatesButton_Command);
			this.AssignCertificatesButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AssignCertificatesButton_Command);

		}
		#endregion

		private void AssignCertificatesButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			ListItem UnassignedSiteItem;
			while ((UnassignedSiteItem = UnassignedCertificatesListBox.SelectedItem) != null)
			{
				UnassignedCertificatesListBox.Items.Remove(UnassignedSiteItem);
				UnassignedSiteItem.Selected = false;

				foreach (ListItem AssignedSiteItem in AssignedCertificatesListBox.Items)
				{
					if (AssignedSiteItem.Text.CompareTo(UnassignedSiteItem.Text) > 0)
					{
						int Index = AssignedCertificatesListBox.Items.IndexOf(AssignedSiteItem);
						AssignedCertificatesListBox.Items.Insert(Index, UnassignedSiteItem);
						UnassignedSiteItem = null;
						break;
					}
				}

				if (UnassignedSiteItem != null)
					AssignedCertificatesListBox.Items.Add(UnassignedSiteItem);
			}

			UpdateAssignedCertificateCollection();
		}

		private void UnassignCertificatesButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			ListItem AssignedSiteItem;
			while ((AssignedSiteItem = AssignedCertificatesListBox.SelectedItem) != null)
			{
				AssignedCertificatesListBox.Items.Remove(AssignedSiteItem);
				AssignedSiteItem.Selected = false;

				foreach (ListItem UnassignedCertificateItem in UnassignedCertificatesListBox.Items)
				{
					if (UnassignedCertificateItem.Text.CompareTo(AssignedSiteItem.Text) > 0)
					{
						int Index = UnassignedCertificatesListBox.Items.IndexOf(UnassignedCertificateItem);
						UnassignedCertificatesListBox.Items.Insert(Index, AssignedSiteItem);
						AssignedSiteItem = null;
						break;
					}
				}

				if (AssignedSiteItem != null)
					UnassignedCertificatesListBox.Items.Add(AssignedSiteItem);
			}

			UpdateAssignedCertificateCollection();
		}

		private void UpdateAssignedCertificateCollection()
		{
			var applicationStringCollection = new ApplicationStringCollectionClass();
			var site = (SiteClass) Session["Site"];

			foreach (ListItem assignedCertificateItem in AssignedCertificatesListBox.Items)
			{
				var assignedApplicationString = new ApplicationStringClass();
				assignedApplicationString.SiteGuid = site.SiteGuid;
				assignedApplicationString.Type = STRING_TYPE.SITE_CERTIFICATE;
				assignedApplicationString.ID = assignedCertificateItem.Text;
				Guid identityGuid;
				if (Guid.TryParse(assignedCertificateItem.Value, out identityGuid))
				{
					assignedApplicationString.IdentityGuid = identityGuid;
				}
				else
				{
					assignedApplicationString.IdentityGuid = Guid.Empty;
				}

				applicationStringCollection.Add(assignedApplicationString);
			}

			site.SiteCertificateCollection = applicationStringCollection;
		}

	}
}