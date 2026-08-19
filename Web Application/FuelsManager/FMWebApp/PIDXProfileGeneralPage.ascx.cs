/******************************************************************************

	FILE NAME:		PDIXProfileGeneralPage.ascx.cs


	PURPOSE:			Implementation of PIDXProfileGeneralPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///		Summary description for PIDXProfileGeneralPage.
	/// </summary>
	public partial class PIDXProfileGeneralPage : FMUserControlBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				PIDXProfileClass pidxProfile=this.Session["PIDXProfile"] as PIDXProfileClass;

				if (! this.Page.IsPostBack) 
				{
					// TypeDropDownList
					for(PIDXType type=PIDXType.Tds; type < PIDXType.MaxPIDX; type++)
					{
						ListItem newTypeItem=new ListItem(PIDXProfileClass.TypeID(type),((int) type).ToString());
						this.TypeDropDownList.Items.Add(newTypeItem);
					    if (pidxProfile != null && pidxProfile.Type == type)
					    {
					        this.TypeDropDownList.SelectedIndex=this.TypeDropDownList.Items.Count-1;
					    }	
					}

                    // VersionDropDownList
                    for (PIDXVersion version = PIDXVersion.OneDotZeroTwo; version < PIDXVersion.MaxVersion; version++)
                    {
                        ListItem newVersionItem = new ListItem(PIDXProfileClass.VersionID(version), ((int)version).ToString());
                        this.VersionDropDownList.Items.Add(newVersionItem);
                        if (pidxProfile != null && pidxProfile.Version == version)
                        {
                            this.VersionDropDownList.SelectedIndex = this.VersionDropDownList.Items.Count - 1;
                        }
                    }

				    if (pidxProfile != null)
				    {
				        this.IDTextBox.Text=pidxProfile.ID;
				        this.IPAddressTextBox.Text=pidxProfile.IPAddress;
				        this.PortTextBox.Text=pidxProfile.Port.ToString();
				        this.TerminalIDTextBox.Text=pidxProfile.TerminalID;
				        this.UserIDTextBox.Text=pidxProfile.UserID;
				        this.PasswordTextBox.Text=pidxProfile.Password;
				        this.EnabledCheckBox.Checked=pidxProfile.Enabled;
				        this.LoggingEnabledCheckBox.Checked = pidxProfile.LoggingEnabled;
				        this.LogFileTextbox.Text = pidxProfile.LogFilePath;
				    }
				}

				this.InitialPasswordTextBox.Text = this.PasswordTextBox.Text;
			}	
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void UpdateData()
		{
			PIDXProfileClass pidxProfile= this.Session["PIDXProfile"] as PIDXProfileClass;

		    if (pidxProfile != null)
		    {
		        pidxProfile.Type=(PIDXType) Convert.ToInt32(this.TypeDropDownList.SelectedValue);
                pidxProfile.Version = (PIDXVersion)Convert.ToInt32(this.VersionDropDownList.SelectedValue);
                pidxProfile.ID= this.IDTextBox.Text;
		        pidxProfile.IPAddress= this.IPAddressTextBox.Text;

		        try
		        {
		            pidxProfile.Port=Convert.ToInt32(this.PortTextBox.Text);
		        }
		        catch
		        {
		            this.ErrorHandler(new Exception("Invalid Port"));
		            this.Response.End();
		        }

		        pidxProfile.TerminalID= this.TerminalIDTextBox.Text;
		        pidxProfile.UserID= this.UserIDTextBox.Text;
                //if (this.PasswordTextBox.Text != "")
                //    pidxProfile.Password = this.PasswordTextBox.Text;
                if (this.InitialPasswordTextBox.Text != "")
				{
					pidxProfile.Password = this.InitialPasswordTextBox.Text;
				}

				pidxProfile.Enabled= this.EnabledCheckBox.Checked;
		        pidxProfile.LoggingEnabled = this.LoggingEnabledCheckBox.Checked;
		        pidxProfile.LogFilePath = this.LogFileTextbox.Text;
		    }
		}
	}
}
