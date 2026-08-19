// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductMessagesPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductMessagesPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	///    Summary description for ProductMessagesPage.
	/// </summary>
	public partial class ProductMessagesPage : ProductPageBase
	{
		#region Public Methods and Operators
		public void UpdateData()
		{
			ApplicationStringMapCollectionClass messageCollection = null;
			var stringMapType = STRING_MAP_TYPE.MAX_STRING_MAP_TYPE;

			if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.PRODUCT_MESSAGE)
			{
				stringMapType = STRING_MAP_TYPE.PRODUCT_MESSAGE;
				messageCollection = this.Product.ProductMessageCollection;
			}

			else if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.DOT_HAZARDOUS_MESSAGE)
			{
				stringMapType = STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE;
				messageCollection = this.Product.HazardousMaterialMessageCollection;
			}

			if (messageCollection != null)
			{
				messageCollection.Clear();

				for (int itemInt = 0; itemInt < this.AssignedMessagesListBox.Items.Count; itemInt++)
				{
					var message = new ApplicationStringMapClass
					              {
						              AssignedToGuid = this.Product.IdentityGuid,
						              ApplicationStringGuid = Guid.Parse(this.AssignedMessagesListBox.Items[itemInt].Value),
						              ID = this.AssignedMessagesListBox.Items[itemInt].Text,
						              Sequence = itemInt,
						              Type = stringMapType
					              };

					messageCollection.Add(message);
				}
			}
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
				if (!this.Page.IsPostBack)
				{
					STRING_TYPE[] stringTypes = { STRING_TYPE.PRODUCT_MESSAGE, STRING_TYPE.DOT_HAZARDOUS_MESSAGE };
					var applicationString = new ApplicationStringClass();
					
					foreach (STRING_TYPE stringType in stringTypes)
					{
						applicationString.Type = stringType;
						string entityTypeID = EntityToSiteMapClass.GetEntityTypeID(applicationString.EntityType);
						this.TypeDropDownList.Items.Add(new ListItem(entityTypeID, 
																	((int)applicationString.Type).ToString(CultureInfo.InvariantCulture)));
					}

					this.TypeDropDownListSelectedIndexChanged(null, null);
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			ApplicationStringMapCollectionClass messageCollection = null;
			var type = STRING_TYPE.MAX_STRING_TYPE;

			if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.PRODUCT_MESSAGE)
			{
				type = STRING_TYPE.PRODUCT_MESSAGE;
				messageCollection = this.Product.ProductMessageCollection;
			}

			else if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.DOT_HAZARDOUS_MESSAGE)
			{
				type = STRING_TYPE.DOT_HAZARDOUS_MESSAGE;
				messageCollection = this.Product.HazardousMaterialMessageCollection;
			}

			// Populate the AssignedMessagesListBox
			this.AssignedMessagesListBox.Items.Clear();

			if (messageCollection != null)
			{
				foreach (ApplicationStringMapClass message in messageCollection)
				{
					this.AssignedMessagesListBox.Items.Add(new ListItem(message.ID, message.ApplicationStringGuid.ToString()));
				}
			}

			// Populate the UnassignedMessagesListBox
			this.UnassignedMessagesListBox.Items.Clear();
			ApplicationStringCollectionClass unassignedMessageCollection = 
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, type)
																);

			if (unassignedMessageCollection != null)
			{
				foreach (ApplicationStringClass unassignedMessage in unassignedMessageCollection)
				{
					if (this.AssignedMessagesListBox.Items.FindByValue(unassignedMessage.IdentityGuid.ToString()) == null)
					{
						this.UnassignedMessagesListBox.Items.Add(
							new ListItem(unassignedMessage.ID, unassignedMessage.IdentityGuid.ToString()));
					}
				}
			}
		}

		private void AssignMessagesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem messageItem;
			
			while ((messageItem = this.UnassignedMessagesListBox.SelectedItem) != null)
			{
				this.UnassignedMessagesListBox.Items.Remove(messageItem);
				messageItem.Selected = false;
				this.AssignedMessagesListBox.Items.Add(messageItem);
			}

			this.UpdateData();
		}

		private void DownButtonCommand(object sender, CommandEventArgs e)
		{
			int selectedInt = this.AssignedMessagesListBox.SelectedIndex;
			int countInt = this.AssignedMessagesListBox.Items.Count;
			
			if (selectedInt < countInt - 1)
			{
				for (int itemInt = countInt - 1; itemInt >= 0; itemInt--)
				{
					if (!this.AssignedMessagesListBox.Items[itemInt].Selected && itemInt > 0
					    && this.AssignedMessagesListBox.Items[itemInt - 1].Selected)
					{
						ListItem moveItem = this.AssignedMessagesListBox.Items[itemInt];
						this.AssignedMessagesListBox.Items.RemoveAt(itemInt);
						this.AssignedMessagesListBox.Items.Insert(itemInt - 1, moveItem);
					}
				}
			}

			this.UpdateData();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UpButton.Command				+= this.UpButtonCommand;
			this.UnassignMessagesButton.Command += this.UnassignMessagesButtonCommand;
			this.AssignMessagesButton.Command	+= this.AssignMessagesButtonCommand;
			this.DownButton.Command				+= this.DownButtonCommand;
		}

		private void UnassignMessagesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedItem;
			
			while ((assignedItem = this.AssignedMessagesListBox.SelectedItem) != null)
			{
				this.AssignedMessagesListBox.Items.Remove(assignedItem);
				assignedItem.Selected = false;

				foreach (ListItem unassignedItem in this.UnassignedMessagesListBox.Items)
				{
					if (String.Compare(unassignedItem.Text, assignedItem.Text, StringComparison.Ordinal) > 0)
					{
						int Index = this.UnassignedMessagesListBox.Items.IndexOf(unassignedItem);
						this.UnassignedMessagesListBox.Items.Insert(Index, assignedItem);
						assignedItem = null;
						break;
					}
				}

				if (assignedItem != null)
				{
					this.UnassignedMessagesListBox.Items.Add(assignedItem);
				}
			}

			this.UpdateData();
		}

		private void UpButtonCommand(object sender, CommandEventArgs e)
		{
			int selectedInt = this.AssignedMessagesListBox.SelectedIndex;
			int countInt = this.AssignedMessagesListBox.Items.Count;
			
			if (selectedInt > 0)
			{
				for (int itemInt = selectedInt - 1; itemInt < countInt; itemInt++)
				{
					if (!this.AssignedMessagesListBox.Items[itemInt].Selected && itemInt < countInt - 1
					    && this.AssignedMessagesListBox.Items[itemInt + 1].Selected)
					{
						ListItem moveItem = this.AssignedMessagesListBox.Items[itemInt];
						this.AssignedMessagesListBox.Items.RemoveAt(itemInt);
						this.AssignedMessagesListBox.Items.Insert(itemInt + 1, moveItem);
					}
				}
			}

			this.UpdateData();
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if ((this.Product.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }

            this.UpButton.Enabled = (this.UpButton.Enabled && this.VersionSpecificFields.Contains("Messages"));
            this.DownButton.Enabled = (this.DownButton.Enabled && this.VersionSpecificFields.Contains("Messages"));
            this.AssignMessagesButton.Enabled = (this.AssignMessagesButton.Enabled && this.VersionSpecificFields.Contains("Messages"));
            this.UnassignMessagesButton.Enabled = (this.UnassignMessagesButton.Enabled && this.VersionSpecificFields.Contains("Messages"));
        }
		#endregion
	}
}