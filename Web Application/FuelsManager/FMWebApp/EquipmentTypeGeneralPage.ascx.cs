/******************************************************************************
	FILE NAME:		EquipmentTypeGeneralPage.ascx.cs
	PURPOSE:		Implementation of EquipmentTypeGeneralPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		---------	-----------------	-------------------------------------------
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public partial class EquipmentTypeGeneralPage : FMUserControlBase
	{
		protected EquipmentTypeClass EquipmentType
		{
			get
			{
				return ((EquipmentTypeDetailsForm)this.Page).EquipmentType;
			}
		}

		protected void Page_Load(object sender,EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, getMemberSites:false, getSchedulesAndProcessVariables:false, bGetAssociatedAliases:false));
				
				// Set units so SI values are converted properly
				this.EquipmentType.SICapacity.Units = site.VolumeUnits;
				this.EquipmentType.SISafeFill.Units = site.VolumeUnits;

				this.EquipmentTypeIDTextbox.Text = this.EquipmentType.ID;
				this.DescriptionTextbox.Text = this.EquipmentType.Description;
				this.IssptTextbox.Text = this.EquipmentType.Isspt;
				this.CapacityTextbox.Text = this.EquipmentType.Capacity;
				this.SafeFillTextbox.Text = this.EquipmentType.SafeFill;
				this.ModelTextbox.Text = this.EquipmentType.Model;
				this.MakeTextbox.Text = this.EquipmentType.Make;
				this.YearTextbox.Text = (this.EquipmentType.Year == 0 ? "" : this.EquipmentType.Year.ToString());
				this.MultiCompartmentCheckBox.Checked = this.EquipmentType.IsMultiCompartment;


				for(EQUIPMENT_TYPE i = (EQUIPMENT_TYPE)0;i < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;i++)
				{
					if(EQUIPMENT_TYPE.COMPARTMENT_TYPE == i)
						continue;
					this.AttributeDropDownList.Items.Add(new System.Web.UI.WebControls.ListItem(EquipmentTypeClass.TypeID(i),((int)i).ToString()));
				}

				this.AttributeDropDownList.SelectByText(EquipmentTypeClass.TypeID(this.EquipmentType.Attribute));

				for (COMPANY_ROLE role = 0; role < COMPANY_ROLE.MAX_COMPANY_ROLE; role++)
				{
					this.CompanyRoleDropDownList.Items.Add(new ListItem(this.GetTranslatedText(CompanyRoleMapClass.RoleID(role)), ((int)role).ToString()));

					if (role == this.EquipmentType.CompanyRoleAssignmentConstraint)
						this.CompanyRoleDropDownList.SelectedIndex = this.CompanyRoleDropDownList.Items.Count - 1;

				}

				this.CompanyRoleDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText("{Any}"), ((int)COMPANY_ROLE.MAX_COMPANY_ROLE).ToString()));

                bool bselected = this.AttributeDropDownList.SelectByText(EquipmentTypeClass.TypeID(EquipmentType.Attribute));
                if (!bselected)
                {
                    EventArgs evt = new EventArgs();
                    AttributeDropDownList_SelectedIndexChanged(AttributeDropDownList, evt);
                }
				this.EquipmentTypeIDTextbox.Focus();
			}

			this.MultiCompartmentCheckBox.Visible = this.EquipmentType.Attribute.IsMultiCompartmentCapable();
		}

		#region private methods

		public void UpdateData()
		{
			EquipmentTypeClass EquipmentType = this.Session["SelectedEquipmentType"] as EquipmentTypeClass;
			EquipmentType.ID = this.EquipmentTypeIDTextbox.Text;
			EquipmentType.Description= this.DescriptionTextbox.Text;
			EquipmentType.Isspt= this.IssptTextbox.Text;
			EquipmentType.Capacity= this.CapacityTextbox.Text;
			EquipmentType.SafeFill= this.SafeFillTextbox.Text;
			EquipmentType.Model= this.ModelTextbox.Text;
			EquipmentType.Make= this.MakeTextbox.Text;
			try
			{
				if (this.YearTextbox.Text.Trim() == "")
					EquipmentType.Year = 0;
				else
					EquipmentType.Year = System.Convert.ToInt32(this.YearTextbox.Text);
			}
			catch
			{
				throw new Exception("Invalid entry for Equipment Year.");
			}

			EquipmentType.Attribute= (EQUIPMENT_TYPE)System.Convert.ToInt32(this.AttributeDropDownList.SelectedValue);
			EquipmentType.CompanyRoleAssignmentConstraint = (COMPANY_ROLE)Convert.ToInt32(this.CompanyRoleDropDownList.SelectedValue);
			EquipmentType.IsMultiCompartment = this.MultiCompartmentCheckBox.Checked;
		}

		#endregion

		protected void AttributeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			string redirectString = "EquipmentTypeDetailsForm.aspx";

			try
			{
				if (this.EquipmentType.Attribute.IsMultiCompartmentCapable() == false)
				{
					this.EquipmentType.IsMultiCompartment = false;
					this.MultiCompartmentCheckBox.Checked = false;
				}

				((EquipmentTypeDetailsForm)this.Page).UpdateData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect(redirectString);
		}

		public void SetReadOnly()
		{
			DisableControls();
		}
	}
}