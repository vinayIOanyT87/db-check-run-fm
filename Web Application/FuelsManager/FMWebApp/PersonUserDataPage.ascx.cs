/******************************************************************************
	FILE NAME:		PersonUserDataPage.ascx.cs
	PURPOSE:		Implementation of PersonUserDataPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	7.4.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2009-07-31	A. Coker	         WI 5055 - Created page.

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for PersonUserDataPage.
	/// </summary>
	public partial class PersonUserDataPage : FMUserDataControlBase
	{
		protected FMControls.FMLabel Label20;
		protected TextBox PersonIDTextbox;
		protected FMControls.FMLabel Label21;
		protected DropDownList PersonNameDropDownList;
		protected SiteClass CurrentSite;

		protected List<string> VersionSpecificFields
		{
			get { return ((PersonForm) this.Page).VersionSpecificFields; }
		}

		protected PersonClass Person
		{
			get { return ((PersonForm) this.Page).Person; }
		}

		protected override Table Table
		{
			get { return this.UserDataTable;}
		}

		protected override ENTITY_TYPE EntityType
		{
			get
			{
				var person = new PersonClass();
				return person.EntityType;
			}
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
			if ((this.Person.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))))
			{
				return;
			}

			int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

				if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
				{
					var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					valueTextBox.Enabled = (valueTextBox.Enabled 
											&& (this.VersionSpecificFields != null)
                                            && this.VersionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
				}
				else
				{
					var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					valueDropDownList.Enabled = (valueDropDownList.Enabled 
												&& (this.VersionSpecificFields != null)
                                                && this.VersionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

					if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueTextBox.Enabled = (valueTextBox.Enabled
												&& (this.VersionSpecificFields != null)
                                                && this.VersionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
					else
					{
						var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueDropDownList.Enabled = (valueDropDownList.Enabled
													&& (this.VersionSpecificFields != null)
                                                    && this.VersionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
				}

				index++;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);

				if (!this.Page.IsPostBack)
				{
					int index = 0;

					foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
					{
						UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							valueTextBox.Text = this.Person.UserData[userDataField1.Number];
						}
						else
						{
							var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							ListItem item = valueDropDownList.Items.FindByText(this.Person.UserData[userDataField1.Number]);

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(this.Person.UserData[userDataField1.Number], this.Person.UserData[userDataField1.Number]));
								valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
							}
							else
							{
								valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf(item);
							}
						}

						if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
						{
							UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

							if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
							{
								var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								valueTextBox.Text = this.Person.UserData[userDataField2.Number];
							}
							else
							{
								var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								ListItem item = valueDropDownList.Items.FindByText(this.Person.UserData[userDataField2.Number]);

								if (item == null)
								{
									valueDropDownList.Items.Add(
										new ListItem(this.Person.UserData[userDataField2.Number], this.Person.UserData[userDataField2.Number]));
									valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
								}
								else
								{
									valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf(item);
								}
							}
						}
						
						index++;
					}

					this.SetFieldAccessibilityForChildRecordVersion();
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
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		public void UpdateData()
		{
			if (this.Person != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
				bool dontPerformRvCheck = ((this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))));

				int index = 0;

				foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
				{
					UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

					if (dontPerformRvCheck 
						|| ((this.VersionSpecificFields != null)
                            && this.VersionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1)))
					{
						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							this.Person.UserData[userDataField1.Number] = valueTextBox.Text;
						}
						else
						{
							var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							this.Person.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
						}
					}

					if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
					{
						if (dontPerformRvCheck
						|| ((this.VersionSpecificFields != null)
                            && this.VersionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2)))
						{
							UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

							if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
							{
								var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								this.Person.UserData[userDataField2.Number] = valueTextBox.Text;
							}
							else
							{
								var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								this.Person.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
							}
						}
					}

					index++;
				}
			}
		}
	}
}

