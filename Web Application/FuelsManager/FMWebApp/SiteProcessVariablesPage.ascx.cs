/******************************************************************************
	FILE NAME:		SiteProcessVariablesPage.ascx.cs
	PURPOSE:		Implementation of SiteProcessVariablesPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-10-24	Richard Panachida	Fixed data dictionary. Some labels are not inhieriting from FMControls (CSI 3405).
		2007-03-15	Richard Panachida	Fixed the add permissive issue (received error when adding a permissive to the
										grid) (CSI 4300).
		2007-07-31	I.Orndorff			1.0.0.3 - Modified "SiteOutputsView()" to use 
												  GetTranslatedText() to retrieve the 
												  datadictionaried value of the process
												  variable type. This fixes CSI #4670.
		2009-11-20	W.Gray				Revised to update data prior to redirect to OPCConnectionForm (Wi 9493)
		
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Web.UI.WebControls;

    using Opc;

    using FMBusinessObjects.DataObjects;
    using FMCore;

    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using Convert = System.Convert;

    /// <summary>
    /// Summary description for SiteProcessVariablesPage.
    /// </summary>
    public partial class SiteProcessVariablesPage : FMUserControlBase
	{
	
		private void UpdateSiteOutputsView()
		{
		    this.SiteOutputsDataGrid.DataSource= this.SiteOutputsView();
		    this.SiteOutputsDataGrid.DataBind();
		}

		private ICollection SiteOutputsView()
		{
			DataTable			pvDataTable=new DataTable();

		    pvDataTable.Columns.Add("IdentityGuid",typeof(Guid));
			pvDataTable.Columns.Add("Type",typeof(string));
			pvDataTable.Columns.Add("Host",typeof(string));
			pvDataTable.Columns.Add("OPCServerID",typeof(string));
			pvDataTable.Columns.Add("OPCItemID",typeof(string));

			SiteClass site=(SiteClass)this.Session["Site"];
			foreach(ProcessVariableClass processVariable in site.ProcessVariableCollection)
			{
				if(processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
				{
					continue;
				}

			    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
				if (this.Session["ProcessVariable"] is ProcessVariableClass
				&& ((ProcessVariableClass)this.Session["ProcessVariable"]).ProcessVariableType == processVariable.ProcessVariableType
				&& ((ProcessVariableClass)this.Session["ProcessVariable"]).InstanceNumber == processVariable.InstanceNumber)
				{
					var editedProcessVariable = (ProcessVariableClass)this.Session["ProcessVariable"];
					processVariable.Load(editedProcessVariable);
				    this.Session.Remove("ProcessVariable");
				}

				var				pvDataRow = pvDataTable.NewRow();

				pvDataRow["IdentityGuid"] = processVariable.IdentityGuid;
				pvDataRow["Type"] = this.GetTranslatedText(ProcessVariableClass.ProcessVariableTypeID(processVariable.ProcessVariableType));
				URL	url=new URL(processVariable.URL);
				pvDataRow["Host"] = url.HostName;
				pvDataRow["OPCServerID"] = processVariable.ProgID;
				pvDataRow["OPCItemID"] = processVariable.OPCItemID;
				pvDataTable.Rows.Add(pvDataRow);
			}

			DataView		pvDataView=new DataView(pvDataTable);
			return pvDataView;
		}

		private void UpdateSitePermissivesView()
		{
		    this.SitePermissivesDataGrid.DataSource= this.SitePermissivesView();
		    this.SitePermissivesDataGrid.DataBind();
		}

		private ICollection SitePermissivesView()
		{
			DataTable			pvDataTable=new DataTable();

		    pvDataTable.Columns.Add("IdentityGuid",typeof(Guid));
			pvDataTable.Columns.Add("Host",typeof(string));
			pvDataTable.Columns.Add("OPCServerID",typeof(string));
			pvDataTable.Columns.Add("OPCItemID",typeof(string));
			pvDataTable.Columns.Add("MessageID",typeof(string));

			SiteClass site=(SiteClass)this.Session["Site"];
			foreach(ProcessVariableClass processVariable in site.ProcessVariableCollection)
			{
				if(processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
				{
					continue;
				}

			    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
				if (this.Session["ProcessVariable"] is ProcessVariableClass
				&& ((ProcessVariableClass)this.Session["ProcessVariable"]).ProcessVariableType == processVariable.ProcessVariableType
				&& ((ProcessVariableClass)this.Session["ProcessVariable"]).InstanceNumber == processVariable.InstanceNumber)
				{
					var editedProcessVariable = (ProcessVariableClass)this.Session["ProcessVariable"];
					processVariable.Load(editedProcessVariable);
				    this.Session.Remove("ProcessVariable");
				}

				var				pvDataRow = pvDataTable.NewRow();

				pvDataRow["IdentityGuid"] = processVariable.IdentityGuid;
				var	url=new URL(processVariable.URL);
				pvDataRow["Host"] = url.HostName;
				pvDataRow["OPCServerID"] = processVariable.ProgID;
				pvDataRow["OPCItemID"] = processVariable.OPCItemID;
				pvDataRow["MessageID"] = processVariable.MessageID;

				pvDataTable.Rows.Add(pvDataRow);
			}

			DataView		pvDataView=new DataView(pvDataTable);
			return pvDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				SiteClass site=(SiteClass)this.Session["Site"];

				if (!this.Page.IsPostBack) 
				{
					if (this.Request.GetQueryOrFormValue("ReturnMode") == "CancelAdd")
					{
						var count = site.ProcessVariableCollection.Count;

						if (count > 0)
						{
							site.ProcessVariableCollection.RemoveAt(count - 1);
						}
					}

				    this.UpdateSiteOutputsView();
				    this.UpdateSitePermissivesView();

                    ProcessVariableClass vruSetpointPv = site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV];
                    if (vruSetpointPv != null)
                    {
                        EngineeringUnit units = site.GetSiteUnits(vruSetpointPv.SiteVariableType);
                        byte decimalPlaces = site.GetSiteDecimalPlaces(vruSetpointPv.SiteVariableType);
                        this.SetpointTextBox.Text = vruSetpointPv.Encode(vruSetpointPv.GetValue(units, decimalPlaces),
                                                                                    new Quality(vruSetpointPv.OPCQuality),
                                                                                    units,
                                                                                    site.GetNumberFormatInfo(vruSetpointPv.SiteVariableType));
                    }

                    ProcessVariableClass vruDeadbandPv = site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV];
                    if (vruDeadbandPv != null)
                    {
                        EngineeringUnit units = site.GetSiteUnits(vruDeadbandPv.SiteVariableType);
                        byte decimalPlaces = site.GetSiteDecimalPlaces(vruDeadbandPv.SiteVariableType);
                        this.DeadbandTextBox.Text = vruDeadbandPv.Encode(vruDeadbandPv.GetValue(units, decimalPlaces),
                                                                                    new Quality(vruDeadbandPv.OPCQuality),
                                                                                    units,
                                                                                    site.GetNumberFormatInfo(vruDeadbandPv.SiteVariableType));
                    }

				    this.WatchdogPeriodTextBox.Text = site.WatchdogPeriod.ToString();

                    // Populate WatchdogModeDropDownList
                    WATCHDOG_MODE[] watchdogModes = {   WATCHDOG_MODE.TOGGLE,
                                                                    WATCHDOG_MODE.COUNTER
                                                    };

                    foreach (WATCHDOG_MODE watchdogMode in watchdogModes)
                    {
                        ListItem newConnectItem = new ListItem(watchdogMode.ToString(), ((int)watchdogMode).ToString());
                        this.WatchdogModeDropDownList.Items.Add(newConnectItem);
                        if (watchdogMode == site.WatchdogMode) this.WatchdogModeDropDownList.SelectedIndex = this.WatchdogModeDropDownList.Items.Count - 1;
                    }

				    this.WatchdogModeDropDownList_SelectedIndexChanged(null, null);

                }
                else
				{
				    this.UpdateData();
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
			this.SiteOutputsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.SiteOutputsDataGrid_EditCommand);
			this.SitePermissivesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.SitePermissivesDataGrid_EditCommand);
			this.SitePermissivesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.SitePermissivesDataGrid_PageIndexChanged);
			this.SitePermissivesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.SitePermissivesDataGrid_DeleteCommand);
			this.AddSitePermissiveButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddSitePermissiveButton_Command);

		}
		#endregion

        // ReSharper disable once InconsistentNaming
		private void SiteOutputsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			Label guidLabel = (Label)e.Item.FindControl("SiteOutputIdentityGuidLabel");
			if(guidLabel != null)
			{
				((SiteForm)this.Page).UpdateData();

			    this.Session["UnitForm"]="SiteForm.aspx";
			    this.Session["TabIndex"]=7;
				SiteClass	site=(SiteClass)this.Session["Site"];
			    this.Session["ProcessVariable"]=site.ProcessVariableCollection[new Guid(guidLabel.Text)];
				this.Redirect("OPCConnectionForm.aspx");
			}
		}


        // ReSharper disable once InconsistentNaming
		private void SitePermissivesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			Label guidLabel = (Label)e.Item.FindControl("SitePermissiveIdentityGuidLabel");
			if(guidLabel != null)
			{
				((SiteForm)this.Page).UpdateData();

			    this.Session["UnitForm"]="SiteForm.aspx";
			    this.Session["TabIndex"]=7;
				SiteClass	site=(SiteClass)this.Session["Site"];
			    this.Session["ProcessVariable"]=site.ProcessVariableCollection[new Guid(guidLabel.Text)];
				this.Redirect("OPCConnectionForm.aspx");
			}
		}

        // ReSharper disable once InconsistentNaming
		private void SitePermissivesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			Label guidLabel = (Label)e.Item.FindControl("SitePermissiveIdentityGuidLabel");
			if(guidLabel != null)
			{
				SiteClass site=(SiteClass)this.Session["Site"];
				site.ProcessVariableCollection.Remove(site.ProcessVariableCollection[new Guid(guidLabel.Text)]);

				if(this.SitePermissivesDataGrid.Items.Count == 1
				&& this.SitePermissivesDataGrid.CurrentPageIndex > 0) this.SitePermissivesDataGrid.CurrentPageIndex--;

			    this.UpdateSitePermissivesView();
			}
		}

        // ReSharper disable once InconsistentNaming
		private void AddSitePermissiveButton_Command(object sender, CommandEventArgs e)
		{
			SiteClass site = (SiteClass)this.Session["Site"];
			int	instanceNumber = 0;

			foreach(ProcessVariableClass processVariable in site.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
				{
					continue;
				}

				if (processVariable.InstanceNumber > instanceNumber)
				{
					instanceNumber = processVariable.InstanceNumber;
				}
			}

		    ProcessVariableClass newProcessVariable = new ProcessVariableClass
		                                                  {
		                                                      UnitType = UNIT_TYPE.SITE_UNIT,
		                                                      ProcessVariableType = PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV,
		                                                      InstanceNumber = instanceNumber + 1,
		                                                      DataType = VarEnum.VT_BOOL,
		                                                      DataTypeEnabled = false
		                                                  };

		    site.ProcessVariableCollection.Add(newProcessVariable);

			((SiteForm)this.Page).UpdateData();

		    this.Session["UnitForm"]="SiteForm.aspx";
		    this.Session["TabIndex"]=7;
		    this.Session["ProcessVariable"]=newProcessVariable;
			this.Redirect("OPCConnectionForm.aspx?Mode=Add");
		}

        // ReSharper disable once InconsistentNaming
		private void SitePermissivesDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.SitePermissivesDataGrid.EditItemIndex > -1)
				return;
		    this.SitePermissivesDataGrid.CurrentPageIndex = e.NewPageIndex;
		    this.UpdateSitePermissivesView();
		}

        public void UpdateData()
        {
            SiteClass site = (SiteClass)this.Session["Site"];

            site.WatchdogPeriod = Convert.ToInt32(this.WatchdogPeriodTextBox.Text);

            if (this.WatchdogModeDropDownList.SelectedIndex != -1)
            {
                site.WatchdogMode = (WATCHDOG_MODE)Convert.ToInt32(this.WatchdogModeDropDownList.SelectedValue);
            }

            if (site.WatchdogMode == WATCHDOG_MODE.COUNTER)
            {
                if (this.CounterStartTextBox.Text != "")
                {
                    site.WatchdogCounterStart = this.CounterStartTextBox.Text;
                }

                if (this.CounterEndTextBox.Text != "")
                {
                    site.WatchdogCounterEnd = this.CounterEndTextBox.Text;
                }
            }

            ProcessVariableClass vruSetpointPv = site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV];
            if (vruSetpointPv != null)
            {
                EngineeringUnit units = site.GetSiteUnits(vruSetpointPv.SiteVariableType);
                vruSetpointPv.SetValue(this.SetpointTextBox.Text, units, site.GetNumberFormatInfo(vruSetpointPv.SiteVariableType));
            }

            ProcessVariableClass vruDeadbandPv = site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV];
            if (vruDeadbandPv != null)
            {
                EngineeringUnit units = site.GetSiteUnits(vruDeadbandPv.SiteVariableType);
                vruDeadbandPv.SetValue(this.DeadbandTextBox.Text, units, site.GetNumberFormatInfo(vruDeadbandPv.SiteVariableType));
            }
        }

        // ReSharper disable once InconsistentNaming
        protected void WatchdogModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SiteClass site = (SiteClass)this.Session["Site"];

            if ((WATCHDOG_MODE)Convert.ToInt32(this.WatchdogModeDropDownList.SelectedValue) == WATCHDOG_MODE.TOGGLE)
            {
                this.CounterStartTextBox.Text = "";
                this.CounterStartTextBox.Enabled = false;
                this.CounterStartTextBox.BackColor = Color.LightGray;
                this.CounterEndTextBox.Text = "";
                this.CounterEndTextBox.Enabled = false;
                this.CounterEndTextBox.BackColor = Color.LightGray;
            }
            else
            {
                this.CounterStartTextBox.Text = site.WatchdogCounterStart;
                this.CounterStartTextBox.Enabled = true;
                this.CounterStartTextBox.BackColor = Color.White;
                this.CounterEndTextBox.Text = site.WatchdogCounterEnd;
                this.CounterEndTextBox.Enabled = true;
                this.CounterEndTextBox.BackColor = Color.White;
            }
        }
    }
}
